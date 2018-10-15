using System.Collections.Generic;
using UnityEngine;
using ADL.World;
using ADL.Combat.Enemies;
using ADL.Combat.Enemies.AI;
using ADL.Util;

namespace ADL.Core
{
    public class Configuration
    {

        // Movement Multipliers
        public static readonly Dictionary<TerrainTypeEnum, float> MOVEMENT_MULTIPLIERS = new Dictionary<TerrainTypeEnum, float>()
    {
        { TerrainTypeEnum.Grass, 1f},
        { TerrainTypeEnum.Sand, 1f},
        { TerrainTypeEnum.Tree, 0.7f},
        { TerrainTypeEnum.River, 0.5f},
        { TerrainTypeEnum.Ocean, 0.3f},
        { TerrainTypeEnum.Mountain, 1f},
    };

        // World
        public static readonly int TREADMILL_RADIUS = 32;
        public static readonly int CHUNK_SIZE = 32;

        // Fog
        public static readonly float FOG_ALPHA = 0.5f;

        // Damage Numbers
        public static readonly int DAMAGE_NUMBERS_HEIGHT = 100;
        public static readonly float DAMAGE_NUMBERS_DURATION = 2f;

        // Display
        public static readonly int PIXELS_PER_UNIT = 32;
        public static readonly float DEATH_DURATION = 3f;
        public static readonly int PLAYER_TRAIL_SPRITES = 10;
        public static readonly float PLAYER_TRAIL_PERIOD = 1 / 60f;

        // Combat
        public static readonly float KNOCKBACK_SPEED = 5f;

        // Levels
        public static int GetLevelExperience(int level)
        {
            if (level <= 1)
            {
                return 0;
            }
            else
            {
                return Mathf.FloorToInt(Mathf.Pow(1.5f, level - 1));
            }
        }
        public static int GetLevelSkillPoints(int level)
        {
            return Mathf.CeilToInt(level / 3f);
        }

