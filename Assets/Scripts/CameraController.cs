using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float ZoomSpeed;
    public float MinimumZoom;
    public float DefaultZoom;
    public float MaximumZoom;

    private Camera MyCamera;
    private GameObject Player;

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
        if (Player != null)
        {
            Vector3 newPosition = Player.transform.position;
            newPosition.z = -10;
            transform.position = newPosition;
        }
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
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

    public void AssignPlayer(GameObject player_param)
    {
        Player = player_param;
    }
}
