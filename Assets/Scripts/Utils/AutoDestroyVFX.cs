using UnityEngine;

namespace Asteroids.Utils
{
    /// <summary>
    /// Destrueix automàticament l'objecte quan el ParticleSystem ha acabat o després d'un temps de seguretat.
    /// Ideal per a prefabs d'explosió i efectes visuals.
    /// </summary>
    public class AutoDestroyVFX : MonoBehaviour
    {
        [SerializeField] private float fallbackLifetime = 2.0f;
        private ParticleSystem ps;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            float duration = fallbackLifetime;
            if (ps != null)
            {
                var main = ps.main;
                duration = Mathf.Max(main.duration + main.startLifetime.constantMax, 0.5f);
            }
            Destroy(gameObject, duration);
        }
    }
}
