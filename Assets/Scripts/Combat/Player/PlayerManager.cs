using UnityEngine;
using MessagePack;
using ADL.Core;
using ADL.World;
using ADL.Combat.Attacks;

namespace ADL.Combat.Player
{
    /// <summary>
    /// The principle class of the player. Owns and manages all of the data not related to the players visual or physical implementation in the scene
    /// </summary>
    /// <remarks>
    /// See the Manager/MonoBehaviour design pattern
    /// </remarks>
    [MessagePackObject]
    public class PlayerManager : CombatantManager
    {
        /// <summary>
        /// The position on the player relative to their bottom center from which attack originate
        /// </summary>
        [IgnoreMember] private static readonly Vector2 ATTACK_ORIGIN = new Vector2(0f, 1f);
        /// <summary>
        /// The center of this player
        /// </summary>
        [IgnoreMember] private static readonly Vector2 CENTER = new Vector2(0f, 1f);
        /// <summary>
        /// The width of the gradient from the radius of no fog to complete fog
        /// </summary>
        [IgnoreMember] private static readonly float FOG_DISSIPATION_RADIUS = 4.0f;
        /// <summary>
        /// The distance from the players ATTACK_ORIGIN that melee attacks will reach at their farthest extent
        /// </summary>
        [IgnoreMember] private static readonly float MELEE_AOE = 3.0f;

        /// <summary>
        /// Whether this player is dead
        /// </summary>
        [IgnoreMember] private bool Dead;
        /// <summary>
        /// The MonoBehaviour of this player
        /// </summary>
        [IgnoreMember] public PlayerMonoBehaviour MonoBehaviour;
        /// <summary>
        /// Whether this player is currently sprinting
        /// </summary>
        [IgnoreMember] public bool Sprinting;
        /// <summary>
        /// Whether this player is currently blocking
        /// </summary>
        [IgnoreMember] public bool Blocking;
        /// <summary>
        /// This players current health
        /// </summary>
        [IgnoreMember] public float CurrentHealth;
        /// <summary>
        /// This players current stamina
        /// </summary>
        [IgnoreMember] public float CurrentStamina;
        /// <summary>
        /// The cooldown of this players melee and ranged attacks
        /// </summary>
        [IgnoreMember] private Cooldown AttackCooldown;
        /// <summary>
        /// The cooldown of this players dash ability
        /// </summary>
        [IgnoreMember] private Cooldown DashCooldown;
        /// <summary>
        /// The cooldown of this players stomp ability
        /// </summary>
        [IgnoreMember] private Cooldown StompCooldown;

        /// <summary>
        /// This players unique identifier. Used for permenance
        /// </summary>
        [Key(0)] public int PlayerIdentifier;
        /// <summary>
        /// The name of this player
        /// </summary>
        [Key(1)] public string Name;
        /// <summary>
        /// The color of this player garb
        /// </summary>
        [Key(2)] public float Color;
        /// <summary>
        /// The death penalty for this player
        /// </summary>
        [Key(3)] public DeathPenaltyType DeathPenalty;

        /// <summary>
        /// The amount of experience this player has accumulated
        /// </summary>
        [Key(4)] public int Experience;
        /// <summary>
        /// The current level of this player
        /// </summary>
        [Key(5)] public int Level;
        /// <summary>
        /// The number of unspent passive points this player has available
        /// </summary>
        [Key(6)] public int PassivePoints;
        /// <summary>
        /// The current attack type (ranged/melee) of the player
        /// </summary>
        [Key(7)] public int AttackType;

        /// <summary>
        /// The number of passive points this player has invested in attack damage
        /// </summary>
        [Key(8)] public int AttackDamagePoints;
        /// <summary>
        /// The number of passive points this player has invested in attack speed
        /// </summary>
        [Key(9)] public int AttackSpeedPoints;
        /// <summary>
        /// The number of passive points this player has invested in movement speed
        /// </summary>
        [Key(10)] public int MoveSpeedPoints;
        /// <summary>
        /// The number of passive points this player has invested in sight radius
        /// </summary>
        [Key(11)] public int SightRadiusPoints;
        /// <summary>
        /// The number of passive points this player has invested in projectile damage
        /// </summary>
        [Key(12)] public int ProjectileDamagePoints;
        /// <summary>
        /// The number of passive points this player has invested in maximum health
        /// </summary>
        [Key(13)] public int MaxHealthPoints;
        /// <summary>
        /// The number of passive points this player has invested in maximum stamina
        /// </summary>
        [Key(14)] public int MaxStaminaPoints;
        /// <summary>
        /// The number of passive points this player has invested in health regen
        /// </summary>
        [Key(15)] public int HealthRegenPoints;
        /// <summary>
        /// The number of passive points this player has invested in stamina regen
        /// </summary>
        [Key(16)] public int StaminaRegenPoints;

