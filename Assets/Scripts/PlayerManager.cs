using UnityEngine;
using System;

[Serializable]
public class PlayerManager : CombatantManager
{
    [NonSerialized] private bool Dead;
    [NonSerialized] private Cooldown AttackCooldown;
    [NonSerialized] public PlayerMonoBehaviour MonoBehaviour;
    [NonSerialized] public bool AttemptingSprint;
    [NonSerialized] public bool Sprinting;
    [NonSerialized] public float CurrentHealth;
    [NonSerialized] public float CurrentMana;

    public int PlayerIdentifier;
    public string Name;
    public DeathPenaltyType DeathPenalty;

    public int Experience;
    public int Level;
    public int PassivePoints;

    public int AttackType;
    public float AttackDamage;
    public float AttackSpeed;
    public float MoveSpeed;
    public float SightRadiusNear;
    public float SightRadiusFar;
    public float ProjectileDamage;
    public float MeleeAoe;
    public int MaxHealth;
    public int MaxStamina;
    public float HealthRegen;
    public float StaminaRegen;

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
        MaxStamina = 5;
        HealthRegen = 0.1f;
        StaminaRegen = 0.5f;
    }

    public void Spawn(WorldLocation spawnLocation)
    {
        AttackCooldown = new Cooldown(1 / AttackSpeed);
        Team = 0;
        Dead = false;
        Sprinting = false;
        AttemptingSprint = false;
        CurrentHealth = MaxHealth;
        CurrentMana = MaxStamina;

        GameObject player = GameObject.Instantiate(Prefabs.PLAYER_PREFAB, new Vector3(spawnLocation.X, spawnLocation.Y, 0), Quaternion.identity);
        MonoBehaviour = player.GetComponent<PlayerMonoBehaviour>();
        MonoBehaviour.AssignManager(this);
    }

    public void Sleep()
    {
        MonoBehaviour.Destroy();
    }

    public void Update(float deltaTime)
    {
        if (!Dead)
        {
            CurrentHealth += HealthRegen * deltaTime;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            CurrentMana += StaminaRegen * deltaTime;
            CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxStamina);
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
        if (CurrentMana >= 1f && AttackCooldown.Use())
        {
            Vector2 attackPosition = MonoBehaviour.transform.position;
            Vector2 attackVector = attackTarget - attackPosition;
            float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);

            CurrentMana -= 1;
            SlashManager slash = new SlashManager(this, attackPosition, attackAngle, MeleeAoe, AttackDamage);
        }
    }

    public void BoltAttack(Vector2 attackTarget)
    {
        if (CurrentMana >= 1f && AttackCooldown.Use())
        {
            Vector2 attackPosition = MonoBehaviour.transform.position;
            Vector2 attackVector = attackTarget - attackPosition;
            float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);

            CurrentMana -= 1;
            BoltManager slash = new BoltManager(this, attackPosition, attackAngle, AttackDamage*ProjectileDamage);
        }
    }

    public bool AttemptSprint()
    {
        float manaCost = 1 * Time.fixedDeltaTime;
        if (AttemptingSprint && CurrentMana >= manaCost)
        {
            CurrentMana -= manaCost;
            Sprinting = true;
        }
        else
        {
            Sprinting = false;
        }
        return Sprinting;
    }

    public override int RecieveHit(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
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
