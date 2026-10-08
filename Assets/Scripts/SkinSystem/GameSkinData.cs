using UnityEngine;
using Asteroids.Asteroids;

namespace Asteroids.SkinSystem
{
    [CreateAssetMenu(fileName = "NewGameSkin", menuName = "Asteroids/Game Skin", order = 1)]
    public class GameSkinData : ScriptableObject
    {
        [Header("General Info")]
        [Tooltip("Nom del tema (ex: 'Retro Neon', 'Deep Space Sci-Fi')")]
        public string skinName = "Default Theme";
        [TextArea(2, 4)]
        public string skinDescription = "Tema clàssic per a Asteroids.";
        public Color themeAccentColor = Color.cyan;

        [Header("Player Ship")]
        public Sprite shipSprite;
        public Color shipColor = Color.white;
        public Sprite thrusterSprite;
        public GameObject thrusterParticlePrefab;
        public AudioClip shootSfx;
        public AudioClip thrustSfx;
        public GameObject shipExplosionPrefab;
        public AudioClip shipExplosionSfx;

        [Header("Projectiles / Lasers")]
        public Sprite projectileSprite;
        public Color projectileColor = Color.yellow;
        public Vector2 projectileScale = Vector2.one;

        [Header("Asteroids")]
        [Tooltip("Sprites per a asteroides grans (es tria a l'atzar si n'hi ha més d'un)")]
        public Sprite[] largeAsteroidSprites;
        [Tooltip("Sprites per a asteroides mitjans")]
        public Sprite[] mediumAsteroidSprites;
        [Tooltip("Sprites per a asteroides petits")]
        public Sprite[] smallAsteroidSprites;
        public Color asteroidTint = Color.white;
        public GameObject asteroidExplosionPrefab;
        public AudioClip asteroidExplosionSfx;
        public AudioClip asteroidHitSfx;

        [Header("Music & Atmosphere")]
        public AudioClip backgroundMusic;
        public AudioClip gameOverSfx;
        public Color backgroundColor = new Color(0.04f, 0.04f, 0.07f, 1f);
        public Sprite backgroundSprite;

        [Header("UI & HUD")]
        public Sprite lifeIconSprite;
        public Color uiTextColor = Color.white;

        // ── ANIMACIONS ────────────────────────────────────────────────────────

        [Header("Animació · Nau")]
        [Tooltip("AnimatorController per a la nau. Si s'assigna, té prioritat sobre shipSprite.")]
        public RuntimeAnimatorController shipAnimator;
        [Tooltip("Nom del paràmetre bool de l'Animator que activa l'animació de propulsió.")]
        public string thrustParamName = "IsThrusting";

        [Header("Animació · Asteroides")]
        [Tooltip("AnimatorController per a asteroides GRANS. Prioritat sobre largeAsteroidSprites.")]
        public RuntimeAnimatorController largeAsteroidAnimator;
        [Tooltip("AnimatorController per a asteroides MITJANS. Prioritat sobre mediumAsteroidSprites.")]
        public RuntimeAnimatorController mediumAsteroidAnimator;
        [Tooltip("AnimatorController per a asteroides PETITS. Prioritat sobre smallAsteroidSprites.")]
        public RuntimeAnimatorController smallAsteroidAnimator;

        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna l'AnimatorController d'asteroide corresponent al tier, o null si no n'hi ha.
        /// </summary>
        public RuntimeAnimatorController GetAsteroidAnimator(AsteroidTier tier) => tier switch
        {
            AsteroidTier.Large  => largeAsteroidAnimator,
            AsteroidTier.Medium => mediumAsteroidAnimator,
            AsteroidTier.Small  => smallAsteroidAnimator,
            _                   => null
        };

        /// <summary>
        /// Retorna un sprite d'asteroide aleatori segons la categoria indicada.
        /// </summary>
        public Sprite GetAsteroidSprite(AsteroidTier tier)
        {
            Sprite[] array = tier switch
            {
                AsteroidTier.Large  => largeAsteroidSprites,
                AsteroidTier.Medium => mediumAsteroidSprites,
                AsteroidTier.Small  => smallAsteroidSprites,
                _                   => largeAsteroidSprites
            };

            if (array == null || array.Length == 0) return null;
            return array[Random.Range(0, array.Length)];
        }
    }
}