        /// <summary>
        /// Create a new PlayerManager
        /// </summary>
        /// <param name="identifier">
        /// The unique identifier of this player
        /// </param>
        /// <param name="name">
        /// The name of this player
        /// </param>
        /// <param name="color">
        /// The color of this player's garb
        /// </param>
        /// <param name="deathPenalty">
        /// The death penalty for this player
        /// </param>
        public PlayerManager(int identifier, string name, float color, DeathPenaltyType deathPenalty)
        {
            PlayerIdentifier = identifier;
            Name = name;
            Color = color;
            DeathPenalty = deathPenalty;

            Experience = 0;
            Level = 1;
            PassivePoints = 0;

            AttackType = 0;
            AttackDamagePoints = 0;
            AttackSpeedPoints = 0;
            MoveSpeedPoints = 0;
            SightRadiusPoints = 0;
            ProjectileDamagePoints = 0;
            MaxHealthPoints = 0;
            MaxStaminaPoints = 0;
            HealthRegenPoints = 0;
            StaminaRegenPoints = 0;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// See the documentation of member fields
        /// </remarks>
        /// <param name="playerIdentifier"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="deathPenalty"></param>
        /// <param name="experience"></param>
        /// <param name="level"></param>
        /// <param name="passivePoints"></param>
        /// <param name="attackType"></param>
        /// <param name="attackDamagePoints"></param>
        /// <param name="attackSpeedPoints"></param>
        /// <param name="moveSpeedPoints"></param>
        /// <param name="sightRadiusPoints"></param>
        /// <param name="projectileDamagePoints"></param>
        /// <param name="maxHealthPoints"></param>
        /// <param name="maxStaminaPoints"></param>
        /// <param name="healthRegenPoints"></param>
        /// <param name="staminaRegenPoints"></param>
        [SerializationConstructor]
        public PlayerManager(
            int playerIdentifier,
            string name,
            float color,
            DeathPenaltyType deathPenalty,
            int experience,
            int level,
            int passivePoints,
            int attackType,
            int attackDamagePoints,
            int attackSpeedPoints,
            int moveSpeedPoints,
            int sightRadiusPoints,
            int projectileDamagePoints,
            int maxHealthPoints,
            int maxStaminaPoints,
            int healthRegenPoints,
            int staminaRegenPoints)
        {
            PlayerIdentifier = playerIdentifier;
            Name = name;
            Color = color;
            DeathPenalty = deathPenalty;
            Experience = experience;
            Level = level;
            PassivePoints = passivePoints;
            AttackType = attackType;
            AttackDamagePoints = attackDamagePoints;
            AttackSpeedPoints = attackSpeedPoints;
            MoveSpeedPoints = moveSpeedPoints;
            SightRadiusPoints = sightRadiusPoints;
            ProjectileDamagePoints = projectileDamagePoints;
            MaxHealthPoints = maxHealthPoints;
            MaxStaminaPoints = maxStaminaPoints;
            HealthRegenPoints = healthRegenPoints;
            StaminaRegenPoints = staminaRegenPoints;
        }

        /// <summary>
        /// Instantiate this player in the physical world
        /// </summary>
        /// <param name="spawnLocation">
        /// The initial location of the player in the world
        /// </param>
        public void Spawn(WorldLocation spawnLocation)
        {
            AttackCooldown = new Cooldown(1 / Configuration.PLAYER_ATTACK_SPEED(AttackSpeedPoints));
            DashCooldown = new Cooldown(2f);
            StompCooldown = new Cooldown(3f);
            Team = 0;
            Dead = false;
            Sprinting = false;
            Blocking = false;
            CurrentHealth = Configuration.PLAYER_MAX_HEALTH(MaxHealthPoints);
            CurrentStamina = Configuration.PLAYER_MAX_STAMINA(MaxStaminaPoints);

            GameObject player = GameObject.Instantiate(Prefabs.PLAYER_PREFAB, new Vector3(spawnLocation.X + 0.5f, spawnLocation.Y, 0), Quaternion.identity);
            MonoBehaviour = player.GetComponent<PlayerMonoBehaviour>();
            MonoBehaviour.Init(this);
        }

        /// <summary>
        /// Destroy all of this players associated GameObjects
        /// </summary>
        public void Sleep()
        {
            MonoBehaviour.Destroy();
        }

        /// <summary>
        /// Update this player
        /// </summary>
        public void FixedUpdate()
        {
            if (!Dead)
            {
                CurrentHealth += Configuration.PLAYER_HEALTH_REGEN(HealthRegenPoints) * Configuration.PLAYER_MAX_HEALTH(MaxHealthPoints) * Time.fixedDeltaTime;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Configuration.PLAYER_MAX_HEALTH(MaxHealthPoints));
                CurrentStamina += Configuration.PLAYER_STAMINA_REGEN(StaminaRegenPoints) * Time.fixedDeltaTime;
                CurrentStamina = Mathf.Clamp(CurrentStamina, 0, Configuration.PLAYER_MAX_STAMINA(MaxStaminaPoints));

                if (Sprinting)
                {
                    float staminaCost = 3f * Time.fixedDeltaTime;
                    if (CurrentStamina >= staminaCost)
                    {
                        CurrentStamina -= staminaCost;
                    }
                    else
                    {
                        StopSprinting();
                    }
                }

                if (Blocking)
                {
                    float staminaCost = 3f * Time.fixedDeltaTime;
                    if (CurrentStamina >= staminaCost)
                    {
                        CurrentStamina -= staminaCost;
                    }
                    else
                    {
                        StopBlocking();
                    }
                }
            }
        }

