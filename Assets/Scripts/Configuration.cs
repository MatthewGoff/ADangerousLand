using System.Collections.Generic;
using UnityEngine;

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
        if (level == 0) { return 0; }
        else if (level == 1) { return 0; }
        else if (level == 2) { return 1; }
        else if (level == 3) { return 5; }
        else if (level == 4) { return 12; }
        else if (level == 5) { return 25; }
        else if (level == 6) { return 50; }
        else if (level == 7) { return 100; }
        else if (level == 8) { return 400; }
        else { return 200+400*(int)Mathf.Pow(level-8, 2); }
    }
    public static int GetLevelSkillPoints(int level)
    {
        if (level == 0) { return 0; }
        else if (level == 1) { return 0; }
        else if (level == 2) { return 1; }
        else if (level == 3) { return 2; }
        else if (level == 4) { return 3; }
        else if (level == 5) { return 4; }
        else if (level == 6) { return 5; }
        else if (level == 7) { return 7; }
        else if (level == 8) { return 10; }
        else { return 15; }
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
            Damage = 0.1f,
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
            ExperienceReward = 2,
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
        {EnemyType.SmallBlueSlime, new EnemyConfiguration(){
            PrefabLocation = "SmallBlueSlime/SmallBlueSlimePrefab",
            AIType = AIType.Basic,
            MaxHealth = 8,
            MoveSpeed = 3.5f,
            ExperienceReward = 3,
            Damage = 0.3f,
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
            MaxHealth = 10,
            MoveSpeed = 3.5f,
            ExperienceReward = 5,
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
            MaxHealth = 15,
            MoveSpeed = 3.5f,
            ExperienceReward = 7,
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
            MaxHealth = 20,
            MoveSpeed = 3.5f,
            ExperienceReward = 10,
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
            MaxHealth = 30,
            MoveSpeed = 3.5f,
            ExperienceReward = 20,
            Damage = 4f,
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
            MaxHealth = 50,
            MoveSpeed = 5.5f,
            ExperienceReward = 35,
            Damage = 2.5f,
            AttackSpeed = 2f,
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
            MoveSpeed = 1f,
            ExperienceReward = 200,
            Damage = 5f,
            AttackSpeed = 0.5f,
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
                (0.5f, EnemyType.SmallRedSlime),
                (0.5f, EnemyType.SmallGreenSlime),
            };
        }
        else if (dangerRating == 3)
        {
            return new (float, EnemyType)[]
            {
                (0.33f, EnemyType.SmallRedSlime),
                (0.33f, EnemyType.SmallGreenSlime),
                (0.33f, EnemyType.SmallBlueSlime),
            };
        }
        else if (dangerRating == 4)
        {
            return new (float, EnemyType)[]
            {
                (0.25f, EnemyType.SmallRedSlime),
                (0.25f, EnemyType.SmallGreenSlime),
                (0.25f, EnemyType.SmallBlueSlime),
                (0.25f, EnemyType.RedSlime),
            };
        }
        else if (dangerRating == 5)
        {
            return new (float, EnemyType)[]
            {
                (0.15f, EnemyType.SmallRedSlime),
                (0.15f, EnemyType.SmallGreenSlime),
                (0.15f, EnemyType.SmallBlueSlime),
                (0.35f, EnemyType.RedSlime),
                (0.20f, EnemyType.GreenSlime),
            };
        }
        else if (dangerRating == 6)
        {
            return new (float, EnemyType)[]
            {
                (0.10f, EnemyType.SmallRedSlime),
                (0.10f, EnemyType.SmallGreenSlime),
                (0.10f, EnemyType.SmallBlueSlime),
                (0.25f, EnemyType.RedSlime),
                (0.25f, EnemyType.GreenSlime),
                (0.20f, EnemyType.BlueSlime),

            };
        }
        else if (dangerRating == 7)
        {
            return new (float, EnemyType)[]
            {
                (0.05f, EnemyType.SmallRedSlime),
                (0.05f, EnemyType.SmallGreenSlime),
                (0.05f, EnemyType.SmallBlueSlime),
                (0.20f, EnemyType.RedSlime),
                (0.20f, EnemyType.GreenSlime),
                (0.20f, EnemyType.BlueSlime),
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
                (0.15f, EnemyType.RedSlime),
                (0.15f, EnemyType.GreenSlime),
                (0.15f, EnemyType.BlueSlime),
                (0.20f, EnemyType.Bat),
                (0.29f, EnemyType.Werewolf)
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
                (0.30f, EnemyType.Bat),
                (0.30f, EnemyType.Werewolf),
                (0.4f, EnemyType.Dragon),
            };
        }
        else if (dangerRating > 9 )
        {
            return new (float, EnemyType)[]
            {
                (0.02f, EnemyType.SmallRedSlime),
                (0.02f, EnemyType.SmallGreenSlime),
                (0.02f, EnemyType.SmallBlueSlime),
                (0.10f, EnemyType.RedSlime),
                (0.10f, EnemyType.GreenSlime),
                (0.10f, EnemyType.BlueSlime),
                (0.30f, EnemyType.Bat),
                (0.34f, EnemyType.Werewolf),
                (0.4f, EnemyType.Dragon),
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

}