using System.Collections.Generic;

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
    public static readonly int TREADMILL_WIDTH = 62;
    public static readonly int TREADMIL_HEIGHT = 62;
    public static readonly int TREADMILL_UPDATE_MARGIN = 1;
    public static readonly int CHUNK_SIZE = 32;

    // Fog
    public static readonly float FOG_ALPHA = 0.5f;

    // Damage Numbers
    public static readonly int DAMAGE_NUMBERS_HEIGHT = 100;
    public static readonly float DAMAGE_NUMBERS_DURATION = 2f;

    // Display
    public static readonly int PIXELS_PER_UNIT = 32;
    public static readonly float DEATH_DURATION = 3f;

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
        else if (level == 8) { return 200; }
        else { return 200+200*(level-8); }
    }
    public static int GetLevelSkillPoints(int level)
    {
        if (level == 0) { return 0; }
        else if (level == 1) { return 0; }
        else if (level == 2) { return 1; }
        else if (level == 3) { return 2; }
        else if (level == 4) { return 4; }
        else if (level == 5) { return 8; }
        else if (level == 6) { return 15; }
        else if (level == 7) { return 25; }
        else if (level == 8) { return 50; }
        else { return 50; }
    }

    // Enemies
    public static readonly Dictionary<EnemyType, EnemyConfiguration> ENEMY_CONFIGURATIONS = new Dictionary<EnemyType, EnemyConfiguration>()
    {
        {EnemyType.Soldier, new EnemyConfiguration(){
            SpriteLocation = "Enemies/SoldierSprite",
            AIType = AIType.Basic,
            MaxHealth = 5,
            MoveSpeed = 3.5f,
            ExperienceReward = 1,
            Damage = 0.5f,
            AttackSpeed = 1f,
            Aoe = 1.5f
        }}
    };

}