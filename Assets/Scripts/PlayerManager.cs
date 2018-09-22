using UnityEngine;
using MessagePack;

[MessagePackObject]
public class PlayerManager : CombatantManager
{
    [IgnoreMember] private bool Dead;
    [IgnoreMember] public PlayerMonoBehaviour MonoBehaviour;
    [IgnoreMember] public bool Sprinting;
    [IgnoreMember] public float CurrentHealth;
    [IgnoreMember] public float CurrentStamina;
    [IgnoreMember] private Cooldown AttackCooldown;

    [Key(0)] public int PlayerIdentifier;
    [Key(1)] public string Name;
    [Key(2)] public DeathPenaltyType DeathPenalty;

    [Key(3)] public int Experience;
    [Key(4)] public int Level;
    [Key(5)] public int PassivePoints;

    [Key(6)] public int AttackType;
    [Key(7)] public float AttackDamage;
    [Key(8)] public float AttackSpeed;
    [Key(9)] public float MoveSpeed;
    [Key(10)] public float SightRadiusNear;
    [Key(11)] public float SightRadiusFar;
    [Key(12)] public float ProjectileDamage;
    [Key(13)] public float MeleeAoe;
    [Key(14)] public int MaxHealth;
    [Key(15)] public int MaxStamina;
    [Key(16)] public float HealthRegen;
    [Key(17)] public float StaminaRegen;

    public PlayerManager(int identifier, string name, DeathPenaltyType deathPenalty)
    {
        PlayerIdentifier = identifier;
        Name = name;
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
        MeleeAoe = 1.5f;
        MaxHealth = 10;
        MaxStamina = 10;
        HealthRegen = 0.1f;
        StaminaRegen = 1f;
    }

    [SerializationConstructor]
    public PlayerManager(
        int playerIdentifier,
        string name,
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
        Team = 0;
        Dead = false;
        Sprinting = false;
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;

        GameObject player = GameObject.Instantiate(Prefabs.PLAYER_PREFAB, new Vector3(spawnLocation.X + 0.5f, spawnLocation.Y + 0.5f, 0), Quaternion.identity);
        MonoBehaviour = player.GetComponent<PlayerMonoBehaviour>();
        MonoBehaviour.AssignManager(this);
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
        }
    }

    public Vector2 GetPlayerPosition()
    {
        return MonoBehaviour.transform.position;
    }

    public void WeaponSwap()
    {
        AttackType = (AttackType + 1) % 2;
    }

    public void SlashAttack(Vector2 attackTarget)
    {
        if (CurrentStamina >= 2f && AttackCooldown.Use())
        {
            Vector2 attackPosition = MonoBehaviour.transform.position;
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
            Vector2 attackPosition = MonoBehaviour.transform.position;
            Vector2 attackVector = attackTarget - attackPosition;
            float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);

            CurrentStamina -= 2;
            BoltManager slash = new BoltManager(this, attackPosition, attackAngle, AttackDamage*ProjectileDamage);
        }
    }

    public bool StartSprinting()
    {
        if (!Sprinting && CurrentStamina >= 1f)
        {
            Sprinting = true;
            return true;
        }
        return false;
    }

    public void StopSprinting()
    {
        Sprinting = false;
    }

    public override int RecieveHit(float damage)
    {
        CurrentHealth -= damage;
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
