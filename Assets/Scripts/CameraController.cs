using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera Camera;
    private PlayerMonoBehaviour MyPlayerController;

    void Start()
    {
        Camera = GetComponent<Camera>();
        Camera.orthographicSize = Screen.height / (2f * Configuration.PIXELS_PER_UNIT);
    }

    public void LateUpdate()
    {
        if (MyPlayerController != null)
        {
            Vector3 newPosition = MyPlayerController.GetPlayerPosition();
            newPosition = Util.RoundToPixel(newPosition, Configuration.PIXELS_PER_UNIT);

            // Move the camera slightly so it is NOT perfectly alligned. This
            // is required to fix bug with directX relating to half pixels.
            newPosition -= new Vector3(0.01f, 0.01f, 0f);

            // Move the camera so it is "above" our two dimentional world
            newPosition.z = -1;

            transform.position = newPosition;
        }
    }

    public void AssignPlayer(PlayerMonoBehaviour playerController)
    {
        MyPlayerController = playerController;
    }
}
