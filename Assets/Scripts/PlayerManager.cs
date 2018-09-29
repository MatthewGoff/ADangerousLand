using UnityEngine;
using MessagePack;

[MessagePackObject]
public class PlayerManager : CombatantManager
{
    [IgnoreMember] private static readonly Vector2 ATTACK_ORIGIN = new Vector2(0f, 1f);
    [IgnoreMember] private static readonly Vector2 CENTER = new Vector2(0f, 1f);
    [IgnoreMember] private static readonly float FOG_DISSIPATION_RADIUS = 4.0f;
    [IgnoreMember] private static readonly float MELEE_AOE = 3.0f;

    [IgnoreMember] private bool Dead;
    [IgnoreMember] public PlayerMonoBehaviour MonoBehaviour;
    [IgnoreMember] public bool Sprinting;
    [IgnoreMember] public bool Blocking;
    [IgnoreMember] public float CurrentHealth;
    [IgnoreMember] public float CurrentStamina;
    [IgnoreMember] private Cooldown AttackCooldown;
    [IgnoreMember] private Cooldown DashCooldown;
    [IgnoreMember] private Cooldown StompCooldown;

    [Key(0)] public int PlayerIdentifier;
    [Key(1)] public string Name;
    [Key(2)] public float Color;
    [Key(3)] public DeathPenaltyType DeathPenalty;

    [Key(4)] public int Experience;
    [Key(5)] public int Level;
    [Key(6)] public int PassivePoints;
    [Key(7)] public int AttackType;

    [Key(8)] public int AttackDamagePoints;
    [Key(9)] public int AttackSpeedPoints;
    [Key(10)] public int MoveSpeedPoints;
    [Key(11)] public int SightRadiusPoints;
    [Key(12)] public int ProjectileDamagePoints;
    [Key(13)] public int MaxHealthPoints;
    [Key(14)] public int MaxStaminaPoints;
    [Key(15)] public int HealthRegenPoints;
    [Key(16)] public int StaminaRegenPoints;

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

    public void Sleep()
    {
        MonoBehaviour.Destroy();
    }

    public void FixedUpdate(float deltaTime)
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

    public override Vector2 GetPosition()
    {
        return MonoBehaviour.transform.position;
    }
    
    public Vector2 GetCenter()
    {
        return (Vector2)MonoBehaviour.transform.position + CENTER;
    }

    public void WeaponSwap()
    {
        AttackType = (AttackType + 1) % 2;
    }

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

    public void Stomp()
    {
        if (CurrentStamina >= 2f && StompCooldown.Use())
        {
            CurrentStamina -= 2;
            StompManager stomp = new StompManager(this, (Vector2)MonoBehaviour.transform.position, Configuration.PLAYER_ATTACK_DAMAGE(AttackDamagePoints));
        }
    }

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

    public void StopSprinting()
    {
        Sprinting = false;
    }

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

    public void StopBlocking()
    {
        Blocking = false;
    }

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

    public override void RecieveExp(int exp)
    {
        Experience += exp;
        if (Experience >= Configuration.GetLevelExperience(Level+1))
        {
            Level++;
            PassivePoints += Configuration.GetLevelSkillPoints(Level);
            GameManager.Singleton.LevelUp();
            RecieveExp(0);
        }
    }

    private void Die()
    {
        Dead = true;
        CurrentHealth = 0;
        MonoBehaviour.Freeze();
        GameManager.Singleton.TakeInput(GameInputType.PlayerDeath);
    }

    public void UpgradeAttackDamage()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            AttackDamagePoints++;
        }
    }

    public void UpgradeAttackSpeed()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            AttackSpeedPoints++;
            AttackCooldown.Modify(1 / Configuration.PLAYER_ATTACK_SPEED(AttackSpeedPoints));
        }
    }

    public void UpgradeMoveSpeed()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            MoveSpeedPoints++;
        }
    }

    public void UpgradeSightRadius()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            SightRadiusPoints++;
        }
    }

    public void UpgradeProjectileDamage()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            ProjectileDamagePoints++;
        }
    }

    public void UpgradeMaxHealth()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            MaxHealthPoints++;
        }
    }

    public void UpgradeMaxStamina()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            MaxStaminaPoints++;
        }
    }

    public void UpgradeHealthRegen()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            HealthRegenPoints++;
        }
    }

    public void UpgradeStaminaRegen()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            StaminaRegenPoints++;
        }
    }
}
