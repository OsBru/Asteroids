using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asteroids.Core;

namespace Asteroids.Asteroids
{
    /// <summary>
    /// Generador d'asteroides perimetrals i control de rondes/onades.
    /// </summary>
    public class AsteroidSpawner : MonoBehaviour
    {
        public static AsteroidSpawner Instance { get; private set; }

        [Header("Configuració")]
        [SerializeField] private GameObject largeAsteroidPrefab;
        [SerializeField] private int baseAsteroidCount = 4;
        [SerializeField] private float spawnDistanceOffset = 1.5f;
        [SerializeField] private float minDistanceToPlayer = 4.0f;
        [SerializeField] private float timeBetweenWaves = 2.5f;

        private Camera mainCamera;
        private readonly HashSet<Asteroid> activeAsteroids = new HashSet<Asteroid>();
        private bool isWaveTransitioning = false;

        public int ActiveAsteroidCount => activeAsteroids.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            mainCamera = Camera.main;
        }

        public void RegisterAsteroid(Asteroid asteroid)
        {
            if (asteroid != null)
            {
                activeAsteroids.Add(asteroid);
            }
        }

        public void UnregisterAsteroid(Asteroid asteroid)
        {
            if (asteroid != null)
            {
                activeAsteroids.Remove(asteroid);
            }

            // Si s'han destruït tots els asteroides i no estem en transició, iniciem la següent onada
            if (activeAsteroids.Count == 0 && !isWaveTransitioning && GameManager.Instance != null && !GameManager.Instance.IsGameOver)
            {
                StartCoroutine(NextWaveRoutine());
            }
        }

        public void ClearAllAsteroids()
        {
            StopAllCoroutines();
            isWaveTransitioning = false;

            var list = new List<Asteroid>(activeAsteroids);
            activeAsteroids.Clear();

            foreach (var ast in list)
            {
                if (ast != null && ast.gameObject != null)
                {
                    Destroy(ast.gameObject);
                }
            }
        }

        public void SpawnWave(int waveNumber)
        {
            StartCoroutine(SpawnWaveRoutine(waveNumber));
        }

        private IEnumerator NextWaveRoutine()
        {
            isWaveTransitioning = true;
            yield return new WaitForSeconds(timeBetweenWaves);

            if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
            {
                GameManager.Instance.NextWave();
            }
            isWaveTransitioning = false;
        }

        private IEnumerator SpawnWaveRoutine(int waveNumber)
        {
            isWaveTransitioning = true;
            if (mainCamera == null) mainCamera = Camera.main;

            int count = baseAsteroidCount + (waveNumber - 1);
            float speedMultiplier = 1f + (waveNumber - 1) * 0.1f;

            for (int i = 0; i < count; i++)
            {
                SpawnSingleLargeAsteroid(speedMultiplier);
                yield return new WaitForSeconds(0.15f);
            }

            isWaveTransitioning = false;
        }

        private void SpawnSingleLargeAsteroid(float speedMultiplier)
        {
            if (largeAsteroidPrefab == null || mainCamera == null) return;

            float vertExtent = mainCamera.orthographicSize;
            float horzExtent = vertExtent * mainCamera.aspect;

            Vector2 spawnPos = Vector2.zero;
            Vector3 playerPos = Vector3.zero;
            if (GameManager.Instance != null && GameManager.Instance.PlayerTransform != null)
            {
                playerPos = GameManager.Instance.PlayerTransform.position;
            }

            // Trobar una posició vàlida al voltant del perímetre allunyada del jugador
            for (int attempt = 0; attempt < 10; attempt++)
            {
                // Escollir un dels 4 marges (0: Dreta, 1: Esquerra, 2: Dalt, 3: Baix)
                int edge = Random.Range(0, 4);
                switch (edge)
                {
                    case 0: // Dreta
                        spawnPos = new Vector2(horzExtent + spawnDistanceOffset, Random.Range(-vertExtent, vertExtent));
                        break;
                    case 1: // Esquerra
                        spawnPos = new Vector2(-horzExtent - spawnDistanceOffset, Random.Range(-vertExtent, vertExtent));
                        break;
                    case 2: // Dalt
                        spawnPos = new Vector2(Random.Range(-horzExtent, horzExtent), vertExtent + spawnDistanceOffset);
                        break;
                    case 3: // Baix
                        spawnPos = new Vector2(Random.Range(-horzExtent, horzExtent), -vertExtent - spawnDistanceOffset);
                        break;
                }

                if (Vector2.Distance(spawnPos, playerPos) >= minDistanceToPlayer)
                {
                    break;
                }
            }

            GameObject obj = Instantiate(largeAsteroidPrefab, spawnPos, Quaternion.identity);
            if (obj.TryGetComponent<Asteroid>(out var asteroid))
            {
                // Direcció cap a un punt aleatori dins la pantalla
                Vector2 targetPoint = Random.insideUnitCircle * (vertExtent * 0.7f);
                Vector2 direction = (targetPoint - spawnPos).normalized;
                asteroid.InitializeMovement(direction, speedMultiplier);
            }
        }
    }
}
