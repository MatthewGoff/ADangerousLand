using UnityEngine;

public class PlayerMonoBehaviour : MonoBehaviour, ICombatantMonoBehaviour
{
    public GameObject MyCamera;

    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;
    private PlayerManager Manager;

    public void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        MoveTarget = RB2D.position;
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && MyCamera != null && GameManager.Singleton.PlayerInputEnabled)
        {
            MoveTarget = MyCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1) && MyCamera != null && GameManager.Singleton.PlayerInputEnabled)
        {
            Vector2 attackTarget = MyCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Vector2 attackVector = attackTarget - (Vector2)RB2D.transform.position;
            Quaternion attackAngle = Quaternion.Euler(0, 0, -Vector2.SignedAngle(attackVector, new Vector2(-1, 1)));
            Manager.SlashAttack(RB2D.transform.position, attackAngle);
        }
    }

    public void FixedUpdate()
    {
        Vector2 movement = MoveTarget - RB2D.position;
        if (movement.magnitude > 1.0f)
        {
            movement.Normalize();
        }
        float movementMultiplier = Manager.MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
        RB2D.MovePosition(RB2D.position + movement * Manager.MoveSpeed * movementMultiplier * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        // We move the transform but not the rigid body. This is to that the
        // player is rendered pixel perfect but we don't mess up the physics.
        transform.position = Util.RoundToPixel(transform.position, MyCamera.GetComponent<CameraController>().PixelsPerUnit);
    }

    public void AssignCamera(GameObject camera)
    {
        MyCamera = camera;
    }

    public Vector2 GetPlayerPosition()
    {
        return RB2D.position;
    }

    public void AssignManager(PlayerManager manager)
    {
        Manager = manager;
    }

    public CombatantManager GetCombatantManager()
    {
        return Manager;
    }
}
