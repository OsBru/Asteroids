using UnityEngine;

namespace Asteroids.SkinSystem
{
    /// <summary>
    /// Aplica el sprite o l'animació de la nau del jugador segons el GameSkinData actiu.
    /// Si la skin té un AnimatorController assignat, s'usa l'Animator i s'ignora l'sprite estàtic.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShipSkinApplier : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer shipRenderer;
        [SerializeField] private SpriteRenderer thrusterRenderer;
        [SerializeField] private Transform thrusterParticleMount;

        private GameObject currentThrusterParticleInstance;
        private Animator shipAnimator;
        private string currentThrustParam;

        private void Awake()
        {
            if (shipRenderer == null)
                shipRenderer = GetComponent<SpriteRenderer>();

            // Agafem l'Animator si ja existeix al GameObject; si no, no el creem (es crea a ApplySkin si cal)
            shipAnimator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            SkinManager.OnSkinChanged += ApplySkin;
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
                ApplySkin(SkinManager.Instance.CurrentSkin);
        }

        private void OnDisable()
        {
            SkinManager.OnSkinChanged -= ApplySkin;
        }

        // ── Cridat des de PlayerController per sincronitzar el paràmetre de propulsió ──
        /// <summary>
        /// Notifica a l'Animator (si n'hi ha) si la nau s'està propulsant o no.
        /// </summary>
        public void SetThrusting(bool isThrusting)
        {
            if (shipAnimator == null || string.IsNullOrEmpty(currentThrustParam)) return;
            shipAnimator.SetBool(currentThrustParam, isThrusting);
        }

        public void ApplySkin(GameSkinData skin)
        {
            if (skin == null) return;

            // ── Nau: animator té prioritat sobre sprite ───────────────────────
            if (skin.shipAnimator != null)
            {
                // Afegim l'Animator si no existia
                if (shipAnimator == null)
                    shipAnimator = gameObject.AddComponent<Animator>();

                shipAnimator.enabled = true;
                shipAnimator.runtimeAnimatorController = skin.shipAnimator;
                currentThrustParam = skin.thrustParamName;

                // Amb Animator, el SpriteRenderer el controla el propi animator;
                // assegurem que el color/tint s'aplica igualment.
                if (shipRenderer != null)
                    shipRenderer.color = skin.shipColor;
            }
            else
            {
                // Mode estàtic: desactivem l'Animator si n'hi havia
                if (shipAnimator != null)
                    shipAnimator.enabled = false;

                currentThrustParam = null;

                if (shipRenderer != null)
                {
                    if (skin.shipSprite != null)
                        shipRenderer.sprite = skin.shipSprite;
                    shipRenderer.color = skin.shipColor;
                }
            }

            // ── Propulsor ────────────────────────────────────────────────────
            if (thrusterRenderer != null && skin.thrusterSprite != null)
                thrusterRenderer.sprite = skin.thrusterSprite;

            // ── Partícules del propulsor ─────────────────────────────────────
            if (thrusterParticleMount != null)
            {
                if (currentThrusterParticleInstance != null)
                    Destroy(currentThrusterParticleInstance);

                if (skin.thrusterParticlePrefab != null)
                {
                    currentThrusterParticleInstance = Instantiate(
                        skin.thrusterParticlePrefab, thrusterParticleMount);
                    currentThrusterParticleInstance.transform.localPosition = Vector3.zero;
                    currentThrusterParticleInstance.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }
}
