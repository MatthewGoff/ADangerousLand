using UnityEngine;

public class EnemyMonoBehaviour : MonoBehaviour, ICombatantMonoBehaviour
{
    private GameObject Sprite;
    private EnemyManager Manager;
    private Rigidbody2D RB2D;
    private SpriteRenderer Renderer;
    private Vector2 MoveTarget;
    private Animator Animator;

    public void Start()
    {
        Sprite = (GameObject)Resources.Load("Prefabs/"+ Configuration.ENEMY_CONFIGURATIONS[Manager.EnemyType].SpriteLocation);
        Sprite = Instantiate(Sprite, transform.position, Quaternion.identity);
        Animator = Sprite.GetComponent<Animator>();
        RB2D = GetComponent<Rigidbody2D>();
        Renderer = Sprite.GetComponent<SpriteRenderer>();
        MoveTarget = RB2D.position;
    }

    public void Update()
    {
        MoveTarget = Manager.Update();
        Renderer.color = new Color(1, 1, 1, Manager.World.GetVisibilityLevel(RB2D.position));
    }

    public void Destroy()
    {
        Destroy(Sprite);
        Destroy(gameObject);
    }

    public void FixedUpdate()
    {
        Vector2 movementVector = MoveTarget - RB2D.position;
        float angle = Vector2.SignedAngle(new Vector2(1, 0), movementVector);
        if (movementVector.magnitude > 1.0f)
        {
            movementVector.Normalize();
        }
        float movementMultiplier = Manager.World.MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
        RB2D.MovePosition(RB2D.position + movementVector * Manager.MoveSpeed * movementMultiplier * Time.fixedDeltaTime);

        Animator.SetFloat("Multiplier", movementMultiplier * Manager.MoveSpeed / 5f);
        if (movementVector.magnitude <= 0.05f)
        {
            Animator.SetTrigger("Idleing");
        }
        else if (Mathf.Abs(angle) < 45)
        {
            Animator.SetTrigger("WalkingRight");
        }
        else if (Mathf.Abs(angle) > 135)
        {
            Animator.SetTrigger("WalkingLeft");
        }
        else if (angle > 0)
        {
            Animator.SetTrigger("WalkingBackwards");
        }
        else
        {
            Animator.SetTrigger("WalkingForwards");
        }
    }

    private void LateUpdate()
    {
        Sprite.transform.position = Util.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
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
