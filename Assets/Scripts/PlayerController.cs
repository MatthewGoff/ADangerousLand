using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static float MoveSpeed = 5;
    public GameObject MyCamera;
    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;

    public void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && MyCamera != null)
        {
            MoveTarget = MyCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
        Vector2 movement = MoveTarget - RB2D.position;
        if (movement.magnitude > 1.0f)
        {
            movement.Normalize();
        }
        RB2D.MovePosition(RB2D.position + movement * MoveSpeed * Time.fixedDeltaTime);
    }

    public void AssignCamera(GameObject camera)
    {
        MyCamera = camera;
    }
}
