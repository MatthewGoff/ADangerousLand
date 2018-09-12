using UnityEngine;
using UnityEngine.UI;

public class PlayerMonoBehaviour : MonoBehaviour, ICombatantMonoBehaviour
{
    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;
    private PlayerManager Manager;

    public void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        MoveTarget = RB2D.position;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        Manager.Update(Time.deltaTime);
        GameObject Camera = GameManager.Singleton.PlayerCamera;

        if (Input.GetMouseButton(0) && Camera != null && GameManager.Singleton.GameIsLive)
        {
            MoveTarget = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1) && Camera != null && GameManager.Singleton.GameIsLive)
        {
            Vector2 attackTarget = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            
            Manager.SlashAttack(attackTarget);
        }
    }

    public void FixedUpdate()
    {
        Vector2 movement = MoveTarget - RB2D.position;
        if (movement.magnitude > 1.0f)
        {
            movement.Normalize();
        }
        float movementMultiplier = Manager.World.MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
        RB2D.MovePosition(RB2D.position + movement * Manager.MoveSpeed * movementMultiplier * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        // We move the transform but not the rigid body. This is to that the
        // player is rendered pixel perfect but we don't mess up the physics.
        transform.position = Util.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
    }

    public Vector2 GetPlayerPosition()
    {
        if (RB2D == null)
        {
            return new Vector2(0, 0);
        }
        else
        {
            return RB2D.position;
        }
    }

    public void AssignManager(PlayerManager manager)
    {
        Manager = manager;
    }

    public CombatantManager GetCombatantManager()
    {
        return Manager;
    }

    public void Freeze()
    {
        RB2D.isKinematic = true;
        MoveTarget = RB2D.position;
    }
}
