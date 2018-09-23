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
        {EnemyType.Bat, new EnemyConfiguration(){
            PrefabLocation = "Bat/BatPrefab",
            AIType = AIType.Basic,
            MaxHealth = 2,
            MoveSpeed = 3.5f,
            ExperienceReward = 1,
            Damage = 0.1f,
            AttackSpeed = 1f,
            Aoe = 1.5f,
            AgroDistance = 10f,
            DeAgroDistance = 15f,
            MinAgroDuration = 3f,
            Width = 2f,
            Height = 1f,
            AttackOrigin = new Vector2(0, 0.5f),
        }},
        {EnemyType.Soldier, new EnemyConfiguration(){
            PrefabLocation = "Soldier/SoldierPrefab",
            AIType = AIType.Basic,
            MaxHealth = 5,
            MoveSpeed = 3.5f,
            ExperienceReward = 2,
            Damage = 0.5f,
            AttackSpeed = 1f,
            Aoe = 1.5f,
            AgroDistance = 10f,
            DeAgroDistance = 15f,
            MinAgroDuration = 3f,
            Width = 1f,
            Height = 1f,
            AttackOrigin = new Vector2(0, 0.5f),
        }},
        {EnemyType.Werewolf, new EnemyConfiguration(){
            PrefabLocation = "Werewolf/WerewolfPrefab",
            AIType = AIType.Basic,
            MaxHealth = 50,
            MoveSpeed = 5.5f,
            ExperienceReward = 10,
            Damage = 0.3f,
            AttackSpeed = 5f,
            Aoe = 1.5f,
            AgroDistance = 10f,
            DeAgroDistance = 15f,
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
            AgroDistance = 10f,
            DeAgroDistance = 15f,
            MinAgroDuration = 3f,
            Width = 3f,
            Height = 3f,
            AttackOrigin = new Vector2(0, 1.5f),
        }},
    };

    // SpawnProbabilities maps a difficulty rating to an array of spawn probabilities
    public static readonly Dictionary<int, (float, EnemyType)[]> SPAWN_PROBABILITIES = new Dictionary<int, (float, EnemyType)[]>()
    {
        {0, new (float, EnemyType)[]
        {
            (1f, EnemyType.Bat),
        }},
        {1, new (float, EnemyType)[]
        {
            (1f, EnemyType.Bat),
        }},
        {2, new (float, EnemyType)[]
        {
            (0.9f, EnemyType.Bat),
            (0.1f, EnemyType.Soldier),
        }},
        {3, new (float, EnemyType)[]
        {
            (0.8f, EnemyType.Bat),
            (0.2f, EnemyType.Soldier),
        }},
        {4, new (float, EnemyType)[]
        {
            (0.6f, EnemyType.Bat),
            (0.3f, EnemyType.Soldier),
            (0.1f, EnemyType.Werewolf)
        }},
        {5, new (float, EnemyType)[]
        {
            (0.4f, EnemyType.Bat),
            (0.4f, EnemyType.Soldier),
            (0.2f, EnemyType.Werewolf)
        }},
        {6, new (float, EnemyType)[]
        {
            (0.3f, EnemyType.Bat),
            (0.4f, EnemyType.Soldier),
            (0.3f, EnemyType.Werewolf)
        }},
        {7, new (float, EnemyType)[]
        {
            (0.2f, EnemyType.Bat),
            (0.3f, EnemyType.Soldier),
            (0.5f, EnemyType.Werewolf)
        }},
        {8, new (float, EnemyType)[]
        {
            (0.2f, EnemyType.Bat),
            (0.2f, EnemyType.Soldier),
            (0.6f, EnemyType.Werewolf)
        }},
        {9, new (float, EnemyType)[]
        {
            (0.1f, EnemyType.Bat),
            (0.2f, EnemyType.Soldier),
            (0.6f, EnemyType.Werewolf),
            (0.1f, EnemyType.Dragon),
        }},
    };

}