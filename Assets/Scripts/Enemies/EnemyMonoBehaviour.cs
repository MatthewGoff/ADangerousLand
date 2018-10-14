using UnityEngine;

namespace ADL
{
    public class EnemyMonoBehaviour : MonoBehaviour, IHitboxOwner
    {
        public GameObject SpritePrefab;
        public GameObject HitboxPrefab;
        public DamageNumbersCanvasMonoBehaviour DamageNumbersCanvas;

        private GameObject Sprite;
        private GameObject Hitbox;
        private EnemyManager Manager;
        private Rigidbody2D RB2D;
        private SpriteRenderer Renderer;
        private Animator Animator;
        private HealthBarMonoBehaviour HealthBar;
        private bool BeingKnockedBack;
        private Vector2 KnockbackTarget;

        public void Start()
        {
            Sprite = Instantiate(SpritePrefab, transform.position, Quaternion.identity);
            Animator = Sprite.GetComponent<Animator>();
            Renderer = Sprite.GetComponent<SpriteRenderer>();

            Hitbox = Instantiate(HitboxPrefab, transform.position, Quaternion.identity);
            Hitbox.transform.SetParent(transform);

            RB2D = GetComponent<Rigidbody2D>();
        }


        public void AssignManager(EnemyManager manager)
        {
            Manager = manager;

            Vector2 top = (Vector2)transform.position + new Vector2(0, Configuration.ENEMY_CONFIGURATIONS[Manager.EnemyType].Height + 0.15f);
            top = Util.RoundToPixel(top, Configuration.PIXELS_PER_UNIT);
            GameObject healthBar = GameObject.Instantiate(Prefabs.HEALTH_BAR_PREFAB, top, Quaternion.identity);
            healthBar.transform.SetParent(transform);
            HealthBar = healthBar.GetComponent<HealthBarMonoBehaviour>();
            UpdateHealthBar();

            GameObject damageNumbersCanvas = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_CANVAS_PREFAB, top, Quaternion.identity);
            damageNumbersCanvas.transform.SetParent(transform);
            DamageNumbersCanvas = damageNumbersCanvas.GetComponent<DamageNumbersCanvasMonoBehaviour>();
        }

        public void Update()
        {
            Renderer.color = new Color(1, 1, 1, GameManager.Singleton.World.GetVisibilityLevel(RB2D.position));
            Sprite.transform.position = Util.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
            Renderer.sortingOrder = Util.SortingOrder(Sprite.transform.position);
        }

        public void Destroy()
        {
            Destroy(Sprite);
            Destroy(Hitbox);
            DamageNumbersCanvas.transform.SetParent(null);
            DamageNumbersCanvas.Destroy();
            HealthBar.Destroy();
            Destroy(gameObject);
        }

        public void FixedUpdate()
        {
            if (BeingKnockedBack)
            {
                Manager.FixedUpdate();
                Vector2 movementVector = KnockbackTarget - RB2D.position;
                RB2D.MovePosition(RB2D.position + movementVector * Configuration.KNOCKBACK_SPEED * Time.fixedDeltaTime);
                if (movementVector.magnitude <= 0.1f)
                {
                    BeingKnockedBack = false;
                }
            }
            else
            {
                Vector2 movementVector = Manager.FixedUpdate();
                float angle = Vector2.SignedAngle(new Vector2(1, 0), movementVector);
                if (movementVector.magnitude > 1.0f)
                {
                    movementVector.Normalize();
                }
                float movementMultiplier = GameManager.Singleton.World.MovementMultiplier(new WorldLocation(Util.RoundVector2(RB2D.position)));
                RB2D.MovePosition(RB2D.position + movementVector * Manager.MoveSpeed * movementMultiplier * Time.fixedDeltaTime);

                if (Animator != null)
                {
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
            }
        }

        public void Knockback(Vector2 knockback)
        {
            BeingKnockedBack = true;
            KnockbackTarget = RB2D.position + knockback;
        }

        public void UpdateHealthBar()
        {
            HealthBar.ShowHealth(Manager.CurrentHealth / Manager.MaxHealth);

        }

        public CombatantManager GetCombatantManager()
        {
            return Manager;
        }
    }
}