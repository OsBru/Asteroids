using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Asteroids.Core;
using Asteroids.SkinSystem;

namespace Asteroids.UI
{
    /// <summary>
    /// Gestiona la interfície d'usuari (HUD, vides, Game Over i canviador de temes).
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI skinIndicatorText;

        [Header("Vides")]
        [SerializeField] private Transform livesContainer;
        [SerializeField] private GameObject lifeIconPrefab;
        private readonly List<GameObject> activeLifeIcons = new List<GameObject>();

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private Button restartButton;

        [Header("Skin Switching")]
        [SerializeField] private Button switchSkinButton;

        private void Awake()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }
            if (switchSkinButton != null)
            {
                switchSkinButton.onClick.AddListener(OnSwitchSkinClicked);
            }
        }

        private void OnEnable()
        {
            GameManager.OnScoreChanged += UpdateScore;
            GameManager.OnHighScoreChanged += UpdateHighScore;
            GameManager.OnLivesChanged += UpdateLives;
            GameManager.OnWaveChanged += UpdateWave;
            GameManager.OnGameOver += ShowGameOver;
            GameManager.OnGameRestarted += HideGameOver;
            SkinManager.OnSkinChanged += HandleSkinChanged;

            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                HandleSkinChanged(SkinManager.Instance.CurrentSkin);
            }
        }

        private void OnDisable()
        {
            GameManager.OnScoreChanged -= UpdateScore;
            GameManager.OnHighScoreChanged -= UpdateHighScore;
            GameManager.OnLivesChanged -= UpdateLives;
            GameManager.OnWaveChanged -= UpdateWave;
            GameManager.OnGameOver -= ShowGameOver;
            GameManager.OnGameRestarted -= HideGameOver;
            SkinManager.OnSkinChanged -= HandleSkinChanged;
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"SCORE: {score:N0}";
        }

        private void UpdateHighScore(int highScore)
        {
            if (highScoreText != null)
                highScoreText.text = $"BEST: {highScore:N0}";
        }

        private void UpdateWave(int wave)
        {
            if (waveText != null)
                waveText.text = $"WAVE {wave}";
        }

        private void UpdateLives(int lives)
        {
            if (livesContainer == null) return;

            // Netejar icones anteriors
            foreach (var icon in activeLifeIcons)
            {
                if (icon != null) Destroy(icon);
            }
            activeLifeIcons.Clear();

            Sprite lifeSprite = null;
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                lifeSprite = SkinManager.Instance.CurrentSkin.lifeIconSprite != null
                    ? SkinManager.Instance.CurrentSkin.lifeIconSprite
                    : SkinManager.Instance.CurrentSkin.shipSprite;
            }

            for (int i = 0; i < lives; i++)
            {
                GameObject iconObj;
                if (lifeIconPrefab != null)
                {
                    iconObj = Instantiate(lifeIconPrefab, livesContainer);
                }
                else
                {
                    iconObj = new GameObject($"Life_{i}", typeof(RectTransform), typeof(Image));
                    iconObj.transform.SetParent(livesContainer, false);
                    var rt = iconObj.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(24, 24);
                }

                if (iconObj.TryGetComponent<Image>(out var img))
                {
                    if (lifeSprite != null)
                    {
                        img.sprite = lifeSprite;
                        img.color = Color.white;
                    }
                }

                activeLifeIcons.Add(iconObj);
            }
        }

        private void HandleSkinChanged(GameSkinData skin)
        {
            if (skin == null) return;

            if (skinIndicatorText != null)
            {
                skinIndicatorText.text = $"THEME: {skin.skinName.ToUpper()} [Prem 'T']";
                skinIndicatorText.color = skin.themeAccentColor;
            }

            // Actualitzar colors de text del HUD
            if (scoreText != null) scoreText.color = skin.uiTextColor;
            if (waveText != null) waveText.color = skin.themeAccentColor;

            // Re-renderitzar icones de vides amb el nou sprite
            if (GameManager.Instance != null)
            {
                UpdateLives(GameManager.Instance.Lives);
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            if (finalScoreText != null && GameManager.Instance != null)
            {
                finalScoreText.text = $"FINAL SCORE\n{GameManager.Instance.Score:N0}";
            }
        }

        private void HideGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void OnRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        private void OnSwitchSkinClicked()
        {
            if (SkinManager.Instance != null)
            {
                SkinManager.Instance.NextSkin();
            }
        }
    }
}
