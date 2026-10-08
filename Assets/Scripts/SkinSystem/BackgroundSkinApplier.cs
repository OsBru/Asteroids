using UnityEngine;

namespace Asteroids.SkinSystem
{
    /// <summary>
    /// Actualitza el color de fons de la càmera o el sprite de fons segons el tema actiu.
    /// </summary>
    public class BackgroundSkinApplier : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private SpriteRenderer backgroundRenderer;

        private void Awake()
        {
            if (targetCamera == null) targetCamera = Camera.main;
        }

        private void OnEnable()
        {
            SkinManager.OnSkinChanged += ApplySkin;
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                ApplySkin(SkinManager.Instance.CurrentSkin);
            }
        }

        private void OnDisable()
        {
            SkinManager.OnSkinChanged -= ApplySkin;
        }

        private void ApplySkin(GameSkinData skin)
        {
            if (skin == null) return;

            if (targetCamera != null)
            {
                targetCamera.backgroundColor = skin.backgroundColor;
            }

            if (backgroundRenderer != null)
            {
                backgroundRenderer.sprite = skin.backgroundSprite;
                backgroundRenderer.gameObject.SetActive(skin.backgroundSprite != null);
            }
        }
    }
}
