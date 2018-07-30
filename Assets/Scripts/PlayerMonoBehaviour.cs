using UnityEngine;

public class PlayerMonoBehaviour : MonoBehaviour
{
    public delegate float MovementMultiplierDelegate(WorldLocation worldLocation);

    public MovementMultiplierDelegate MovementMultiplier = location => 1;
    public static float MoveSpeed = Configuration.DEFAULT_MOVE_SPEED;
    public GameObject MyCamera;
    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;
    private Cooldown SlashCooldown;

    public void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        MoveTarget = RB2D.position;
        SlashCooldown = new Cooldown(1f);
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && MyCamera != null && GameManager.Singleton.PlayerInputEnabled)
        {
            MoveTarget = MyCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1) && MyCamera != null && GameManager.Singleton.PlayerInputEnabled && SlashCooldown.Use())
        {
            Slash();   
        }
    }

    private void Slash()
    {
        Vector2 attackTarget = MyCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackVector = attackTarget - (Vector2)RB2D.transform.position;
        Quaternion attackAngle = Quaternion.Euler(0, 0, -Vector2.SignedAngle(attackVector, new Vector2(-1, 1)));
        Instantiate(Prefabs.SLASH_PREFAB, RB2D.transform.position, attackAngle);
    }

    public void FixedUpdate()
    {
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
