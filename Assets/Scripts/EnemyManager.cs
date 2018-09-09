using UnityEngine;

public class EnemyManager : CombatantManager
{
    public bool Awake;
    public Vector2 Position { get; private set; }
    private EnemyMonoBehaviour MonoBehaviour;
    private HealthBarMonoBehaviour HealthBar;
    private Chunk CurrentChunk;

    public EnemyManager(WorldLocation worldLocation, Chunk chunk)
    {
        Position = new Vector2(worldLocation.X, worldLocation.Y);
        CurrentChunk = chunk;
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
    }

    public void Sleep()
    {
        Awake = false;
        Position = MonoBehaviour.transform.position;
        MonoBehaviour.Destroy();
        HealthBar.Destroy();
    }

    public void Die()
    {
        MonoBehaviour.Destroy();
        CurrentChunk.EnemyHasDied(this);
    }

    public override void RecieveHit()
    {
        CurrentHealth--;
        HealthBar.ShowHealth((float)CurrentHealth / MaxHealth);
        if (CurrentHealth == 0)
        {
            Die();
        }
    }
}