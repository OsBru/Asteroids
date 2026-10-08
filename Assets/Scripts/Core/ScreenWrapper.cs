using UnityEngine;

namespace Asteroids.Core
{
    /// <summary>
    /// Fa que qualsevol objecte (nau, asteroides) reaparegui a l'altre extrem de la pantalla
    /// quan en surt pels marges (efecte pantalla infinita / toroidal clàssic d'Asteroids).
    /// </summary>
    public class ScreenWrapper : MonoBehaviour
    {
        [Tooltip("Marge extra en coordenades de món abans de fer el salt a l'altre costat")]
        [SerializeField] private float padding = 0.5f;

        private Camera mainCamera;
        private Rigidbody2D rb;

        private void Awake()
        {
            mainCamera = Camera.main;
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null) return;
            }

            Vector3 pos = transform.position;
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(pos);

            // Càlcul de límits en coordenades del món
            float vertExtent = mainCamera.orthographicSize;
            float horzExtent = vertExtent * mainCamera.aspect;

            float rightBound = horzExtent + padding;
            float leftBound = -horzExtent - padding;
            float topBound = vertExtent + padding;
            float bottomBound = -vertExtent - padding;

            bool wrapped = false;

            if (pos.x > rightBound)
            {
                pos.x = leftBound;
                wrapped = true;
            }
            else if (pos.x < leftBound)
            {
                pos.x = rightBound;
                wrapped = true;
            }

            if (pos.y > topBound)
            {
                pos.y = bottomBound;
                wrapped = true;
            }
            else if (pos.y < bottomBound)
            {
                pos.y = topBound;
                wrapped = true;
            }

            if (wrapped)
            {
                transform.position = pos;
                if (rb != null)
                {
                    rb.position = pos;
                }
            }
        }
    }
}
