using UnityEngine;

public class EnemyManager : CombatantManager
{
    public bool Awake;
    public Vector2 Position { get; private set; }
    public readonly World World;
    public readonly BasicAI AI;
    public readonly float MoveSpeed = 3.5f;
    public EnemyMonoBehaviour MonoBehaviour;

    private int MaxHealth;
    private float CurrentHealth;
    private HealthBarMonoBehaviour HealthBar;
    private DamageNumbersCanvasMonoBehaviour DamageNumbersCanvas;
    private Chunk CurrentChunk;
    private Cooldown SlashCooldown;
    private readonly int Exp = 1;

    public EnemyManager(World world, WorldLocation worldLocation)
    {
        SlashCooldown = new Cooldown(1f);
        World = world;
        Position = new Vector2(worldLocation.X, worldLocation.Y);
        CurrentChunk = world.GetChunk(world.GetChunkIndex(worldLocation));
        AI = new BasicAI(this);
        MaxHealth = 5;
        CurrentHealth = MaxHealth;
        Team = 1;
    }

    public void CheckTreadmill(Treadmill treadmill)
    {
        if (Awake)
        {
            bool onTreadmill = treadmill.OnTreadmill(MonoBehaviour.transform.position);
            if (!onTreadmill)
            {
                Sleep();
            }
        }
        else
        {
            bool onTreadmill = treadmill.OnTreadmill(Position);
            if (onTreadmill)
            {
                WakeUp();
            }
        }
    }

    public Vector2 Update()
    {
        return AI.Update();
    }

    public void WakeUp()
    {
        Awake = true;

        GameObject enemy = GameObject.Instantiate(Prefabs.ENEMY_PREFAB, Position, Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
        MonoBehaviour = enemy.GetComponent<EnemyMonoBehaviour>();
        MonoBehaviour.AssignManager(this);

        GameObject healthBar = GameObject.Instantiate(Prefabs.HEALTH_BAR_PREFAB, Position + new Vector2(0, 0.625f), Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
        healthBar.transform.SetParent(enemy.transform);
        HealthBar = healthBar.GetComponent<HealthBarMonoBehaviour>();
        HealthBar.AssignManager(this);
        UpdateHealthBar();

        GameObject damageNumbersCanvas = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_CANVAS_PREFAB, Position + new Vector2(0, 0.625f), Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
        damageNumbersCanvas.transform.SetParent(enemy.transform);
        DamageNumbersCanvas = damageNumbersCanvas.GetComponent<DamageNumbersCanvasMonoBehaviour>();
        DamageNumbersCanvas.AssignManager(this);
    }

    public void Sleep()
    {
        if (Awake)
        {
            Awake = false;
            Position = MonoBehaviour.transform.position;
            DamageNumbersCanvas.transform.SetParent(null);
            MonoBehaviour.Destroy();
            HealthBar.Destroy();
            DamageNumbersCanvas.Destroy();
        }
    }

    public void Die()
    {
        DamageNumbersCanvas.transform.SetParent(null);
        MonoBehaviour.Destroy();
        HealthBar.Destroy();
        DamageNumbersCanvas.Destroy();
        CurrentChunk.EnemyHasDied(this);
    }

    public override int RecieveHit(float damage)
    {
        DamageNumbersCanvas.Log(damage.ToString());

        CurrentHealth -= damage;
        UpdateHealthBar();
        if (CurrentHealth <= 0)
        {
            Die();
            return Exp;
        }
        else
        {
            return 0;
        }
    }

    public override void RecieveExp(int exp)
    {

    }

    public void SlashAttack(Vector2 attackTarget)
    {
        if (SlashCooldown.Use())
        {
            Vector2 attackPosition = MonoBehaviour.transform.position;
            Vector2 attackVector = attackTarget - attackPosition;
            float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);
            float aoe = 1.5f;
            float damage = 0.5f;

            SlashManager slash = new SlashManager(this, attackPosition, attackAngle, aoe, damage);
        }
    }

    private void UpdateHealthBar()
    {
        HealthBar.ShowHealth((float)CurrentHealth / MaxHealth);

    }

    public void Immigrate(Chunk newHome)
    {
        CurrentChunk = newHome;
    }
}