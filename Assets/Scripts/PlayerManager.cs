using UnityEngine;

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
    private bool Dead;

    public PlayerManager(World world)
    {
        SlashCooldown = new Cooldown(1f);
        World = world;

        MaxHealth = 10;
        HealthRegen = 0.1f;
        MaxMana = 10;
        ManaRegen = 0.5f;

        Team = 0;
    }

    public void Spawn(WorldLocation spawnLocation)
    {
        Dead = false;
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;

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
            CurrentMana += ManaRegen * deltaTime;
            CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana);
        }
    }

    public Vector2 GetPlayerPosition()
    {
        return MonoBehaviour.transform.position;
    }

    public void SlashAttack(Vector2 attackTarget)
    {
        if (CurrentMana >= 1f && SlashCooldown.Use())
        {
            Vector2 attackPosition = MonoBehaviour.transform.position;
            Vector2 attackVector = attackTarget - attackPosition;
            Quaternion attackAngle = Quaternion.Euler(0, 0, -Vector2.SignedAngle(attackVector, new Vector2(-1, 1)));
            float aoe = 2f;

            CurrentMana -= 1;
            SlashManager slash = new SlashManager(this, attackPosition, attackAngle, aoe);
        }
    }

    public override void RecieveHit()
    {
        CurrentHealth--;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Dead = true;
        CurrentHealth = 0;
        MonoBehaviour.Freeze();
        GameManager.Singleton.Input(GameInputType.PlayerDeath);
    }
}
