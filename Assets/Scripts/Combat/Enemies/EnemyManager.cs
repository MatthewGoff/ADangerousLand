using UnityEngine;
using MessagePack;
using System.Collections.Generic;
using ADL.World;
using ADL.Combat.Enemies.AI;
using ADL.Combat.Attacks;
using ADL.Core;

namespace ADL.Combat.Enemies
{
    /// <summary>
    /// The principle class of an enemy. Owns and manages the enemy's AI, its MonoBehaviour and all of the data not related to its visual or physical attributes.
    /// </summary>
    /// <remarks>
    /// See the Manager/MonoBehaviour design pattern
    /// </remarks>
    [MessagePackObject]
    public class EnemyManager : CombatantManager
    {
        /// <summary>
        /// The MonoBehaviour responsible for the enemies visual and physical implementation
        /// </summary>
        [IgnoreMember] public EnemyMonoBehaviour MonoBehaviour;

        /// <summary>
        /// Whether this enemy is implemented in the world or is inactive to save on performance
        /// </summary>
        [IgnoreMember] private bool Awake = false;
        /// <summary>
        /// The cooldown of this enemy's slash attack
        /// </summary>
        [IgnoreMember] private Cooldown SlashCooldown;
        /// <summary>
        /// A list of attack which have hit this enemy for which we want to ignore subsequent hits
        /// </summary>
        [IgnoreMember] private List<AttackManager> Immunities;

        /// <summary>
        /// The x position of this enemy in the world
        /// </summary>
        /// <remarks>
        /// The x and y position of the enemy are not a vector because the position needs to be serializable
        /// </remarks>
        [Key(0)] public float XPosition { get; private set; }
        /// <summary>
        /// The y position of this enemy in the world
        /// </summary>
        /// <remarks>
        /// The x and y position of the enemy are not a vector because the position needs to be serializable
        /// </remarks>
        [Key(1)] public float YPosition { get; private set; }
        /// <summary>
        /// The enemy type of this instance
        /// </summary>
        [Key(2)] public EnemyType EnemyType;
        /// <summary>
        /// The AI of this enemy
        /// </summary>
        [Key(3)] public readonly EnemyAI AI;
        /// <summary>
        /// The enemies movement speed
        /// </summary>
        [Key(4)] public readonly float MoveSpeed;
        /// <summary>
        /// The AOE of melee attacks made by this enemy
        /// </summary>
        [Key(5)] public float Aoe;
        /// <summary>
        /// The maximum health of this enemy
        /// </summary>
        [Key(6)] public int MaxHealth;
        /// <summary>
        /// The current health of this enemy
        /// </summary>
        [Key(7)] public float CurrentHealth;
        /// <summary>
        /// The chunk in which this enemy currently resides
        /// </summary>
        [Key(8)] public ChunkIndex CurrentChunk;
        /// <summary>
        /// The base damage this enemy does with attacks
        /// </summary>
        [Key(9)] public float Damage;
        /// <summary>
        /// The distance from the player under which underwhich which this enemy will become aggro'd
        /// </summary>
        [Key(10)] public float AgroDistance;
        /// <summary>
        /// The distance from the player over which this enemy will start losing aggro
        /// </summary>
        [Key(11)] public float DeAgroDistance;
        /// <summary>
        /// The duration of absent aggro before which the enemy will de-aggro
        /// </summary>
        [Key(12)] public float MinAgroDuration;

        /// <summary>
        /// Create a new EnemyManager
        /// </summary>
        /// <param name="spawnPosition">
        /// The initial position of this enemy in the world
        /// </param>
        /// <param name="enemyType">
        /// The enemy type of this instance
        /// </param>
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

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// See documentation of member fields
        /// </remarks>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="enemyType"></param>
        /// <param name="aI"></param>
        /// <param name="moveSpeed"></param>
        /// <param name="aoe"></param>
        /// <param name="maxHealth"></param>
        /// <param name="currentHealth"></param>
        /// <param name="currentChunk"></param>
        /// <param name="damage"></param>
        /// <param name="agroDistance"></param>
        /// <param name="deAgroDistance"></param>
        /// <param name="minAgroDuration"></param>
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

        /// <summary>
        /// Check whether this enemy is on the treadmill and should be physically instantiated
        /// </summary>
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

        /// <summary>
        /// Update this enemy
        /// </summary>
        /// <returns>
        /// The position to which this enemies AI has directed movement
        /// </returns>
        public Vector2 FixedUpdate()
        {
            return AI.FixedUpdate(this);
        }

