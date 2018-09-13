using UnityEngine;
using UnityEngine.UI;

public class PlayerMonoBehaviour : MonoBehaviour, ICombatantMonoBehaviour
{
    public GameObject Sprite;

    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;
    private PlayerManager Manager;
    private Animator Animator;

    public void Start()
    {
        Sprite = GameObject.Instantiate(Sprite, transform.position, Quaternion.identity);
        Animator = Sprite.GetComponent<Animator>();
        RB2D = GetComponent<Rigidbody2D>();
        MoveTarget = RB2D.position;
    }

    public void Destroy()
    {
        Destroy(Sprite);
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

        if (Input.GetMouseButton(2) && Camera != null && GameManager.Singleton.GameIsLive)
        {
            Vector2 attackTarget = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

            Manager.BoltAttack(attackTarget);
        }
    }

    public void FixedUpdate()
    {
        Vector2 movementVector = MoveTarget - RB2D.position;
        if (movementVector.magnitude <= 0.05f)
        {
            Animator.SetTrigger("Idleing");
        }
        else
        {
            float angle = Vector2.SignedAngle(new Vector2(1, 0), movementVector);
            if (Mathf.Abs(angle) < 45)
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
            if (movementVector.magnitude > 1.0f)
            {
                movementVector.Normalize();
            }
            float movementMultiplier = Manager.World.MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
            RB2D.MovePosition(RB2D.position + movementVector * Manager.MoveSpeed * movementMultiplier * Time.fixedDeltaTime);
        }
        
    }

    private void LateUpdate()
    {
        Sprite.transform.position = Util.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
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
