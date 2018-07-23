using UnityEngine;

public class PlayerMonoBehaviour : MonoBehaviour
{
    public delegate float MovementMultiplierDelegate(WorldLocation worldLocation);

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
        if (Input.GetMouseButton(0) && MyCamera != null && GameManager.Singleton.PlayerInputEnabled)
        {
            MoveTarget = MyCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
        Vector2 movement = MoveTarget - RB2D.position;
        if (movement.magnitude > 1.0f)
        {
            movement.Normalize();
        }
        float movementMultiplier = MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
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

    public Vector2 GetPlayerPosition()
    {
        return RB2D.position;
    }
}