        /// <summary>
        /// Get this player's current position in the world
        /// </summary>
        /// <returns>
        /// The player's current position 
        /// </returns>
        public override Vector2 GetPosition()
        {
            return MonoBehaviour.transform.position;
        }

        /// <summary>
        /// Get this player's current center
        /// </summary>
        /// <returns>
        /// The player's current center
        /// </returns>
        public Vector2 GetCenter()
        {
            return (Vector2)MonoBehaviour.transform.position + CENTER;
        }

        /// <summary>
        /// Switch the attack type of this player (melee/ranged)
        /// </summary>
        public void WeaponSwap()
        {
            AttackType = (AttackType + 1) % 2;
        }

        /// <summary>
        /// Attempt to perform a slash attack
        /// </summary>
        /// <param name="attackTarget">
        /// The position towards which this player should attack
        /// </param>
        public void SlashAttack(Vector2 attackTarget)
        {
            if (CurrentStamina >= 2f && AttackCooldown.Use())
            {
                Vector2 attackPosition = (Vector2)MonoBehaviour.transform.position + ATTACK_ORIGIN;
                Vector2 attackVector = attackTarget - attackPosition;
                float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);

                CurrentStamina -= 2;
                SlashManager slash = new SlashManager(this, attackPosition, attackAngle, MELEE_AOE, Configuration.PLAYER_ATTACK_DAMAGE(AttackDamagePoints));
            }
        }

