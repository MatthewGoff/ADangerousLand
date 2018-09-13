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
    public int Experience;
    public int Level;
    public bool Sprinting;

    private Cooldown AttackCooldown;
    private bool Dead;

    public PlayerManager(World world)
    {
        AttackCooldown = new Cooldown(1f);
        World = world;

        MaxHealth = 10;
        HealthRegen = 0.1f;
        MaxMana = 5;
        ManaRegen = 0.5f;

        Team = 0;
        Experience = 0;
        Level = 1;
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
        if (CurrentMana >= 1f && AttackCooldown.Use())
        {
            Vector2 attackPosition = MonoBehaviour.transform.position;
            Vector2 attackVector = attackTarget - attackPosition;
            float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);
            float aoe = 2f;

            CurrentMana -= 1;
            SlashManager slash = new SlashManager(this, attackPosition, attackAngle, aoe);
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
            BoltManager slash = new BoltManager(this, attackPosition, attackAngle);
        }
    }

    public bool AttemptSprint()
    {
        float manaCost = 1 * Time.fixedDeltaTime;
        if (Sprinting && CurrentMana >= manaCost)
        {
            CurrentMana -= manaCost;
            Configuration.FOG_INNER_RADIUS = 3.5f;
            Configuration.FOG_OUTER_RADIUS = 5.5f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int RecieveHit()
    {
        CurrentHealth -= 0.5f;
        if (CurrentHealth <= 0)
        {
            Die();
        }
        return 0;
    }

    public override void RecieveExp(int exp)
    {
        Experience += exp;
        if (Experience >= Configuration.LEVEL_EXPERIENCE[Level+1])
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
}
