using System.Collections.Generic;

public class Configuration
{
    public static readonly Dictionary<TerrainTypeEnum, float> MOVEMENT_MULTIPLIERS = new Dictionary<TerrainTypeEnum, float>()
    {
        { TerrainTypeEnum.Grass, 1f},
        { TerrainTypeEnum.Sand, 1f},
        { TerrainTypeEnum.Tree, 0.7f},
        { TerrainTypeEnum.River, 0.5f},
        { TerrainTypeEnum.Ocean, 0.3f},
        { TerrainTypeEnum.Mountain, 1f},
    };

    //World
    public static readonly int TREADMILL_WIDTH = 62;
    public static readonly int TREADMIL_HEIGHT = 62;
    public static readonly int TREADMILL_UPDATE_MARGIN = 1;
    public static readonly int CHUNK_SIZE = 32;

    //Player
    public static readonly float DEFAULT_MOVE_SPEED = 5f;

    //Fog
    public static float FOG_OUTER_RADIUS = 11;
    public static float FOG_INNER_RADIUS = 7;
    public static readonly float FOG_ALPHA = 0.5f;

    //Enemies
    public static readonly int ENEMIES_PER_CHUNK = 4;

    //Damage Numbers
    public static readonly int DAMAGE_NUMBERS_HEIGHT = 100;
    public static readonly float DAMAGE_NUMBERS_DURATION = 2f;
}