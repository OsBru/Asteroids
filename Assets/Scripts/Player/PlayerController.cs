using System.Collections;
using UnityEngine;
using Asteroids.SkinSystem;
using Asteroids.Audio;
using Asteroids.Core;
using Asteroids.Asteroids;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Asteroids.Player
{
    /// <summary>
    /// Controlador de la nau espacial amb físiques clàssiques 2D (inèrcia, rotació i propulsió).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Físiques i Moviment")]
        [SerializeField] private float thrustForce = 8f;
        [SerializeField] private float rotationSpeed = 220f;
        [SerializeField] private float maxSpeed = 12f;

        [Header("Efectes Visuals")]
        [SerializeField] private GameObject thrusterVisual;
        [SerializeField] private SpriteRenderer shipRenderer;

        [Header("Invulnerabilitat")]
        [SerializeField] private float respawnInvulnerabilityDuration = 3f;
        [SerializeField] private float blinkInterval = 0.15f;

        private Rigidbody2D rb;
        private Collider2D col;
        private bool isThrusting;
        private float rotationInput;
        public bool CanControl { get; private set; } = true;
        public bool IsInvulnerable { get; private set; } = false;

        private ShipSkinApplier shipSkinApplier;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            if (shipRenderer == null) shipRenderer = GetComponent<SpriteRenderer>();
            shipSkinApplier = GetComponent<ShipSkinApplier>();

            // Configurar propietats físiques típiques d'Asteroids (espai sense fregament pesat)
            rb.gravityScale = 0f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 2f;
        }

        private void Start()
        {
            StartCoroutine(RespawnInvulnerabilityRoutine());
        }

        private void Update()
        {
            if (!CanControl)
            {
                isThrusting = false;
                rotationInput = 0f;
                UpdateThrusterVisual(false);
                return;
            }

            // Lectura de controls (suport Input System i fallback clàssic)
            ReadInputs();

            // Actualització visual del propulsor
            UpdateThrusterVisual(isThrusting);

            // Sincronitzar l'estat de propulsió amb l'Animator de la nau (si n'hi ha)
            shipSkinApplier?.SetThrusting(isThrusting);

            // Àudio de propulsió
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetThrustLoopActive(isThrusting);
            }
        }


        private void FixedUpdate()
        {
            if (!CanControl) return;

            // Rotació de la nau
            if (Mathf.Abs(rotationInput) > 0.01f)
            {
                float rotAmount = -rotationInput * rotationSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation + rotAmount);
            }

            // Propulsió cap endavant
            if (isThrusting)
            {
                Vector2 forward = transform.up;
                rb.AddForce(forward * thrustForce, ForceMode2D.Force);

                // Limitar velocitat màxima per mantenir la jugabilitat controlable
                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }
            }
        }

        private void ReadInputs()
        {
            float rot = 0f;
            bool thrust = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    rot -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    rot += 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                    thrust = true;
            }
#else
            rot = Input.GetAxisRaw("Horizontal");
            thrust = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetButton("Vertical");
#endif

            rotationInput = rot;
            isThrusting = thrust;
        }

        private void UpdateThrusterVisual(bool active)
        {
            if (thrusterVisual != null && thrusterVisual.activeSelf != active)
            {
                thrusterVisual.SetActive(active);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsInvulnerable || !CanControl) return;

            if (collision.gameObject.GetComponent<Asteroid>() != null)
            {
                Die();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsInvulnerable || !CanControl) return;

            if (collision.GetComponent<Asteroid>() != null)
            {
                Die();
            }
        }

        public void Die()
        {
            if (!CanControl) return;
            CanControl = false;
            UpdateThrusterVisual(false);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetThrustLoopActive(false);
            }

            // Instanciar explosió del tema actiu
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                var skin = SkinManager.Instance.CurrentSkin;
                if (skin.shipExplosionPrefab != null)
                {
                    Instantiate(skin.shipExplosionPrefab, transform.position, Quaternion.identity);
                }
                if (AudioManager.Instance != null && skin.shipExplosionSfx != null)
                {
                    AudioManager.Instance.PlaySfx(skin.shipExplosionSfx);
                }
            }

            // Ocultar nau i desactivar col·lisions
            if (shipRenderer != null) shipRenderer.enabled = false;
            if (col != null) col.enabled = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Notificar al GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDied();
            }
        }

        public void Respawn(Vector3 position)
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            if (shipRenderer != null) shipRenderer.enabled = true;
            if (col != null) col.enabled = true;

            CanControl = true;
            StartCoroutine(RespawnInvulnerabilityRoutine());
        }

        private IEnumerator RespawnInvulnerabilityRoutine()
        {
            IsInvulnerable = true;
            float elapsed = 0f;

            while (elapsed < respawnInvulnerabilityDuration)
            {
                if (shipRenderer != null)
                {
                    shipRenderer.enabled = !shipRenderer.enabled;
                }
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            if (shipRenderer != null) shipRenderer.enabled = true;
            IsInvulnerable = false;
        }

        private void OnDisable()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetThrustLoopActive(false);
            }
        }
    }
}
