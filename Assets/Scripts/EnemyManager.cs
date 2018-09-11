using UnityEngine;

public class EnemyManager : CombatantManager
{
    public bool Awake;
    public Vector2 Position { get; private set; }
    public readonly World World;
    public readonly EnemyAI AI;
    public readonly float MoveSpeed = 4f;

    private int MaxHealth;
    private float CurrentHealth;
    private EnemyMonoBehaviour MonoBehaviour;
    private HealthBarMonoBehaviour HealthBar;
    private DamageNumbersCanvasMonoBehaviour DamageNumbersCanvas;
    private Chunk CurrentChunk;
    
    public EnemyManager(World world, WorldLocation worldLocation, Chunk chunk)
    {
        World = world;
        Position = new Vector2(worldLocation.X, worldLocation.Y);
        CurrentChunk = chunk;
        AI = new EnemyAI(this);
        MaxHealth = 10;
        CurrentHealth = MaxHealth;
    }

    public void Update(Treadmill treadmill)
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
        Awake = false;
        Position = MonoBehaviour.transform.position;
        DamageNumbersCanvas.transform.SetParent(null);
        MonoBehaviour.Destroy();
        HealthBar.Destroy();
        DamageNumbersCanvas.Destroy();
    }

    public void Die()
    {
        DamageNumbersCanvas.transform.SetParent(null);
        MonoBehaviour.Destroy();
        HealthBar.Destroy();
        DamageNumbersCanvas.Destroy();
        CurrentChunk.EnemyHasDied(this);
    }

    public override void RecieveHit()
    {
        DamageNumbersCanvas.Log("12345");

        CurrentHealth--;
        UpdateHealthBar();
        if (CurrentHealth == 0)
        {
            Die();
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