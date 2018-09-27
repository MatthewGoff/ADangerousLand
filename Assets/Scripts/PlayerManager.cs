using UnityEngine;
using MessagePack;

[MessagePackObject]
public class PlayerManager : CombatantManager
{
    [IgnoreMember] private static readonly Vector2 ATTACK_ORIGIN = new Vector2(0f, 1f);
    [IgnoreMember] private static readonly Vector2 CENTER = new Vector2(0f, 1f);

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
    [Key(8)] public float AttackDamage;
    [Key(9)] public float AttackSpeed;
    [Key(10)] public float MoveSpeed;
    [Key(11)] public float SightRadiusNear;
    [Key(12)] public float SightRadiusFar;
    [Key(13)] public float ProjectileDamage;
    [Key(14)] public float MeleeAoe;
    [Key(15)] public int MaxHealth;
    [Key(16)] public int MaxStamina;
    [Key(17)] public float HealthRegen;
    [Key(18)] public float StaminaRegen;

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
        AttackDamage = 1;
        AttackSpeed = 1;
        MoveSpeed = 5;
        SightRadiusNear = 7;
        SightRadiusFar = 11;
        ProjectileDamage = 0.5f;
        MeleeAoe = 3f;
        MaxHealth = 10;
        MaxStamina = 10;
        HealthRegen = 0.1f;
        StaminaRegen = 1f;
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
        float attackDamage,
        float attackSpeed,
        float moveSpeed,
        float sightRadiusNear,
        float sightRadiusFar,
        float projectileDamage,
        float meleeAoe,
        int maxHealth,
        int maxStamina,
        float healthRegen,
        float staminaRegen)
    {
        PlayerIdentifier = playerIdentifier;
        Name = name;
        Color = color;
        DeathPenalty = deathPenalty;
        Experience = experience;
        Level = level;
        PassivePoints = passivePoints;
        AttackType = attackType;
        AttackDamage = attackDamage;
        AttackSpeed = attackSpeed;
        MoveSpeed = moveSpeed;
        SightRadiusNear = sightRadiusNear;
        SightRadiusFar = sightRadiusFar;
        ProjectileDamage = projectileDamage;
        MeleeAoe = meleeAoe;
        MaxHealth = maxHealth;
        MaxStamina = maxStamina;
        HealthRegen = healthRegen;
        StaminaRegen = staminaRegen;
    }

    public void Spawn(WorldLocation spawnLocation)
    {
        AttackCooldown = new Cooldown(1 / AttackSpeed);
        DashCooldown = new Cooldown(1f);
        StompCooldown = new Cooldown(1f);
        Team = 0;
        Dead = false;
        Sprinting = false;
        Blocking = false;
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;

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
            CurrentHealth += HealthRegen * Time.fixedDeltaTime;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            CurrentStamina += StaminaRegen * Time.fixedDeltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);

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
            SlashManager slash = new SlashManager(this, attackPosition, attackAngle, MeleeAoe, AttackDamage);
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
            BoltManager slash = new BoltManager(this, attackPosition, attackAngle, AttackDamage*ProjectileDamage);
        }
    }

    public bool Dash(Vector2 dashTarget)
    {
        if (CurrentStamina >= 3 && DashCooldown.Use())
        {
            DashManager dash = new DashManager(this, MonoBehaviour.transform.position, dashTarget, AttackDamage * 0.5f);
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
            StompManager stomp = new StompManager(this, (Vector2)MonoBehaviour.transform.position, 0f);
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
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
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
            return SightRadiusNear * 0.5f;
        }
        else
        {
            return SightRadiusNear;
        }
    }

    public float GetSightRadiusFar()
    {
        if (Sprinting)
        {
            return SightRadiusFar * 0.5f;
        }
        else
        {
            return SightRadiusFar;
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

    public float GetNextAttackDamage()
    {
        return AttackDamage * 1.1f;
    }

    public float GetNextAttackSpeed()
    {
        return AttackSpeed * 1.1f;
    }

    public float GetNextMoveSpeed()
    {
        return MoveSpeed * 1.1f;
    }

    public float GetNextSightRadius()
    {
        return SightRadiusNear * 1.1f;
    }

    public float GetNextProjectileDamage()
    {
        return ProjectileDamage * 1.1f;
    }

    public float GetNextMeleeAoe()
    {
        return MeleeAoe * 1.1f;
    }

    public float GetNextHealthRegen()
    {
        return HealthRegen * 1.1f;
    }

    public float GetNextStaminaRegen()
    {
        return StaminaRegen * 1.1f;
    }

    public void UpgradeAttackDamage()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            AttackDamage = GetNextAttackDamage();
        }
    }

    public void UpgradeAttackSpeed()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            AttackSpeed = GetNextAttackSpeed();
            AttackCooldown.Modify(1 / AttackSpeed);
        }
    }

    public void UpgradeMoveSpeed()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            MoveSpeed = GetNextMoveSpeed();
        }
    }

    public void UpgradeSightRadius()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            SightRadiusFar *= 1.1f;
            SightRadiusNear *= 1.1f;
        }
    }

    public void UpgradeProjectileDamage()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            ProjectileDamage = GetNextProjectileDamage();
        }
    }

    public void UpgradeMeleeAoe()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            MeleeAoe = GetNextMeleeAoe();
        }
    }

    public void UpgradeHealthRegen()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            HealthRegen = GetNextHealthRegen();
        }
    }

    public void UpgradeStaminaRegen()
    {
        if (PassivePoints > 0)
        {
            PassivePoints--;
            StaminaRegen = GetNextStaminaRegen();
        }
    }
}
