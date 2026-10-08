using UnityEngine;

namespace Asteroids.UI
{
    /// <summary>
    /// ScriptableObject per configurar visualment la pantalla d'inici.
    /// Crea'n un a través de Assets > Create > Asteroids > Main Menu Config.
    /// </summary>
    [CreateAssetMenu(fileName = "MainMenuConfig", menuName = "Asteroids/Main Menu Config", order = 2)]
    public class MainMenuConfig : ScriptableObject
    {
        [Header("Títol del joc")]
        [Tooltip("Text que apareix com a títol principal.")]
        public string titleText = "ASTEROIDS";
        public Color titleColor = Color.white;
        [Tooltip("Mida del títol en punts.")]
        public float titleFontSize = 96f;
        [Tooltip("Separació vertical entre el títol i el botó (píxels).")]
        public float titleBottomMargin = 40f;

        [Header("Subtítol / Eslògan")]
        [Tooltip("Deixa-ho buit per amagar-lo.")]
        public string subtitleText = "SURVIVE THE VOID";
        public Color subtitleColor = new Color(0.7f, 0.7f, 1f, 1f);
        public float subtitleFontSize = 28f;

        [Header("Fons")]
        [Tooltip("Sprite de fons. Si és null, s'usa el color sòlid.")]
        public Sprite backgroundSprite;
        public Color backgroundColor = new Color(0.04f, 0.04f, 0.07f, 1f);
        [Tooltip("Modo d'ajust de la imatge de fons.")]
        public UnityEngine.UI.Image.Type backgroundImageType = UnityEngine.UI.Image.Type.Sliced;

        [Header("Botó Play")]
        public string playButtonText = "JUGAR";
        public Color buttonNormalColor = new Color(0.1f, 0.8f, 1f, 1f);
        public Color buttonHoverColor = new Color(0.2f, 1f, 1f, 1f);
        public Color buttonPressedColor = new Color(0.0f, 0.5f, 0.7f, 1f);
        public Color buttonTextColor = new Color(0.04f, 0.04f, 0.07f, 1f);
        public float buttonFontSize = 36f;
        [Tooltip("Amplada del botó en píxels.")]
        public float buttonWidth = 280f;
        [Tooltip("Alçada del botó en píxels.")]
        public float buttonHeight = 80f;
        [Tooltip("Radi de les cantonades arrodonides del botó (0 = quadrat).")]
        [Range(0f, 60f)]
        public float buttonCornerRadius = 12f;
        [Tooltip("Sprite personalitzat per al botó. Si és null, s'usa color sòlid.")]
        public Sprite buttonSprite;

        [Header("Versió / Crèdits")]
        [Tooltip("Deixa-ho buit per amagar-ho.")]
        public string versionText = "v1.0";
        public Color versionColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        public float versionFontSize = 18f;
    }
}
