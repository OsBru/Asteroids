using UnityEngine;
using Asteroids.SkinSystem;
using Asteroids.Audio;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Asteroids.Player
{
    /// <summary>
    /// Gestiona els trets de la nau, la cadència de foc i el so de dispar del tema actiu.
    /// </summary>
    public class PlayerShooting : MonoBehaviour
    {
        [Header("Configuració")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private float fireRate = 0.22f;

        private float nextFireTime;
        private Rigidbody2D shipRb;
        private PlayerController playerController;

        private void Awake()
        {
            shipRb = GetComponent<Rigidbody2D>();
            playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (playerController != null && !playerController.CanControl) return;

            bool fireRequested = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.ctrlKey.wasPressedThisFrame))
            {
                fireRequested = true;
            }
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                fireRequested = true;
            }
#else
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
            {
                fireRequested = true;
            }
#endif

            if (fireRequested && Time.time >= nextFireTime)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            nextFireTime = Time.time + fireRate;

            Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : transform.position + transform.up * 0.5f;
            Quaternion spawnRot = transform.rotation;

            if (projectilePrefab != null)
            {
                GameObject bulletObj = Instantiate(projectilePrefab, spawnPos, spawnRot);
                if (bulletObj.TryGetComponent<Projectile>(out var projectile))
                {
                    Vector2 shipVel = shipRb != null ? shipRb.linearVelocity : Vector2.zero;
                    projectile.Launch(transform.up, shipVel);
                }
            }

            // Reproducció de so del tema actiu
            if (AudioManager.Instance != null && SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                AudioManager.Instance.PlaySfx(SkinManager.Instance.CurrentSkin.shootSfx);
            }
        }
    }
}
