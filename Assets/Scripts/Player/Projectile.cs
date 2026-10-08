using UnityEngine;
using Asteroids.SkinSystem;
using Asteroids.Asteroids;

namespace Asteroids.Player
{
    /// <summary>
    /// Bales o làsers disparats per la nau del jugador.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 14f;
        [SerializeField] private float lifetime = 1.6f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
            ApplyCurrentSkin();
        }

        public void Launch(Vector2 direction, Vector2 shipVelocity)
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            // La bala hereta una part de la inèrcia de la nau + la seva pròpia velocitat
            rb.linearVelocity = (direction.normalized * speed) + (shipVelocity * 0.25f);
        }

        private void ApplyCurrentSkin()
        {
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                var skin = SkinManager.Instance.CurrentSkin;
                if (spriteRenderer != null && skin.projectileSprite != null)
                {
                    spriteRenderer.sprite = skin.projectileSprite;
                    spriteRenderer.color = skin.projectileColor;
                }
                transform.localScale = new Vector3(skin.projectileScale.x, skin.projectileScale.y, 1f);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Asteroid>(out var asteroid))
            {
                asteroid.TakeHit();
                Destroy(gameObject);
            }
        }
    }
}
