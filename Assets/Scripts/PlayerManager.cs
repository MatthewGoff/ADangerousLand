﻿using UnityEngine;

public class PlayerManager : CombatantManager
{
    public float MoveSpeed = Configuration.DEFAULT_MOVE_SPEED;
    public readonly World World;
    public PlayerMonoBehaviour MonoBehaviour;

    public int MaxHealth;
    public float CurrentHealth;
    public float HealthRegen;
    public int MaxMana;
    public float CurrentMana;
    public float ManaRegen;

    private Cooldown SlashCooldown;

    public PlayerManager(World world, WorldLocation spawnLocation)
    {
        SlashCooldown = new Cooldown(1f);
        World = world;

        MaxHealth = 10;
        CurrentHealth = MaxHealth;
        HealthRegen = 0.1f;
        MaxMana = 10;
        CurrentMana = MaxMana;
        ManaRegen = 0.5f;

        GameObject player = GameObject.Instantiate(Prefabs.PLAYER_PREFAB, new Vector3(spawnLocation.X, spawnLocation.Y, 0), Quaternion.identity);
        MonoBehaviour = player.GetComponent<PlayerMonoBehaviour>();
        MonoBehaviour.AssignManager(this);
    }

    public void Update(float deltaTime)
    {
        CurrentHealth += HealthRegen * deltaTime;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        CurrentMana += ManaRegen * deltaTime;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana);
    }

    public Vector2 GetPlayerPosition()
    {
        return MonoBehaviour.transform.position;
    }

    public void SlashAttack(Vector2 position, Quaternion angle)
    {
        if (CurrentMana >= 1f && SlashCooldown.Use())
        {
            CurrentMana -= 1;
            SlashManager slash = new SlashManager(this, position, angle, 2);
        }
    }

    public override void RecieveHit()
    {

    }
}
