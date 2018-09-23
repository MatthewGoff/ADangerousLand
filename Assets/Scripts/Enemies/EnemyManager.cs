using UnityEngine;
using MessagePack;

[MessagePackObject]
public class EnemyManager : CombatantManager
{
    [IgnoreMember] public EnemyMonoBehaviour MonoBehaviour;

    [IgnoreMember] private bool Awake = false;
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

    public EnemyManager(Vector2 spawnPosition, EnemyType enemyType)
    {
        EnemyType = enemyType;
        XPosition = spawnPosition.x;
        YPosition = spawnPosition.y;
        CurrentChunk = GameManager.Singleton.World.GetChunkIndex(spawnPosition);
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

    public void CheckTreadmill()
    {
        if (Awake)
        {
            bool onTreadmill = GameManager.Singleton.World.OnTreadmill(MonoBehaviour.transform.position);
            if (!onTreadmill)
            {
                Sleep();
            }
        }
        else
        {
            bool onTreadmill = GameManager.Singleton.World.OnTreadmill(new Vector2(XPosition, YPosition));
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
        if (!Awake)
        {
            Awake = true;

            Vector2 position = new Vector2(XPosition, YPosition);
            SlashCooldown = new Cooldown(1 / Configuration.ENEMY_CONFIGURATIONS[EnemyType].AttackSpeed);

            GameObject prefab = (GameObject) Resources.Load("Prefabs/Enemies/" + Configuration.ENEMY_CONFIGURATIONS[EnemyType].PrefabLocation);
            GameObject enemy = GameObject.Instantiate(prefab, position, Quaternion.identity);
            MonoBehaviour = enemy.GetComponent<EnemyMonoBehaviour>();
            MonoBehaviour.AssignManager(this);

            Vector2 top = position + new Vector2(0, Configuration.ENEMY_CONFIGURATIONS[EnemyType].Height + 0.15f);
            top = Util.RoundToPixel(top, Configuration.PIXELS_PER_UNIT);
            GameObject healthBar = GameObject.Instantiate(Prefabs.HEALTH_BAR_PREFAB, top, Quaternion.identity);
            healthBar.transform.SetParent(enemy.transform);
            HealthBar = healthBar.GetComponent<HealthBarMonoBehaviour>();
            HealthBar.AssignManager(this);
            UpdateHealthBar();

            GameObject damageNumbersCanvas = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_CANVAS_PREFAB, top, Quaternion.identity);
            damageNumbersCanvas.transform.SetParent(enemy.transform);
            DamageNumbersCanvas = damageNumbersCanvas.GetComponent<DamageNumbersCanvasMonoBehaviour>();
        }
    }

    public void Sleep()
    {
        if (Awake)
        {
            Awake = false;
            XPosition = MonoBehaviour.transform.position.x;
            YPosition = MonoBehaviour.transform.position.y;
            DamageNumbersCanvas.transform.SetParent(null);
            DamageNumbersCanvas.Destroy();
            MonoBehaviour.Destroy();
            HealthBar.Destroy();
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
        DamageNumbersCanvas.Log(damage);

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
            Vector2 attackPosition = (Vector2) MonoBehaviour.transform.position + Configuration.ENEMY_CONFIGURATIONS[EnemyType].AttackOrigin;
            Vector2 attackVector = attackTarget - attackPosition;
            float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);

            SlashManager slash = new SlashManager(this, attackPosition, attackAngle, Aoe, Damage);
        }
    }

    public Vector2 GetCenter()
    {
        return (Vector2)MonoBehaviour.transform.position + new Vector2(0, Configuration.ENEMY_CONFIGURATIONS[EnemyType].Height / 2f);
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