        // Enemies
        public static readonly Dictionary<EnemyType, EnemyConfiguration> ENEMY_CONFIGURATIONS = new Dictionary<EnemyType, EnemyConfiguration>()
    {
        {EnemyType.SmallRedSlime, new EnemyConfiguration(){
            PrefabLocation = "SmallRedSlime/SmallRedSlimePrefab",
            AIType = AIType.Basic,
            MaxHealth = 3,
            MoveSpeed = 3.5f,
            ExperienceReward = 1,
            Damage = 0.2f,
            AttackSpeed = 1f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = .5f,
            Height = .5f,
            AttackOrigin = new Vector2(0, 0.25f),
        }},
        {EnemyType.SmallGreenSlime, new EnemyConfiguration(){
            PrefabLocation = "SmallGreenSlime/SmallGreenSlimePrefab",
            AIType = AIType.Basic,
            MaxHealth = 5,
            MoveSpeed = 3.5f,
            ExperienceReward = 3,
            Damage = 0.4f,
            AttackSpeed = 1f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = .5f,
            Height = .5f,
            AttackOrigin = new Vector2(0, 0.25f),
        }},
        {EnemyType.SmallBlueSlime, new EnemyConfiguration(){
            PrefabLocation = "SmallBlueSlime/SmallBlueSlimePrefab",
            AIType = AIType.Basic,
            MaxHealth = 15,
            MoveSpeed = 3.5f,
            ExperienceReward = 9,
            Damage = 0.6f,
            AttackSpeed = 1f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = .5f,
            Height = .5f,
            AttackOrigin = new Vector2(0, 0.25f),
        }},
        {EnemyType.RedSlime, new EnemyConfiguration(){
            PrefabLocation = "RedSlime/RedSlimePrefab",
            AIType = AIType.Basic,
            MaxHealth = 30,
            MoveSpeed = 4.5f,
            ExperienceReward = 27,
            Damage = 1f,
            AttackSpeed = .5f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = 1f,
            Height = 1f,
            AttackOrigin = new Vector2(0, 0.5f),
        }},
        {EnemyType.GreenSlime, new EnemyConfiguration(){
            PrefabLocation = "GreenSlime/GreenSlimePrefab",
            AIType = AIType.Basic,
            MaxHealth = 60,
            MoveSpeed = 4.5f,
            ExperienceReward = 81,
            Damage = 2f,
            AttackSpeed = .5f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = 1f,
            Height = 1f,
            AttackOrigin = new Vector2(0, 0.5f),
        }},
        {EnemyType.BlueSlime, new EnemyConfiguration(){
            PrefabLocation = "BlueSlime/BlueSlimePrefab",
            AIType = AIType.Basic,
            MaxHealth = 80,
            MoveSpeed = 4.5f,
            ExperienceReward = 243,
            Damage = 3f,
            AttackSpeed = .5f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = 1f,
            Height = 1f,
            AttackOrigin = new Vector2(0, 0.5f),
        }},
        {EnemyType.Bat, new EnemyConfiguration(){
            PrefabLocation = "Bat/BatPrefab",
            AIType = AIType.Basic,
            MaxHealth = 100,
            MoveSpeed = 6f,
            ExperienceReward = 729,
            Damage = 5f,
            AttackSpeed = 1f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = 2f,
            Height = 1f,
            AttackOrigin = new Vector2(0, 0.5f),
        }},
        {EnemyType.Werewolf, new EnemyConfiguration(){
            PrefabLocation = "Werewolf/WerewolfPrefab",
            AIType = AIType.Basic,
            MaxHealth = 140,
            MoveSpeed = 8f,
            ExperienceReward = 2187,
            Damage = 8f,
            AttackSpeed = 1.5f,
            Aoe = 2f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = 1f,
            Height = 1.5f,
            AttackOrigin = new Vector2(0, 0.75f),
        }},
        {EnemyType.Dragon, new EnemyConfiguration(){
            PrefabLocation = "Dragon/DragonPrefab",
            AIType = AIType.Basic,
            MaxHealth = 1000,
            MoveSpeed = 2.5f,
            ExperienceReward = 6561,
            Damage = 25f,
            AttackSpeed = 0.75f,
            Aoe = 10f,
            AgroDistance = 15f,
            DeAgroDistance = 20f,
            MinAgroDuration = 3f,
            Width = 3f,
            Height = 3f,
            AttackOrigin = new Vector2(0, 1.5f),
        }},
    };
        public static int MAXIMUM_ENEMIES(int dangerRating)
        {
            if (dangerRating == 0)
            {
                return 0;
            }
            else
            {
                return Mathf.CeilToInt(dangerRating / 2f) + 1;
            }
        }

