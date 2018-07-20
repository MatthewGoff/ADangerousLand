using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public delegate float MovementMultiplierDelegate((int, int) location);

    public MovementMultiplierDelegate MovementMultiplier = location => 1;
    public static float MoveSpeed = Configuration.DEFAULT_MOVE_SPEED;
    public GameObject MyCamera;
    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;

    public void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        MoveTarget = RB2D.position;
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
        float movementMultiplier = MovementMultiplier(Util.RoundVector2(RB2D.position));
        RB2D.MovePosition(RB2D.position + movement * MoveSpeed * movementMultiplier * Time.fixedDeltaTime);
    }

    public void AssignCamera(GameObject camera)
    {
        MyCamera = camera;
    }

    public void AssignMovementMultiplier(MovementMultiplierDelegate movementMultiplier)
    {
        MovementMultiplier = movementMultiplier;
    }
}
