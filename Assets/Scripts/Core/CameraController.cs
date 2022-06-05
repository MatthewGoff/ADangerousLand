using UnityEngine;
using ADL.Util;

namespace ADL.Core
{
    /// <summary>
    /// Class which controls the position and size of a camera
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// The Camera component of the GameObject to which this MonoBehaviour is attached
        /// </summary>
        private Camera Camera;

        /// <summary>
        /// Called when the GameObject is instantiated
        /// </summary>
        void Awake()
        {
            Camera = GetComponent<Camera>();
            Camera.orthographicSize = Screen.height / (2f * Configuration.PIXELS_PER_UNIT);
        }

        /// <summary>
        /// Called after all Update() calls have occured
        /// </summary>
        public void LateUpdate()
        {
            Vector3 newPosition = GameManager.Singleton.World.PlayerManager.GetCenter();
            newPosition = Helpers.RoundToPixel(newPosition, Configuration.PIXELS_PER_UNIT);

            // Move the camera slightly so it is NOT perfectly alligned. This
            // is required to fix bug with directX relating to half pixels.
            newPosition -= new Vector3(0.01f, 0.01f, 0f);

            // Move the camera so it is "above" our two dimentional world
            newPosition.z = -1;

            transform.position = newPosition;
        }
    }
}