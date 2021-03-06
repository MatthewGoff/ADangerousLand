﻿using UnityEngine;
using MessagePack;
using System.Collections.Generic;
using ADL.World;
using ADL.Combat.Enemies.AI;
using ADL.Combat.Attacks;
using ADL.Core;

namespace ADL.Combat.Enemies
{
    [MessagePackObject]
    public class EnemyManager : CombatantManager
    {
        [IgnoreMember] public EnemyMonoBehaviour MonoBehaviour;

        [IgnoreMember] private bool Awake = false;
        [IgnoreMember] private Cooldown SlashCooldown;
        [IgnoreMember] private List<AttackManager> Immunities;

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

                GameObject prefab = (GameObject)Resources.Load("Prefabs/Enemies/" + Configuration.ENEMY_CONFIGURATIONS[EnemyType].PrefabLocation);
                GameObject enemy = GameObject.Instantiate(prefab, position, Quaternion.identity);
                MonoBehaviour = enemy.GetComponent<EnemyMonoBehaviour>();
                MonoBehaviour.AssignManager(this);

                Immunities = new List<AttackManager>();
            }
        }

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

        public void Die()
        {
            //It is neccesary to un-child the damage numbers canvas so that it is not destoryed with the parent gameobject
            MonoBehaviour.DamageNumbersCanvas.transform.SetParent(null);

            MonoBehaviour.Destroy();
            GameManager.Singleton.World.GetChunk(CurrentChunk).EnemyHasDied(this);
        }

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

        public void AttackExpired(AttackManager attackManager)
        {
            Immunities.Remove(attackManager);
        }

        public override void RecieveExp(int exp)
        {

        }

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

        public Vector2 GetCenter()
        {
            return (Vector2)MonoBehaviour.transform.position + new Vector2(0, Configuration.ENEMY_CONFIGURATIONS[EnemyType].Height / 2f);
        }



        public void Immigrate(ChunkIndex newHome)
        {
            CurrentChunk = newHome;
            //GameManager.Singleton.World.GetChunk(newHome).RecieveImmigrantEnemy(this);
        }
    }
}