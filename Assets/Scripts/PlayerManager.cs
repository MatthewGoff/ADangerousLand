﻿using UnityEngine;

public class PlayerManager : CombatantManager
{
    public readonly World World;
    public PlayerMonoBehaviour MonoBehaviour;

    public float CurrentHealth;
    public float CurrentMana;

    public int Experience;
    public int Level;
    public bool AttemptingSprint;
    public bool Sprinting;

    private Cooldown AttackCooldown;
    private bool Dead;

    public float AttackDamage = 1;
    public float AttackSpeed = 1;
    public float MoveSpeed = 5;
    public float SightRadiusNear = 7;
    public float SightRadiusFar = 11;
    public float ProjectileDamage = 0.5f;
    public float MeleeAoe = 1.5f;
    public int MaxHealth = 10;
    public int MaxStamina = 5;
    public float HealthRegen = 0.1f;
    public float StaminaRegen = 0.5f;

    public PlayerManager(World world)
    {
        AttackCooldown = new Cooldown(1/AttackSpeed);
        World = world;

        Team = 0;
        Experience = 0;
        Level = 1;
    }

    public void Spawn(WorldLocation spawnLocation)
    {
        Dead = false;
        CurrentHealth = MaxHealth;
        CurrentMana = MaxStamina;
        Sprinting = false;
        AttemptingSprint = false;

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
            GameManager.Singleton.LevelUp();
        }
    }

    private void Die()
    {
        Dead = true;
        CurrentHealth = 0;
        MonoBehaviour.Freeze();
        GameManager.Singleton.Input(GameInputType.PlayerDeath);
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
        AttackDamage = GetNextAttackDamage();
    }

    public void UpgradeAttackSpeed()
    {
        AttackSpeed = GetNextAttackSpeed();
        AttackCooldown.Modify(1 / AttackSpeed);
    }

    public void UpgradeMoveSpeed()
    {
        MoveSpeed = GetNextMoveSpeed();
    }

    public void UpgradeSightRadius()
    {
        SightRadiusFar *= 1.1f;
        SightRadiusNear *= 1.1f;
    }

    public void UpgradeProjectileDamage()
    {
        ProjectileDamage = GetNextProjectileDamage();
    }

    public void UpgradeMeleeAoe()
    {
        MeleeAoe = GetNextMeleeAoe();
    }

    public void UpgradeHealthRegen()
    {
        HealthRegen = GetNextHealthRegen();
    }

    public void UpgradeStaminaRegen()
    {
        StaminaRegen = GetNextStaminaRegen();
    }
}
