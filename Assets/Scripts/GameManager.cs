using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerCamera;

    private UnityEngine.XR.WSA.WorldManager MyWorldManager;

    void Awake()
    {
        MyWorldManager = GetComponent<UnityEngine.XR.WSA.WorldManager>();
        PlayerCamera = Instantiate(PlayerCamera, new Vector3(0, 0, -10), Quaternion.identity) as GameObject;
    }

    public void InitGame()
    {
        MyWorldManager.DestoryWorld();
        MyWorldManager.CreateWorld();
        PlayerCamera.GetComponent<CameraController>().AssignPlayer(MyWorldManager.GetPlayer());
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            InitGame();
        }
    }
}
