using UnityEngine;
using System.Collections;
using System;

public class PlayerMonoBehaviour : MonoBehaviour, ICombatantMonoBehaviour
{
    public GameObject Sprite;

    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;
    private PlayerManager Manager;
    private Animator Animator;
    private readonly int SprintFrames = 10;
    private readonly float SprintDelay = 0.01f;
    private readonly float SprintMinAlpha = 0.0f;
    private GameObject[] SprintSprites;
    private Vector2[] SprintPositions;

    public void Start()
    {
        Sprite = GameObject.Instantiate(Sprite, transform.position, Quaternion.identity);
        SprintSprites = new GameObject[SprintFrames];
        SprintPositions = new Vector2[SprintFrames];
        for (int i = 0; i < SprintFrames; i++)
        {
            SprintSprites[i] = GameObject.Instantiate(Sprite, transform.position, Quaternion.identity);
            SprintSprites[i].GetComponent<SpriteRenderer>().sortingOrder = -i-1;
            SprintSprites[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - (1 - SprintMinAlpha) * i / (float)SprintFrames);
            SprintPositions[i] = transform.position;
        }
        Animator = Sprite.GetComponent<Animator>();
        RB2D = GetComponent<Rigidbody2D>();
        MoveTarget = RB2D.position;
        StartCoroutine("LaySprintSprites");
    }

    public void Destroy()
    {
        Destroy(Sprite);
        Destroy(gameObject);
        for (int i=0; i<SprintFrames; i++)
        {
            Destroy(SprintSprites[i]);
        }
    }

    public void Update()
    {
        Manager.Update(Time.deltaTime);
        GameObject Camera = GameManager.Singleton.PlayerCamera;

        if (Input.GetKeyDown(KeyCode.Space) && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            Manager.WeaponSwap();
        }

        if (Input.GetMouseButton(0) && Camera != null && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            MoveTarget = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1) && Camera != null && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            Vector2 attackTarget = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

            if (Manager.AttackType == 0)
            {
                Manager.SlashAttack(attackTarget);
            }
            else if (Manager.AttackType == 1)
            {
                Manager.BoltAttack(attackTarget);
            }
            
        }

        Manager.AttemptingSprint = Input.GetKey(KeyCode.LeftShift) && GameManager.Singleton.GameState == GameStateType.Playing;
    }

    public void FixedUpdate()
    {
        Vector2 movementVector = MoveTarget - RB2D.position;
        if (movementVector.magnitude > 1.0f)
        {
            movementVector.Normalize();
        }
                
        float movementMultiplier = Manager.World.MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
        bool Sprinting = Manager.AttemptSprint();
        if (Sprinting)
        {
            movementMultiplier *= 3;
            EnableSprintSprites();
        }
        else
        {
            ResetSprintSprites();
            DisableSprintSprites();
        }
        RB2D.MovePosition(RB2D.position + movementVector * Manager.MoveSpeed * movementMultiplier * Time.fixedDeltaTime);

        Animator.SetFloat("Multiplier", movementMultiplier * Manager.MoveSpeed / 5f);
        float angle = Vector2.SignedAngle(new Vector2(1, 0), movementVector);
        if (movementVector.magnitude <= 0.05f)
        {
            TriggerAnimation("Idleing");
        }
        else if (Mathf.Abs(angle) < 45)
        {
            TriggerAnimation("WalkingRight");
        }
        else if (Mathf.Abs(angle) > 135)
        {
            TriggerAnimation("WalkingLeft");
        }
        else if (angle > 0)
        {
            TriggerAnimation("WalkingBackwards");
        }
        else
        {
            TriggerAnimation("WalkingForwards");
        }
    }

    private void ResetSprintSprites()
    {
        for (int i = 0; i < SprintFrames; i++)
        {
            SprintPositions[i] = transform.position;
        }
    }

    private void EnableSprintSprites()
    {
        for (int i=0; i<SprintFrames; i++)
        {
            SprintSprites[i].SetActive(true);
        }
    }

    private void DisableSprintSprites()
    {
        for (int i = 0; i < SprintFrames; i++)
        {
            SprintSprites[i].SetActive(false);
        }
    }

    public void TriggerAnimation(String trigger)
    {
        Animator.SetTrigger(trigger);
        for (int i=0; i<SprintFrames; i++)
        {
            if (SprintSprites[i].activeSelf) {
                SprintSprites[i].GetComponent<Animator>().SetTrigger(trigger);
            }
        }
    }

    private void LateUpdate()
    {
        Sprite.transform.position = Util.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
    }
    
    private IEnumerator LaySprintSprites()
    {
        while (true)
        {
            Array.Copy(SprintPositions, 0, SprintPositions, 1, SprintFrames-1);
            SprintPositions[0] = transform.position;
            for (int i=0; i< SprintFrames; i++)
            {
                SprintSprites[i].transform.position = SprintPositions[i];
            }
            yield return new WaitForSeconds(SprintDelay);
        }
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
