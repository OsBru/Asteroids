using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Asteroids.SkinSystem
{
    /// <summary>
    /// Gestor central de skins/temes. Permet canviar el tema del joc en qualsevol moment
    /// i notifica a tots els elements visuals i sonors perquè s'actualitzin immediatament.
    /// </summary>
    public class SkinManager : MonoBehaviour
    {
        public static SkinManager Instance { get; private set; }

        [Header("Skins Disponibles")]
        [SerializeField] private GameSkinData[] availableSkins;
        [SerializeField] private int defaultSkinIndex = 0;

        [Header("Controls en Temps Real")]
        [Tooltip("Tecla per canviar de tema durant la partida (per defecte: T)")]
        [SerializeField] private KeyCode switchSkinKey = KeyCode.T;

        public GameSkinData CurrentSkin { get; private set; }
        public int CurrentSkinIndex { get; private set; }

        public static event Action<GameSkinData> OnSkinChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitSkin();
        }

        private void InitSkin()
        {
            if (availableSkins != null && availableSkins.Length > 0)
            {
                int index = Mathf.Clamp(defaultSkinIndex, 0, availableSkins.Length - 1);
                SetSkin(index, false);
            }
        }

        private void Start()
        {
            // Disparem l'esdeveniment a l'inici per assegurar que tots els components tenen el tema correcte
            if (CurrentSkin != null)
            {
                OnSkinChanged?.Invoke(CurrentSkin);
            }
        }

        private void Update()
        {
            // Permetre alternar el tema amb la tecla 'T'
            bool switchRequested = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            {
                switchRequested = true;
            }
#else
            if (Input.GetKeyDown(switchSkinKey))
            {
                switchRequested = true;
            }
#endif

            if (switchRequested)
            {
                NextSkin();
            }
        }

        public void NextSkin()
        {
            if (availableSkins == null || availableSkins.Length <= 1) return;
            int nextIndex = (CurrentSkinIndex + 1) % availableSkins.Length;
            SetSkin(nextIndex);
        }

        public void SetSkin(int index, bool notify = true)
        {
            if (availableSkins == null || index < 0 || index >= availableSkins.Length) return;
            CurrentSkinIndex = index;
            CurrentSkin = availableSkins[index];

            if (notify)
            {
                OnSkinChanged?.Invoke(CurrentSkin);
            }
        }

        public void SetSkin(GameSkinData skin)
        {
            if (skin == null) return;
            CurrentSkin = skin;
            if (availableSkins != null)
            {
                for (int i = 0; i < availableSkins.Length; i++)
                {
                    if (availableSkins[i] == skin)
                    {
                        CurrentSkinIndex = i;
                        break;
                    }
                }
            }
            OnSkinChanged?.Invoke(CurrentSkin);
        }

        public GameSkinData[] GetAvailableSkins() => availableSkins;
    }
}
