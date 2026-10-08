using UnityEngine;
using Asteroids.SkinSystem;

namespace Asteroids.Audio
{
    /// <summary>
    /// Sistema d'àudio desacoblat que adapta la música i els efectes de so al tema actiu.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource thrustSource;

        [Header("Volums")]
        [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.8f;
        [Range(0f, 1f)] [SerializeField] private float thrustVolume = 0.4f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetupAudioSources();
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                var obj = new GameObject("MusicSource");
                obj.transform.SetParent(transform);
                musicSource = obj.AddComponent<AudioSource>();
            }
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.playOnAwake = false;

            if (sfxSource == null)
            {
                var obj = new GameObject("SFXSource");
                obj.transform.SetParent(transform);
                sfxSource = obj.AddComponent<AudioSource>();
            }
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
            sfxSource.playOnAwake = false;

            if (thrustSource == null)
            {
                var obj = new GameObject("ThrustSource");
                obj.transform.SetParent(transform);
                thrustSource = obj.AddComponent<AudioSource>();
            }
            thrustSource.loop = true;
            thrustSource.volume = thrustVolume;
            thrustSource.playOnAwake = false;
        }

        private void OnEnable()
        {
            SkinManager.OnSkinChanged += HandleSkinChanged;
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                HandleSkinChanged(SkinManager.Instance.CurrentSkin);
            }
        }

        private void OnDisable()
        {
            SkinManager.OnSkinChanged -= HandleSkinChanged;
        }

        private void HandleSkinChanged(GameSkinData skin)
        {
            if (skin == null) return;

            // Actualitzar i reproduir música del nou tema
            if (skin.backgroundMusic != null)
            {
                if (musicSource.clip != skin.backgroundMusic || !musicSource.isPlaying)
                {
                    musicSource.clip = skin.backgroundMusic;
                    musicSource.Play();
                }
            }
            else
            {
                musicSource.Stop();
            }

            // Actualitzar so del motor
            if (thrustSource != null && skin.thrustSfx != null)
            {
                thrustSource.clip = skin.thrustSfx;
            }
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void SetThrustLoopActive(bool active)
        {
            if (thrustSource == null || thrustSource.clip == null) return;

            if (active)
            {
                if (!thrustSource.isPlaying)
                {
                    thrustSource.Play();
                }
            }
            else
            {
                if (thrustSource.isPlaying)
                {
                    thrustSource.Stop();
                }
            }
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }
    }
}
