using System;
using System.Collections;
using UnityEngine;
using Asteroids.Player;
using Asteroids.Asteroids;
using Asteroids.Audio;
using Asteroids.SkinSystem;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Asteroids.Core
{
    /// <summary>
    /// Control central del cicle de joc, puntuació, rècord, vides i transicions d'onades.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private const string HIGH_SCORE_KEY = "Asteroids_HighScore";

        [Header("Estat de la Partida")]
        [SerializeField] private int initialLives = 3;
        [SerializeField] private float respawnDelay = 2.0f;

        [Header("Referències")]
        [SerializeField] private PlayerController player;
        [SerializeField] private Transform respawnPoint;

        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public int Lives { get; private set; }
        public int Wave { get; private set; } = 1;
        public bool IsGameOver { get; private set; } = false;

        /// <summary>
        /// Cert mentre la pantalla d'inici és activa. En aquest estat, StartNewGame no fa res.
        /// </summary>
        public bool IsWaitingForMenu { get; private set; } = false;

        public Transform PlayerTransform => player != null ? player.transform : null;

        public static event Action<int> OnScoreChanged;
        public static event Action<int> OnHighScoreChanged;
        public static event Action<int> OnLivesChanged;
        public static event Action<int> OnWaveChanged;
        public static event Action OnGameOver;
        public static event Action OnGameRestarted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            LoadHighScore();
        }

        private void Start()
        {
            // Si hi ha un MainMenu actiu a l'escena, ell mateix cridarà StartNewGame()
            // a través de SetWaitingForMenu(false). Si no n'hi ha, arranquem directament.
            if (!IsWaitingForMenu)
            {
                StartNewGame();
            }
        }

        /// <summary>
        /// Crida des de MainMenuManager per bloquejar o alliberar l'inici de la partida.
        /// </summary>
        public void SetWaitingForMenu(bool waiting)
        {
            IsWaitingForMenu = waiting;
        }

        private void Update()
        {
            // Reiniciar amb tecla R si la partida s'ha acabat
            if (IsGameOver)
            {
                bool restartRequested = false;

#if ENABLE_INPUT_SYSTEM
                if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                {
                    restartRequested = true;
                }
#else
                if (Input.GetKeyDown(KeyCode.R))
                {
                    restartRequested = true;
                }
#endif

                if (restartRequested)
                {
                    RestartGame();
                }
            }
        }

        private void LoadHighScore()
        {
            HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            OnHighScoreChanged?.Invoke(HighScore);
        }

        public void StartNewGame()
        {
            if (IsWaitingForMenu) return;

            Score = 0;
            Lives = initialLives;
            Wave = 1;
            IsGameOver = false;

            OnScoreChanged?.Invoke(Score);
            OnLivesChanged?.Invoke(Lives);
            OnWaveChanged?.Invoke(Wave);
            OnHighScoreChanged?.Invoke(HighScore);
            OnGameRestarted?.Invoke();

            if (AsteroidSpawner.Instance != null)
            {
                AsteroidSpawner.Instance.ClearAllAsteroids();
                AsteroidSpawner.Instance.SpawnWave(Wave);
            }

            Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : Vector3.zero;
            if (player != null)
            {
                player.Respawn(spawnPos);
            }
        }

        public void AddScore(int points)
        {
            if (IsGameOver) return;

            Score += points;
            OnScoreChanged?.Invoke(Score);

            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
                PlayerPrefs.Save();
                OnHighScoreChanged?.Invoke(HighScore);
            }
        }

        public void OnPlayerDied()
        {
            if (IsGameOver) return;

            Lives--;
            OnLivesChanged?.Invoke(Lives);

            if (Lives > 0)
            {
                StartCoroutine(RespawnRoutine());
            }
            else
            {
                TriggerGameOver();
            }
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            if (!IsGameOver && player != null)
            {
                Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : Vector3.zero;
                player.Respawn(spawnPos);
            }
        }

        private void TriggerGameOver()
        {
            IsGameOver = true;
            OnGameOver?.Invoke();

            if (AudioManager.Instance != null && SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                var skin = SkinManager.Instance.CurrentSkin;
                if (skin.gameOverSfx != null)
                {
                    AudioManager.Instance.PlaySfx(skin.gameOverSfx);
                }
            }
        }

        public void NextWave()
        {
            if (IsGameOver) return;

            Wave++;
            OnWaveChanged?.Invoke(Wave);

            if (AsteroidSpawner.Instance != null)
            {
                AsteroidSpawner.Instance.SpawnWave(Wave);
            }
        }

        public void RestartGame()
        {
            StartNewGame();
        }
    }
}