        /// <summary>
        /// Fully instantiate the enemy so that it is active and visible in the world
        /// </summary>
        public void WakeUp()
        {
            if (!Awake)
            {
                Awake = true;

                Vector2 position = new Vector2(XPosition, YPosition);
                SlashCooldown = new Cooldown(1 / Configuration.ENEMY_CONFIGURATIONS[EnemyType].AttackSpeed);

                GameObject prefab = (GameObject)Resources.Load("Prefabs/Enemies/" + Configuration.ENEMY_CONFIGURATIONS[EnemyType].PrefabLocation);
                GameObject enemy = GameObject.Instantiate(prefab, position, Quaternion.identity);
                MonoBehaviour = enemy.GetComponent<EnemyMonoBehaviour>();
                MonoBehaviour.AssignManager(this);

                Immunities = new List<AttackManager>();
            }
        }

        /// <summary>
        /// Remove the enemy from the scene, storing its last known position
        /// </summary>
        public void Sleep()
        {
            if (Awake)
            {
                Awake = false;
                XPosition = MonoBehaviour.transform.position.x;
                YPosition = MonoBehaviour.transform.position.y;
                MonoBehaviour.Destroy();
            }
        }

        /// <summary>
        /// Get the current position of this enemy
        /// </summary>
        /// <returns>
        /// The position of this enemy
        /// </returns>
        public override Vector2 GetPosition()
        {
            if (Awake)
            {
                return MonoBehaviour.transform.position;
            }
            else
            {
                return new Vector2(XPosition, YPosition);
            }

        }

        /// <summary>
        /// Destroy the enemy's monobehaviour and inform the current chunk the enemy has died.
        /// </summary>
        public void Die()
        {
            //It is neccesary to un-child the damage numbers canvas so that it is not destoryed with the parent gameobject
            MonoBehaviour.DamageNumbersCanvas.transform.SetParent(null);

            MonoBehaviour.Destroy();
            GameManager.Singleton.World.GetChunk(CurrentChunk).EnemyHasDied(this);
        }

        /// <summary>
        /// Recieve a hit from an attack
        /// </summary>
        /// <param name="attackManager">
        /// The AttackManager of the attack
        /// </param>
        /// <param name="damage">
        /// The damage done by the attack
        /// </param>
        /// <param name="knockback">
        /// The knockback done by the attack
        /// </param>
        /// <returns>
        /// The experience given if the attack killed this enemy
        /// </returns>
        public override int RecieveHit(AttackManager attackManager, float damage, Vector2 knockback)
        {
            if (!Immunities.Contains(attackManager))
            {
                Immunities.Add(attackManager);
                attackManager.AddExpirationListner(AttackExpired);
                return RecieveHit(damage, knockback);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Private helper for RecieveHit
        /// </summary>
        /// <param name="damage">
        /// The damage done by the incoming hit
        /// </param>
        /// <param name="knockback">
        /// The knockback done by the incoming hit
        /// </param>
        /// <returns>
        /// The experience given if the hit killed this enemy
        /// </returns>
        private int RecieveHit(float damage, Vector2 knockback)
        {
            MonoBehaviour.DamageNumbersCanvas.Log(damage);
            MonoBehaviour.Knockback(knockback);

            CurrentHealth -= damage;
            MonoBehaviour.UpdateHealthBar();
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

        /// <summary>
        /// Remove an expired attack from this enemies list of Immunities
        /// </summary>
        /// <param name="attackManager"></param>
        public void AttackExpired(AttackManager attackManager)
        {
            Immunities.Remove(attackManager);
        }

        /// <summary>
        /// Recieve experience
        /// </summary>
        /// <param name="exp">
        /// The ammount of experience recieved
        /// </param>
        public override void RecieveExp(int exp)
        {

        }

        /// <summary>
        /// Perform a slash attack
        /// </summary>
        /// <param name="attackTarget">
        /// The position in the world towards which the enemy will attack
        /// </param>
        public void SlashAttack(Vector2 attackTarget)
        {
            if (SlashCooldown.Use())
            {
                Vector2 attackPosition = (Vector2)MonoBehaviour.transform.position + Configuration.ENEMY_CONFIGURATIONS[EnemyType].AttackOrigin;
                Vector2 attackVector = attackTarget - attackPosition;
                float attackAngle = Vector2.SignedAngle(Vector2.right, attackVector);

                SlashManager slash = new SlashManager(this, attackPosition, attackAngle, Aoe, Damage);
            }
        }

        /// <summary>
        /// Get the center of this Enemie's spawnbox
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCenter()
        {
            return (Vector2)MonoBehaviour.transform.position + new Vector2(0, Configuration.ENEMY_CONFIGURATIONS[EnemyType].Height / 2f);
        }

        /// <summary>
        /// Inform this enemy that it has crossed a chunk boundryinto a new chunk
        /// </summary>
        /// <param name="newHome">
        /// The Index of the chunk in which the enemy now resides
        /// </param>
        public void Immigrate(ChunkIndex newHome)
        {
            CurrentChunk = newHome;
        }
    }
}