        /// <summary>
        /// Attempt to perform a bolt attack
        /// </summary>
        /// <param name="attackTarget">
        /// The position towards which this player should attack
        /// </param>
        public void BoltAttack(Vector2 attackTarget)
        {
            if (CurrentStamina >= 2f && AttackCooldown.Use())
            {
                Vector2 attackPosition = (Vector2)MonoBehaviour.transform.position + ATTACK_ORIGIN;
                Vector2 attackVector = attackTarget - attackPosition;
                float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);

                CurrentStamina -= 2;
                BoltManager slash = new BoltManager(this, attackPosition, attackAngle, Configuration.PLAYER_ATTACK_DAMAGE(AttackDamagePoints) * Configuration.PLAYER_PROJECTILE_DAMAGE(ProjectileDamagePoints));
            }
        }

        /// <summary>
        /// Attempt to perform a dash
        /// </summary>
        /// <param name="dashTarget">
        /// The position to which this player should dash
        /// </param>
        /// <returns>
        /// true if the player is able to dash (stamina and cooldown withstanding)
        /// </returns>
        public bool Dash(Vector2 dashTarget)
        {
            if (CurrentStamina >= 3 && DashCooldown.Use())
            {
                DashManager dash = new DashManager(this, MonoBehaviour.transform.position, dashTarget, Configuration.PLAYER_ATTACK_DAMAGE(AttackDamagePoints) * 0.5f);
                CurrentStamina -= 3;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to perform a stomp attack
        /// </summary>
        public void Stomp()
        {
            if (CurrentStamina >= 2f && StompCooldown.Use())
            {
                CurrentStamina -= 2;
                StompManager stomp = new StompManager(this, (Vector2)MonoBehaviour.transform.position, Configuration.PLAYER_ATTACK_DAMAGE(AttackDamagePoints));
            }
        }

        /// <summary>
        /// Attempt to start sprinting with this player
        /// </summary>
        /// <returns>
        /// true if the player will start sprinting (false if insuficuient stamina for sprinting or already sprinting)
        /// </returns>
        public bool StartSprinting()
        {
            if (!Sprinting && CurrentStamina >= 1f)
            {
                Sprinting = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// End this player's sprinting
        /// </summary>
        public void StopSprinting()
        {
            Sprinting = false;
        }

        /// <summary>
        /// Attempt to start blocking
        /// </summary>
        /// <returns>
        /// true if the player will start blocking (false if insufficient stamina for blocking or already blocking)
        /// </returns>
        public bool StartBlocking()
        {
            if (!Blocking && CurrentStamina >= 1f)
            {
                Blocking = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stop blocking
        /// </summary>
        public void StopBlocking()
        {
            Blocking = false;
        }

        /// <summary>
        /// Recieve a hit from an attack
        /// </summary>
        /// <param name="attackManager">
        /// The AttackManager of the incoming attack
        /// </param>
        /// <param name="damage">
        /// The damage of the incoming attack
        /// </param>
        /// <param name="knockback">
        /// The knockback of the incoming attack
        /// </param>
        /// <returns>
        /// The experience rewarded if this attack kills the player
        /// </returns>
        public override int RecieveHit(AttackManager attackManager, float damage, Vector2 knockback)
        {
            if (Blocking)
            {
                damage *= 0.1f;
            }
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, Configuration.PLAYER_MAX_HEALTH(MaxHealthPoints));
            if (CurrentHealth <= 0 && !Dead)
            {
                Die();
            }
            return 0;
        }

        /// <summary>
        /// Get the distance from the player over which fog starts becoming opaque
        /// </summary>
        /// <returns>
        /// The distance from the player over which fog starts becoming opaque
        /// </returns>
        public float GetSightRadiusNear()
        {
            if (Sprinting)
            {
                return Configuration.PLAYER_SIGHT_RADIUS(SightRadiusPoints) * 0.5f;
            }
            else
            {
                return Configuration.PLAYER_SIGHT_RADIUS(SightRadiusPoints);
            }
        }

        /// <summary>
        /// Get the distance from the player after which fog is completely opaque
        /// </summary>
        /// <returns>
        /// The distance from the player after which fog is completely opaque
        /// </returns>
        public float GetSightRadiusFar()
        {
            if (Sprinting)
            {
                return (Configuration.PLAYER_SIGHT_RADIUS(SightRadiusPoints) + FOG_DISSIPATION_RADIUS) * 0.5f;
            }
            else
            {
                return (Configuration.PLAYER_SIGHT_RADIUS(SightRadiusPoints) + FOG_DISSIPATION_RADIUS);
            }
        }

        /// <summary>
        /// Recieve experience
        /// </summary>
        /// <param name="exp">
        /// The amount of experience recieved
        /// </param>
        public override void RecieveExp(int exp)
        {
            Experience += exp;
            if (Experience >= Configuration.GetLevelExperience(Level + 1))
            {
                Level++;
                PassivePoints += Configuration.GetLevelSkillPoints(Level);
                GameManager.Singleton.LevelUp();
                RecieveExp(0);
            }
        }

        /// <summary>
        /// Execute the logic associated with player death
        /// </summary>
        private void Die()
        {
            Dead = true;
            CurrentHealth = 0;
            MonoBehaviour.Freeze();
            GameManager.Singleton.TakeInput(GameInputType.PlayerDeath);
        }

        /// <summary>
        /// Invest one point in the player's attack damage
        /// </summary>
        public void UpgradeAttackDamage()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                AttackDamagePoints++;
            }
        }

        /// <summary>
        /// Invest one point in the player's attack speed
        /// </summary>
        public void UpgradeAttackSpeed()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                AttackSpeedPoints++;
                AttackCooldown.Modify(1 / Configuration.PLAYER_ATTACK_SPEED(AttackSpeedPoints));
            }
        }
        /// <summary>
        /// Invest one point in the player's movement speed
        /// </summary>
        public void UpgradeMoveSpeed()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                MoveSpeedPoints++;
            }
        }
        /// <summary>
        /// Invest one point in the player's sight radius
        /// </summary>
        public void UpgradeSightRadius()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                SightRadiusPoints++;
            }
        }

        /// <summary>
        /// Invest one point in the player's projectile damage
        /// </summary>
        public void UpgradeProjectileDamage()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                ProjectileDamagePoints++;
            }
        }

        /// <summary>
        /// Invest one point in the player's maximum health
        /// </summary>
        public void UpgradeMaxHealth()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                MaxHealthPoints++;
            }
        }

        /// <summary>
        /// Invest one point in the player's maximum stamina
        /// </summary>
        public void UpgradeMaxStamina()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                MaxStaminaPoints++;
            }
        }

        /// <summary>
        /// Invest one point in the player's health regen
        /// </summary>
        public void UpgradeHealthRegen()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                HealthRegenPoints++;
            }
        }

        /// <summary>
        /// Invest one point in the player's stamina regen
        /// </summary>
        public void UpgradeStaminaRegen()
        {
            if (PassivePoints > 0)
            {
                PassivePoints--;
                StaminaRegenPoints++;
            }
        }
    }
}