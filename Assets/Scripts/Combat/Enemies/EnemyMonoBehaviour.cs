using UnityEngine;
using ADL.UI;
using ADL.Util;
using ADL.Core;
using ADL.World;

namespace ADL.Combat.Enemies
{
    /// <summary>
    /// The MonoBehaviour which manages all of an enemie's visual and physical interfaces
    /// </summary>
    /// <remarks>
    /// See the Manager/MonoBehaviour design pattern
    /// </remarks>
    public class EnemyMonoBehaviour : MonoBehaviour, IHitboxOwner
    {
        /// <summary>
        /// The prefab of this enemy's sprite
        /// </summary>
        public GameObject SpritePrefab;
        /// <summary>
        /// The prefab of this enemy's hitbox
        /// </summary>
        public GameObject HitboxPrefab;
        /// <summary>
        /// The canvas that will display scrolling damage numbers over this enemy
        /// </summary>
        public DamageNumbersCanvasMonoBehaviour DamageNumbersCanvas;

        /// <summary>
        /// The sprite for thie enemy
        /// </summary>
        private GameObject Sprite;
        /// <summary>
        /// The hitbox of this enemy
        /// </summary>
        private GameObject Hitbox;
        /// <summary>
        /// The manager of this enemy
        /// </summary>
        private EnemyManager Manager;
        /// <summary>
        /// The Rigidbody of this enemy
        /// </summary>
        private Rigidbody2D RB2D;
        /// <summary>
        /// The renderer of this enemy's sprite
        /// </summary>
        private SpriteRenderer Renderer;
        /// <summary>
        /// The animatorof this enemy's sprite
        /// </summary>
        private Animator Animator;
        /// <summary>
        /// The MonoBehaviour of this enemy's healthbar
        /// </summary>
        private HealthBarMonoBehaviour HealthBar;
        /// <summary>
        /// Whether this enemy is currently being knocked back
        /// </summary>
        private bool BeingKnockedBack;
        /// <summary>
        /// The position towards which this enemy is being knocked back
        /// </summary>
        private Vector2 KnockbackTarget;

        /// <summary>
        /// Gets called when the enemy is instantiated
        /// </summary>
        public void Awake()
        {
            Sprite = Instantiate(SpritePrefab, transform.position, Quaternion.identity);
            Animator = Sprite.GetComponent<Animator>();
            Renderer = Sprite.GetComponent<SpriteRenderer>();

            Hitbox = Instantiate(HitboxPrefab, transform.position, Quaternion.identity);
            Hitbox.transform.SetParent(transform);

            RB2D = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Assign this MonoBehaviour a manager
        /// </summary>
        /// <param name="manager">
        /// The manager of this MonoBehaviour
        /// </param>
        public void AssignManager(EnemyManager manager)
        {
            Manager = manager;

            Vector2 top = (Vector2)transform.position + new Vector2(0, Configuration.ENEMY_CONFIGURATIONS[Manager.EnemyType].Height + 0.15f);
            top = Helpers.RoundToPixel(top, Configuration.PIXELS_PER_UNIT);
            GameObject healthBar = GameObject.Instantiate(Prefabs.HEALTH_BAR_PREFAB, top, Quaternion.identity);
            healthBar.transform.SetParent(transform);
            HealthBar = healthBar.GetComponent<HealthBarMonoBehaviour>();
            UpdateHealthBar();

            GameObject damageNumbersCanvas = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_CANVAS_PREFAB, top, Quaternion.identity);
            damageNumbersCanvas.transform.SetParent(transform);
            DamageNumbersCanvas = damageNumbersCanvas.GetComponent<DamageNumbersCanvasMonoBehaviour>();
        }

        /// <summary>
        /// Gets called once per frame
        /// </summary>
        public void Update()
        {
            Renderer.color = new Color(1, 1, 1, GameManager.Singleton.World.GetVisibilityLevel(RB2D.position));
            Sprite.transform.position = Helpers.RoundToPixel(transform.position, Configuration.PIXELS_PER_UNIT);
            Renderer.sortingOrder = Helpers.SortingOrder(Sprite.transform.position);
        }

        /// <summary>
        /// Completely destroy this monobehaviour
        /// </summary>
        public void Destroy()
        {
            Destroy(Sprite);
            Destroy(Hitbox);
            DamageNumbersCanvas.transform.SetParent(null);
            DamageNumbersCanvas.Destroy();
            HealthBar.Destroy();
            Destroy(gameObject);
        }

        /// <summary>
        /// Gets called once per physics update
        /// </summary>
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
                float movementMultiplier = GameManager.Singleton.World.MovementMultiplier(new WorldLocation(Helpers.RoundVector2(RB2D.position)));
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

        /// <summary>
        /// Knockback this enemy
        /// </summary>
        /// <param name="knockback">
        /// The direction and magnitude in which this enemy is being knockedback
        /// </param>
        public void Knockback(Vector2 knockback)
        {
            BeingKnockedBack = true;
            KnockbackTarget = RB2D.position + knockback;
        }

        /// <summary>
        /// Update this enemies health bar sprite to show current health percentages
        /// </summary>
        public void UpdateHealthBar()
        {
            HealthBar.ShowHealth(Manager.CurrentHealth / Manager.MaxHealth);

        }

        /// <summary>
        /// Get the CombatantManager of this enemy
        /// </summary>
        /// <returns>
        /// The CombatantManager of this enemy
        /// </returns>
        public CombatantManager GetCombatantManager()
        {
            return Manager;
        }
    }
}