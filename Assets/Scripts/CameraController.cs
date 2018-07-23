using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float ZoomSpeed;
    public float MinimumZoom;
    public float DefaultZoom;
    public float MaximumZoom;

    private Camera MyCamera;
    private PlayerMonoBehaviour MyPlayerController;

    void Start()
    {
        MyCamera = GetComponent<Camera>();
        MyCamera.orthographicSize = DefaultZoom;
    }

    void Update()
    {
        Translate();
        Zoom();
    }

    void Translate()
    {
        if (MyPlayerController != null)
        {
            Vector3 newPosition = MyPlayerController.GetPlayerPosition();
            newPosition.z = -10;
            transform.position = newPosition;
        }
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f && GameManager.Singleton.PlayerInputEnabled)
        {
            float cameraSize;
            if (scroll < 0.0f)
            {
                cameraSize = MyCamera.orthographicSize * ZoomSpeed;
            }
            else
            {
                cameraSize = MyCamera.orthographicSize / ZoomSpeed;
            }
            MyCamera.orthographicSize = Mathf.Clamp(cameraSize, MinimumZoom, MaximumZoom);
        }
    }

    public void AssignPlayer(PlayerMonoBehaviour playerController)
    {
        MyPlayerController = playerController;
    }
}
