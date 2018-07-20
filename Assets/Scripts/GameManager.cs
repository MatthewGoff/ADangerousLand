using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void Signal();
    private readonly float startDelay = 2f;
    public GameObject LoadScreen;
    
    public static bool MaxMode = false;
    public GameObject PlayerCamera;
    public GameObject MyWorld;

    private World MyWorldScript;

    void Awake()
    {
        Prefabs.LoadPrefabs();
        MyWorld = Instantiate(MyWorld, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        MyWorldScript = MyWorld.GetComponent<World>();
        MyWorldScript.DoneLoading = Verhogen;
        PlayerCamera = Instantiate(Prefabs.CAMERA_PREFAB, new Vector3(0, 0, -10), Quaternion.identity) as GameObject;
        InitGame();
        void Verhogen()
        {
            LoadScreen.SetActive(false);
        }
    }

    public void InitGame()
    {
        MyWorldScript.DestoryWorld();
        MyWorldScript.CreateWorld();
        PlayerCamera.GetComponent<CameraController>().AssignPlayer(MyWorldScript.Player);
        MyWorldScript.Player.GetComponent<PlayerController>().AssignCamera(PlayerCamera);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            //InitGame();
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            MaxMode = !MaxMode;
            if (MaxMode)
            {
                FogController.OUTER_RADIUS = 54;
                FogController.INNER_RADIUS = 45;
                PlayerController.MoveSpeed = 50;
            }
            else
            {
                FogController.OUTER_RADIUS = 11;
                FogController.INNER_RADIUS = 7;
                PlayerController.MoveSpeed = 5;
            }
        }
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
