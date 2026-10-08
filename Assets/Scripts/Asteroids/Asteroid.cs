using UnityEngine;
using Asteroids.SkinSystem;
using Asteroids.Audio;
using Asteroids.Core;

namespace Asteroids.Asteroids
{
    /// <summary>
    /// Representa un asteroide. S'encarrega del seu moviment, fragmentació en mides inferiors
    /// i de la invocació dels efectes visuals i sonors del tema actiu.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Asteroid : MonoBehaviour
    {
        [Header("Classificació")]
        [SerializeField] private AsteroidTier tier = AsteroidTier.Large;
        [SerializeField] private int scoreValue = 20;

        [Header("Fragmentació")]
        [SerializeField] private GameObject smallerAsteroidPrefab;
        [SerializeField] private int fragmentCount = 2;

        [Header("Velocitat i Gir")]
        [SerializeField] private float minSpeed = 1.2f;
        [SerializeField] private float maxSpeed = 3.5f;
        [SerializeField] private float maxSpinSpeed = 90f;

        private Rigidbody2D rb;

        public AsteroidTier Tier => tier;
        public int ScoreValue => scoreValue;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
        }

        private void Start()
        {
            // Registre a l'AsteroidSpawner
            if (AsteroidSpawner.Instance != null)
            {
                AsteroidSpawner.Instance.RegisterAsteroid(this);
            }
        }

        /// <summary>
        /// Inicialitza el moviment i rotació aleatòries de l'asteroide.
        /// </summary>
        public void InitializeMovement(Vector2 direction, float speedMultiplier = 1f)
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            float speed = Random.Range(minSpeed, maxSpeed) * speedMultiplier;
            rb.linearVelocity = direction.normalized * speed;
            rb.angularVelocity = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        }

        public void TakeHit()
        {
            // Efectes del tema actiu (explosió de partícules i so)
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                var skin = SkinManager.Instance.CurrentSkin;
                if (skin.asteroidExplosionPrefab != null)
                {
                    Instantiate(skin.asteroidExplosionPrefab, transform.position, Quaternion.identity);
                }

                if (AudioManager.Instance != null)
                {
                    AudioClip sfx = skin.asteroidExplosionSfx != null ? skin.asteroidExplosionSfx : skin.asteroidHitSfx;
                    AudioManager.Instance.PlaySfx(sfx);
                }
            }

            // Puntuació
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }

            // Fragmentació en asteroides més petits
            if (smallerAsteroidPrefab != null && fragmentCount > 0)
            {
                for (int i = 0; i < fragmentCount; i++)
                {
                    Vector2 randomOffset = Random.insideUnitCircle * 0.25f;
                    Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                    GameObject childObj = Instantiate(smallerAsteroidPrefab, spawnPos, Quaternion.identity);

                    if (childObj.TryGetComponent<Asteroid>(out var childAsteroid))
                    {
                        // Direcció divergent
                        Vector2 baseDir = rb != null ? rb.linearVelocity.normalized : Vector2.up;
                        float angleOffset = (i == 0) ? 45f : -45f;
                        angleOffset += Random.Range(-15f, 15f);
                        Vector2 fragmentDir = Quaternion.Euler(0, 0, angleOffset) * baseDir;

                        childAsteroid.InitializeMovement(fragmentDir, 1.25f);
                    }
                }
            }

            // Notificar destrucció
            if (AsteroidSpawner.Instance != null)
            {
                AsteroidSpawner.Instance.UnregisterAsteroid(this);
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (AsteroidSpawner.Instance != null)
            {
                AsteroidSpawner.Instance.UnregisterAsteroid(this);
            }
        }
    }
}
