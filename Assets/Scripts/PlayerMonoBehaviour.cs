using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerMonoBehaviour : MonoBehaviour, IHitboxOwner
{
    public GameObject Hitbox;
    
    public GameObject HeadSprite;
    public GameObject BodySprite;
    public GameObject WeaponSprite;
    public GameObject ShieldSprite;

    private SpriteRenderer HeadRenderer;
    private SpriteRenderer BodyRenderer;
    private SpriteRenderer WeaponRenderer;
    private SpriteRenderer ShieldRenderer;

    private Animator HeadAnimator;
    private Animator BodyAnimator;
    private Animator WeaponAnimator;

    private Rigidbody2D RB2D;
    private Vector2 MoveTarget;
    private Vector2 DashTarget;
    private bool Dashing;
    private PlayerManager Manager;

    public void Init(PlayerManager manager)
    {
        Manager = manager;

        HeadSprite = Instantiate(HeadSprite, transform.position, Quaternion.identity);
        BodySprite = Instantiate(BodySprite, transform.position, Quaternion.identity);
        WeaponSprite = Instantiate(WeaponSprite, transform.position, Quaternion.identity);

        HeadRenderer = HeadSprite.GetComponent<SpriteRenderer>();
        BodyRenderer = BodySprite.GetComponent<SpriteRenderer>();
        WeaponRenderer = WeaponSprite.GetComponent<SpriteRenderer>();

        BodyRenderer.color = Color.HSVToRGB(Manager.Color, 1, 1);

        HeadAnimator = HeadSprite.GetComponent<Animator>();
        BodyAnimator = BodySprite.GetComponent<Animator>();
        WeaponAnimator = WeaponSprite.GetComponent<Animator>();

        if (Manager.AttackType == 0)
        {
            HeadAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Melee_Head");
            BodyAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Melee_Body");
            WeaponAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Melee_Sword");
        }
        else
        {
            HeadAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Ranged_Head");
            BodyAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Ranged_Body");
            WeaponAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Ranged_Bow");
        }

        ShieldSprite = Instantiate(ShieldSprite, Manager.GetCenter(), Quaternion.identity);
        ShieldRenderer = ShieldSprite.GetComponent<SpriteRenderer>();
        ShieldSprite.SetActive(false);

        Hitbox = Instantiate(Hitbox, transform.position, Quaternion.identity);
        Hitbox.transform.SetParent(transform);

        RB2D = GetComponent<Rigidbody2D>();
        MoveTarget = RB2D.position;
    }

    public void Destroy()
    {
        Destroy(HeadSprite);
        Destroy(BodySprite);
        Destroy(WeaponSprite);
        Destroy(Hitbox);
        Destroy(gameObject);
    }

    public void Update()
    {
        GameObject Camera = GameManager.Singleton.PlayerCamera;

        if (Input.GetKeyDown(KeyCode.Space) && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            Manager.WeaponSwap();

            if (Manager.AttackType == 0)
            {
                HeadAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Melee_Head");
                BodyAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Melee_Body");
                WeaponAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Melee_Sword");
            }
            else
            {
                HeadAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Ranged_Head");
                BodyAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Ranged_Body");
                WeaponAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/Controllers/Player/Ranged_Bow");
            }

            // It is neccesary to synch the animations after swaping animation controllers
            HeadAnimator.Play(HeadAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
            BodyAnimator.Play(BodyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
            WeaponAnimator.Play(WeaponAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
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

        if (Input.GetKeyDown(KeyCode.D) && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            DashTarget = Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Vector2 dashVector = DashTarget - RB2D.position;
            if (dashVector.magnitude >= 10f)
            {
                dashVector = dashVector.normalized * 10f;
            }
            DashTarget = RB2D.position + dashVector;
            Dashing = true;
        }

        if (Input.GetKey(KeyCode.LeftShift) && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            if (Manager.StartSprinting())
            {
                StartCoroutine("LayPlayerTrail");
            }
        }
        else
        {
            Manager.StopSprinting();
        }

        if (Input.GetKey(KeyCode.A) && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            Manager.StartBlocking();
        }
        else
        {
            Manager.StopBlocking();
        }

        if (Input.GetKeyDown(KeyCode.S) && GameManager.Singleton.GameState == GameStateType.Playing)
        {
            Manager.Stomp();
        }
        
        Vector2 position = Util.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
        HeadSprite.transform.position = position;
        BodySprite.transform.position = position;
        WeaponSprite.transform.position = position;

        int sortingOrder = Util.SortingOrder(position);
        HeadRenderer.sortingOrder = sortingOrder;
        BodyRenderer.sortingOrder = sortingOrder;
        WeaponRenderer.sortingOrder = sortingOrder;

        if (Manager.Blocking)
        {
            ShieldSprite.SetActive(true);
            ShieldSprite.transform.position = Manager.GetCenter();
            ShieldRenderer.sortingOrder = sortingOrder + 1;
        }
        else
        {
            ShieldSprite.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        if (Dashing)
        {
            Dashing = false;

            if (ValidDashTarget(DashTarget) && Manager.Dash(DashTarget))
            {
                RB2D.position = DashTarget;
                MoveTarget = RB2D.position;
            }
        }

        Manager.FixedUpdate(Time.deltaTime);

        Vector2 movementVector = MoveTarget - RB2D.position;
        if (movementVector.magnitude > 1.0f)
        {
            movementVector.Normalize();
        }
                
        float movementMultiplier = GameManager.Singleton.World.MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
        if (Manager.Sprinting)
        {
            movementMultiplier *= 2.0f;
        }

        RB2D.MovePosition(RB2D.position + movementVector * Manager.MoveSpeed * movementMultiplier * Time.fixedDeltaTime);

        HeadAnimator.SetFloat("Multiplier", movementMultiplier * Manager.MoveSpeed / 5f);
        BodyAnimator.SetFloat("Multiplier", movementMultiplier * Manager.MoveSpeed / 5f);
        WeaponAnimator.SetFloat("Multiplier", movementMultiplier * Manager.MoveSpeed / 5f);

        float movementAngle = Vector2.SignedAngle(new Vector2(1, 0), movementVector);
        if (movementVector.magnitude <= 0.05f)
        {
            TriggerAnimation("Idleing");
        }
        else if (Mathf.Abs(movementAngle) < 45)
        {
            TriggerAnimation("WalkingRight");
        }
        else if (Mathf.Abs(movementAngle) > 135)
        {
            TriggerAnimation("WalkingLeft");
        }
        else if (movementAngle > 0)
        {
            TriggerAnimation("WalkingBackwards");
        }
        else
        {
            TriggerAnimation("WalkingForwards");
        }
    }

    private bool ValidDashTarget(Vector2 dashTarget)
    {
        RaycastHit2D[] results = new RaycastHit2D[100];
        ContactFilter2D contactFilter = new ContactFilter2D
        {
            useTriggers = true
        };
        int numResults = Physics2D.Linecast(RB2D.position, DashTarget, contactFilter, results);
        for (int i = 0; i < numResults; i++)
        {
            if (results[i].collider.tag == "Terrain")
            {
                return false;
            }
        }
        return true;
    }

    public void TriggerAnimation(String trigger)
    {
        HeadAnimator.SetTrigger(trigger);
        BodyAnimator.SetTrigger(trigger);
        WeaponAnimator.SetTrigger(trigger);
    }

    public IEnumerator LayPlayerTrail()
    {
        Queue<GameObject> headSprites = new Queue<GameObject>();
        Queue<GameObject> bodySprites = new Queue<GameObject>();
        Queue<GameObject> weaponSprites = new Queue<GameObject>();

        while (Manager.Sprinting)
        {
            // Create a new set of sprites
            GameObject newHeadSprite = Instantiate(HeadSprite, transform.position, Quaternion.identity);
            GameObject newBodySprite = Instantiate(BodySprite, transform.position, Quaternion.identity);
            GameObject newWeaponSprite = Instantiate(WeaponSprite, transform.position, Quaternion.identity);

            // Synch those sprites with the player's animation
            float time = HeadSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            int state = HeadSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash;
            newHeadSprite.GetComponent<Animator>().Play(state, 0, time);
            newBodySprite.GetComponent<Animator>().Play(state, 0, time);
            newWeaponSprite.GetComponent<Animator>().Play(state, 0, time);

            // Snap those sprites to pixel grid
            Vector2 position = Util.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
            newHeadSprite.transform.position = position;
            newBodySprite.transform.position = position;
            newWeaponSprite.transform.position = position;

            // Sprite sorting order
            int sortingOrder = Util.SortingOrder(newHeadSprite.transform.position);
            newHeadSprite.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            newBodySprite.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            newWeaponSprite.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

            // Enqueue them for later update and destruction
            headSprites.Enqueue(newHeadSprite);
            bodySprites.Enqueue(newBodySprite);
            weaponSprites.Enqueue(newWeaponSprite);

            // Update the queue
            float alpha = 1f - (headSprites.Count / (float)Configuration.PLAYER_TRAIL_SPRITES);
            foreach (GameObject headSprite in headSprites)
            {
                headSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
                alpha += (1 / (float) Configuration.PLAYER_TRAIL_SPRITES);
            }
            Color color = Color.HSVToRGB(Manager.Color, 1, 1);
            alpha = 1f - (bodySprites.Count / (float) Configuration.PLAYER_TRAIL_SPRITES);
            foreach (GameObject bodySprite in bodySprites)
            {
                color.a = alpha;
                bodySprite.GetComponent<SpriteRenderer>().color = color;
                alpha += (1 / (float) Configuration.PLAYER_TRAIL_SPRITES);
            }
            alpha = 1f - (weaponSprites.Count / (float) Configuration.PLAYER_TRAIL_SPRITES);
            foreach (GameObject weaponSprite in weaponSprites)
            {
                weaponSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
                alpha += (1 / (float) Configuration.PLAYER_TRAIL_SPRITES);
            }

            headSprites.Last().GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            bodySprites.Last().GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            weaponSprites.Last().GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

            if (headSprites.Count == Configuration.PLAYER_TRAIL_SPRITES + 1)
            {
                Destroy(headSprites.Dequeue());
                Destroy(bodySprites.Dequeue());
                Destroy(weaponSprites.Dequeue());
            }

            yield return new WaitForSeconds(Configuration.PLAYER_TRAIL_PERIOD);
        }

        foreach (GameObject headSprite in headSprites)
        {
            Destroy(headSprite);
        }
        foreach (GameObject bodySprite in bodySprites)
        {
            Destroy(bodySprite);
        }
        foreach (GameObject weaponSprite in weaponSprites)
        {
            Destroy(weaponSprite);
        }
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
