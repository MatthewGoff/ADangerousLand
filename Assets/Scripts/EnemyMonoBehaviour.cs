using UnityEngine;

public class EnemyMonoBehaviour : MonoBehaviour, ICombatantMonoBehaviour
{
    private EnemyManager Manager;
    private Rigidbody2D RB2D;
    private SpriteRenderer Renderer;
    private Vector2 MoveTarget;

    public void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        Renderer = GetComponent<SpriteRenderer>();
        MoveTarget = RB2D.position;
    }

    public void Update()
    {
        MoveTarget = Manager.AI.GetMoveTarget();
        Renderer.color = new Color(1, 1, 1, Manager.World.GetVisibilityLevel(RB2D.position));
    }

    public void Destroy()
    {
        GameManager.Singleton.GameObjectCount--;
        Destroy(gameObject);
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

    public void AssignManager(EnemyManager manager)
    {
        Manager = manager;
    }

    public CombatantManager GetCombatantManager()
    {
        return Manager;
    }
}
