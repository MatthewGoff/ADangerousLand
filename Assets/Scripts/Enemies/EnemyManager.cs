using UnityEngine;
using MessagePack;

[MessagePackObject]
public class EnemyManager : CombatantManager
{
    [IgnoreMember] public EnemyMonoBehaviour MonoBehaviour;
    [IgnoreMember] public bool Awake = false;

    [IgnoreMember] private HealthBarMonoBehaviour HealthBar;
    [IgnoreMember] private DamageNumbersCanvasMonoBehaviour DamageNumbersCanvas;
    [IgnoreMember] private Cooldown SlashCooldown;

    // X and Y position are held separately since MessagePack cannot serialize tupples
    [Key(0)] public float XPosition { get; private set; }
    [Key(1)] public float YPosition { get; private set; }
    [Key(2)] public EnemyType EnemyType;
    [Key(3)] public readonly EnemyAI AI;
    [Key(4)] public readonly float MoveSpeed;
    [Key(5)] public float Aoe;
    [Key(6)] public int MaxHealth;
    [Key(7)] public float CurrentHealth;
    [Key(8)] public ChunkIndex CurrentChunk;
    [Key(9)] public float Damage;
    [Key(10)] public float AgroDistance;
    [Key(11)] public float DeAgroDistance;
    [Key(12)] public float MinAgroDuration;

    public EnemyManager(WorldLocation worldLocation, EnemyType enemyType)
    {
        EnemyType = enemyType;
        XPosition = worldLocation.X;
        YPosition = worldLocation.Y;
        CurrentChunk = GameManager.Singleton.World.GetChunkIndex(worldLocation);
        MaxHealth = Configuration.ENEMY_CONFIGURATIONS[EnemyType].MaxHealth;
        MoveSpeed = Configuration.ENEMY_CONFIGURATIONS[EnemyType].MoveSpeed;
        Aoe = Configuration.ENEMY_CONFIGURATIONS[EnemyType].Aoe;
        Damage = Configuration.ENEMY_CONFIGURATIONS[EnemyType].Damage;
        AgroDistance = Configuration.ENEMY_CONFIGURATIONS[EnemyType].AgroDistance;
        DeAgroDistance = Configuration.ENEMY_CONFIGURATIONS[EnemyType].DeAgroDistance;
        MinAgroDuration = Configuration.ENEMY_CONFIGURATIONS[EnemyType].MinAgroDuration;
        CurrentHealth = MaxHealth;
        Team = 1;
        if (Configuration.ENEMY_CONFIGURATIONS[EnemyType].AIType == AIType.Basic)
        {
            AI = new BasicAI();
        }
    }

    [SerializationConstructor]
    public EnemyManager(
        float xPosition,
        float yPosition,
        EnemyType enemyType,
        EnemyAI aI,
        float moveSpeed,
        float aoe,
        int maxHealth,
        float currentHealth,
        ChunkIndex currentChunk,
        float damage,
        float agroDistance,
        float deAgroDistance,
        float minAgroDuration)
    {
        XPosition = xPosition;
        YPosition = yPosition;
        EnemyType = enemyType;
        AI = aI;
        MoveSpeed = moveSpeed;
        Aoe = aoe;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        CurrentChunk = currentChunk;
        Damage = damage;
        AgroDistance = agroDistance;
        DeAgroDistance = deAgroDistance;
        MinAgroDuration = minAgroDuration;
        
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
            bool onTreadmill = treadmill.OnTreadmill((XPosition, YPosition));
            if (onTreadmill)
            {
                WakeUp();
            }
        }
    }

    public Vector2 FixedUpdate()
    {
        return AI.FixedUpdate(this);
    }

    public void WakeUp()
    {
        Awake = true;

        Vector2 position = new Vector2(XPosition, YPosition);
        SlashCooldown = new Cooldown(1 / Configuration.ENEMY_CONFIGURATIONS[EnemyType].AttackSpeed);


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
            XPosition = MonoBehaviour.transform.position.x;
            YPosition = MonoBehaviour.transform.position.y;
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