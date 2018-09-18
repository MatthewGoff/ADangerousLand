using UnityEngine;
using ProtoBuf;

[ProtoContract]
public class EnemyManager : CombatantManager
{
    public EnemyMonoBehaviour MonoBehaviour;
    private HealthBarMonoBehaviour HealthBar;
    private DamageNumbersCanvasMonoBehaviour DamageNumbersCanvas;
    public bool Awake = false;

    [ProtoMember(1)] public EnemyType EnemyType;
    [ProtoMember(2)] public (float X, float Y) Position { get; private set; }
    [ProtoMember(3)] public readonly BasicAI AI;
    [ProtoMember(4)] public readonly float MoveSpeed;
    [ProtoMember(5)] public float Aoe;

    [ProtoMember(6)] private int MaxHealth;
    [ProtoMember(7)] private float CurrentHealth;
    [ProtoMember(8)] private ChunkIndex CurrentChunk;
    [ProtoMember(9)] private Cooldown SlashCooldown;
    [ProtoMember(10)] private float Damage;

    public EnemyManager(WorldLocation worldLocation, EnemyType enemyType)
    {
        EnemyType = enemyType;
        SlashCooldown = new Cooldown(1/Configuration.ENEMY_CONFIGURATIONS[EnemyType].AttackSpeed);
        Position = worldLocation.Tuple;
        CurrentChunk = GameManager.Singleton.World.GetChunkIndex(worldLocation);
        MaxHealth = Configuration.ENEMY_CONFIGURATIONS[EnemyType].MaxHealth;
        MoveSpeed = Configuration.ENEMY_CONFIGURATIONS[EnemyType].MoveSpeed;
        Aoe = Configuration.ENEMY_CONFIGURATIONS[EnemyType].Aoe;
        Damage = Configuration.ENEMY_CONFIGURATIONS[EnemyType].Damage;
        CurrentHealth = MaxHealth;
        Team = 1;
        if (Configuration.ENEMY_CONFIGURATIONS[EnemyType].AIType == AIType.Basic)
        {
            AI = new BasicAI();
        }
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
        return AI.Update(this);
    }

    public void WakeUp()
    {
        Awake = true;

        Vector2 position = new Vector2(Position.X, Position.Y);

        GameObject enemy = GameObject.Instantiate(Prefabs.ENEMY_PREFAB, position, Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
        MonoBehaviour = enemy.GetComponent<EnemyMonoBehaviour>();
        MonoBehaviour.AssignManager(this);

        GameObject healthBar = GameObject.Instantiate(Prefabs.HEALTH_BAR_PREFAB, position + new Vector2(0, 0.625f), Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
        healthBar.transform.SetParent(enemy.transform);
        HealthBar = healthBar.GetComponent<HealthBarMonoBehaviour>();
        HealthBar.AssignManager(this);
        UpdateHealthBar();

        GameObject damageNumbersCanvas = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_CANVAS_PREFAB, position + new Vector2(0, 0.625f), Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
        damageNumbersCanvas.transform.SetParent(enemy.transform);
        DamageNumbersCanvas = damageNumbersCanvas.GetComponent<DamageNumbersCanvasMonoBehaviour>();
    }

    public void Sleep()
    {
        if (Awake)
        {
            Awake = false;
            Position = (MonoBehaviour.transform.position.x, MonoBehaviour.transform.position.y);
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
        GameManager.Singleton.World.GetChunk(CurrentChunk).EnemyHasDied(this);
    }

    public override int RecieveHit(float damage)
    {
        DamageNumbersCanvas.Log(damage.ToString());

        CurrentHealth -= damage;
        UpdateHealthBar();
        if (CurrentHealth <= 0)
        {
            Die();
            return Configuration.ENEMY_CONFIGURATIONS[EnemyType].ExperienceReward;
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

            SlashManager slash = new SlashManager(this, attackPosition, attackAngle, Aoe, Damage);
        }
    }

    private void UpdateHealthBar()
    {
        HealthBar.ShowHealth(CurrentHealth / MaxHealth);

    }

    public void Immigrate(ChunkIndex newHome)
    {
        CurrentChunk = newHome;
    }
}