        // SpawnProbabilities maps a danger rating to an array of spawn probabilities
        public static (float, EnemyType)[] SPAWN_PROBABILITIES(int dangerRating)
        {
            if (dangerRating == 0)
            {
                return new (float, EnemyType)[]
                {
                (1f, EnemyType.SmallRedSlime),
                };
            }
            else if (dangerRating == 1)
            {
                return new (float, EnemyType)[]
                {
                (1f, EnemyType.SmallRedSlime),
                };
            }
            else if (dangerRating == 2)
            {
                return new (float, EnemyType)[]
                {
                (0.7f, EnemyType.SmallRedSlime),
                (0.3f, EnemyType.SmallGreenSlime),
                };
            }
            else if (dangerRating == 3)
            {
                return new (float, EnemyType)[]
                {
                (0.50f, EnemyType.SmallRedSlime),
                (0.30f, EnemyType.SmallGreenSlime),
                (0.20f, EnemyType.SmallBlueSlime),
                };
            }
            else if (dangerRating == 4)
            {
                return new (float, EnemyType)[]
                {
                (0.30f, EnemyType.SmallRedSlime),
                (0.30f, EnemyType.SmallGreenSlime),
                (0.20f, EnemyType.SmallBlueSlime),
                (0.20f, EnemyType.RedSlime),
                };
            }
            else if (dangerRating == 5)
            {
                return new (float, EnemyType)[]
                {
                (0.20f, EnemyType.SmallRedSlime),
                (0.20f, EnemyType.SmallGreenSlime),
                (0.15f, EnemyType.SmallBlueSlime),
                (0.30f, EnemyType.RedSlime),
                (0.15f, EnemyType.GreenSlime),
                };
            }
            else if (dangerRating == 6)
            {
                return new (float, EnemyType)[]
                {
                (0.10f, EnemyType.SmallRedSlime),
                (0.10f, EnemyType.SmallGreenSlime),
                (0.10f, EnemyType.SmallBlueSlime),
                (0.30f, EnemyType.RedSlime),
                (0.30f, EnemyType.GreenSlime),
                (0.10f, EnemyType.BlueSlime),

                };
            }
            else if (dangerRating == 7)
            {
                return new (float, EnemyType)[]
                {
                (0.05f, EnemyType.SmallRedSlime),
                (0.05f, EnemyType.SmallGreenSlime),
                (0.05f, EnemyType.SmallBlueSlime),
                (0.25f, EnemyType.RedSlime),
                (0.25f, EnemyType.GreenSlime),
                (0.10f, EnemyType.BlueSlime),
                (0.25f, EnemyType.Bat),
                };
            }
            else if (dangerRating == 8)
            {
                return new (float, EnemyType)[]
                {
                (0.02f, EnemyType.SmallRedSlime),
                (0.02f, EnemyType.SmallGreenSlime),
                (0.02f, EnemyType.SmallBlueSlime),
                (0.20f, EnemyType.RedSlime),
                (0.20f, EnemyType.GreenSlime),
                (0.10f, EnemyType.BlueSlime),
                (0.29f, EnemyType.Bat),
                (0.15f, EnemyType.Werewolf)
                };
            }
            else if (dangerRating == 9)
            {
                return new (float, EnemyType)[]
                {
                (0.02f, EnemyType.SmallRedSlime),
                (0.02f, EnemyType.SmallGreenSlime),
                (0.02f, EnemyType.SmallBlueSlime),
                (0.10f, EnemyType.RedSlime),
                (0.10f, EnemyType.GreenSlime),
                (0.10f, EnemyType.BlueSlime),
                (0.35f, EnemyType.Bat),
                (0.25f, EnemyType.Werewolf),
                (0.04f, EnemyType.Dragon),
                };
            }
            else if (dangerRating > 9)
            {
                return new (float, EnemyType)[]
                {
                (0.02f, EnemyType.SmallRedSlime),
                (0.02f, EnemyType.SmallGreenSlime),
                (0.02f, EnemyType.SmallBlueSlime),
                (0.10f, EnemyType.RedSlime),
                (0.10f, EnemyType.GreenSlime),
                (0.10f, EnemyType.BlueSlime),
                (0.35f, EnemyType.Bat),
                (0.25f, EnemyType.Werewolf),
                (0.04f, EnemyType.Dragon),
                };
            }
            else
            {
                return new (float, EnemyType)[]
                {
                (1f, EnemyType.SmallRedSlime),
                };
            }
        }

        // Player Attributes
        public static float PLAYER_ATTACK_DAMAGE(int pointsInvested)
        {
            BoundedLogisticFunction f = new BoundedLogisticFunction(10, 1, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_ATTACK_SPEED(int pointsInvested)
        {
            LogisticFunction f = new LogisticFunction(20, 1, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_MOVE_SPEED(int pointsInvested)
        {
            LogisticFunction f = new LogisticFunction(15, 5, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_SIGHT_RADIUS(int pointsInvested)
        {
            LogisticFunction f = new LogisticFunction(20, 7, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_PROJECTILE_DAMAGE(int pointsInvested)
        {
            LogisticFunction f = new LogisticFunction(1.0f, 0.5f, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_MAX_HEALTH(int pointsInvested)
        {
            BoundedLogisticFunction f = new BoundedLogisticFunction(30, 10, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_MAX_STAMINA(int pointsInvested)
        {
            BoundedLogisticFunction f = new BoundedLogisticFunction(30, 10, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_HEALTH_REGEN(int pointsInvested)
        {
            LogisticFunction f = new LogisticFunction(0.15f, 0.01f, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
        public static float PLAYER_STAMINA_REGEN(int pointsInvested)
        {
            LogisticFunction f = new LogisticFunction(2.5f, 1, 20, 0.9f);
            return f.Evaluate(pointsInvested);
        }
    }
}