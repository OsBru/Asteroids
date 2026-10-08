using UnityEngine;
using Asteroids.Asteroids;

namespace Asteroids.SkinSystem
{
    /// <summary>
    /// Aplica el sprite o l'animació als asteroides segons la seva mida i el tema actual.
    /// Si la skin té un AnimatorController per al tier corresponent, té prioritat sobre l'sprite estàtic.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class AsteroidSkinApplier : MonoBehaviour
    {
        [SerializeField] private AsteroidTier tier = AsteroidTier.Large;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public AsteroidTier Tier => tier;

        private Animator asteroidAnimator;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            asteroidAnimator = GetComponent<Animator>();
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

        public void SetTier(AsteroidTier newTier)
        {
            tier = newTier;
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
                ApplySkin(SkinManager.Instance.CurrentSkin);
        }

        public void ApplySkin(GameSkinData skin)
        {
            if (skin == null || spriteRenderer == null) return;

            var animCtrl = skin.GetAsteroidAnimator(tier);

            if (animCtrl != null)
            {
                // ── Mode animat ──────────────────────────────────────────────
                if (asteroidAnimator == null)
                    asteroidAnimator = gameObject.AddComponent<Animator>();

                asteroidAnimator.enabled = true;
                asteroidAnimator.runtimeAnimatorController = animCtrl;

                // El tint s'aplica sempre al SpriteRenderer
                spriteRenderer.color = skin.asteroidTint;
            }
            else
            {
                // ── Mode estàtic ─────────────────────────────────────────────
                if (asteroidAnimator != null)
                    asteroidAnimator.enabled = false;

                Sprite s = skin.GetAsteroidSprite(tier);
                if (s != null)
                    spriteRenderer.sprite = s;

                spriteRenderer.color = skin.asteroidTint;
            }
        }
    }
}
