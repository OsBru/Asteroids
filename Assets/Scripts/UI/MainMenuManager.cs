using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Asteroids.Core;

namespace Asteroids.UI
{
    /// <summary>
    /// Gestiona la pantalla d'inici del joc.
    /// Col·loca aquest script en un GameObject "MainMenu" i assigna la config.
    /// La pantalla s'amaga automàticament en prémer Play i crida GameManager.StartNewGame().
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Configuració Visual")]
        [Tooltip("ScriptableObject amb tots els paràmetres estètics de la pantalla d'inici.")]
        [SerializeField] private MainMenuConfig config;

        [Header("Referències UI (autogenerades si es deixen buides)")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI subtitleLabel;
        [SerializeField] private Button playButton;
        [SerializeField] private TextMeshProUGUI playButtonLabel;
        [SerializeField] private TextMeshProUGUI versionLabel;

        // ──────────────────────────────────────────────────────────────
        //  Cicle de vida
        // ──────────────────────────────────────────────────────────────

        private void Awake()
        {
            // Si les referències no estan assignades manualment, les cerquem als fills
            AutoFindReferences();
            ApplyConfig();
        }

        private void Start()
        {
            // Assegurem que el joc no comença fins que el jugador premi Play
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetWaitingForMenu(true);
            }

            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }

            gameObject.SetActive(true);
        }

        // ──────────────────────────────────────────────────────────────
        //  Acció del botó
        // ──────────────────────────────────────────────────────────────

        private void OnPlayClicked()
        {
            // Amaga la pantalla d'inici
            gameObject.SetActive(false);

            // Informa el GameManager que pot començar
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetWaitingForMenu(false);
                GameManager.Instance.StartNewGame();
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  Aplicació de la config
        // ──────────────────────────────────────────────────────────────

        private void ApplyConfig()
        {
            if (config == null) return;

            // Fons
            if (backgroundImage != null)
            {
                if (config.backgroundSprite != null)
                {
                    backgroundImage.sprite = config.backgroundSprite;
                    backgroundImage.type = config.backgroundImageType;
                    backgroundImage.color = Color.white;
                }
                else
                {
                    backgroundImage.sprite = null;
                    backgroundImage.color = config.backgroundColor;
                }
            }

            // Títol
            if (titleLabel != null)
            {
                titleLabel.text = config.titleText;
                titleLabel.color = config.titleColor;
                titleLabel.fontSize = config.titleFontSize;
            }

            // Subtítol
            if (subtitleLabel != null)
            {
                bool hasSubtitle = !string.IsNullOrWhiteSpace(config.subtitleText);
                subtitleLabel.gameObject.SetActive(hasSubtitle);
                if (hasSubtitle)
                {
                    subtitleLabel.text = config.subtitleText;
                    subtitleLabel.color = config.subtitleColor;
                    subtitleLabel.fontSize = config.subtitleFontSize;
                }
            }

            // Botó Play
            if (playButton != null)
            {
                // Colors de l'estat del botó
                var colors = playButton.colors;
                colors.normalColor    = config.buttonNormalColor;
                colors.highlightedColor = config.buttonHoverColor;
                colors.pressedColor   = config.buttonPressedColor;
                colors.selectedColor  = config.buttonHoverColor;
                playButton.colors = colors;

                // Sprite opcional del botó
                if (config.buttonSprite != null)
                {
                    var img = playButton.GetComponent<Image>();
                    if (img != null)
                    {
                        img.sprite = config.buttonSprite;
                        img.type = Image.Type.Sliced;
                    }
                }
                else
                {
                    // Apliquem el color normal directament a la imatge perquè es vegi a l'editor
                    var img = playButton.GetComponent<Image>();
                    if (img != null) img.color = config.buttonNormalColor;
                }

                // Mida del botó
                var rt = playButton.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.sizeDelta = new Vector2(config.buttonWidth, config.buttonHeight);
                }
            }

            // Text del botó
            if (playButtonLabel != null)
            {
                playButtonLabel.text = config.playButtonText;
                playButtonLabel.color = config.buttonTextColor;
                playButtonLabel.fontSize = config.buttonFontSize;
            }

            // Versió
            if (versionLabel != null)
            {
                bool hasVersion = !string.IsNullOrWhiteSpace(config.versionText);
                versionLabel.gameObject.SetActive(hasVersion);
                if (hasVersion)
                {
                    versionLabel.text = config.versionText;
                    versionLabel.color = config.versionColor;
                    versionLabel.fontSize = config.versionFontSize;
                }
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  Cerca automàtica de referències als fills
        // ──────────────────────────────────────────────────────────────

        private void AutoFindReferences()
        {
            if (backgroundImage == null)
                backgroundImage = GetComponentInChildren<Image>(true);

            if (titleLabel == null)
            {
                var allTMP = GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var t in allTMP)
                {
                    if (t.gameObject.name.ToLower().Contains("title") && titleLabel == null)
                        titleLabel = t;
                    else if (t.gameObject.name.ToLower().Contains("subtitle") && subtitleLabel == null)
                        subtitleLabel = t;
                    else if (t.gameObject.name.ToLower().Contains("version") && versionLabel == null)
                        versionLabel = t;
                    else if (t.gameObject.name.ToLower().Contains("play") && playButtonLabel == null)
                        playButtonLabel = t;
                }
            }

            if (playButton == null)
                playButton = GetComponentInChildren<Button>(true);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Permet previsualitzar els canvis de config a l'editor sense entrar en Play Mode.
        /// </summary>
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                AutoFindReferences();
                ApplyConfig();
            }
        }
#endif
    }
}
