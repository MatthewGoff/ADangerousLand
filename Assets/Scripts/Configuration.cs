using System.Collections.Generic;

public class Configuration
{
    public static readonly Dictionary<TerrainType, float> MOVEMENT_MULTIPLIERS = new Dictionary<TerrainType, float>()
    {
        { TerrainType.GrassShort, 1f },
        { TerrainType.GrassMedium, 1f},
        { TerrainType.GrassTall, 1f},
        { TerrainType.Sand, 1f},
        { TerrainType.River, 0.5f},
        { TerrainType.Tree, 0.5f},
        { TerrainType.Ocean, 0.5f},
        { TerrainType.Mountain, 1f},
    };

    //World
    public static readonly int TREADMILL_RADIUS = 59;
    public static readonly int TREADMILL_UPDATE_MARGIN = 5;

    //Player
    public static readonly float DEFAULT_MOVE_SPEED = 5f;

    //Fog
    public static float FOG_OUTER_RADIUS = 11;
    public static float FOG_INNER_RADIUS = 7;
    public static readonly int FOG_UPDATE_MARGIN = 5;
    public static readonly float FOG_ALPHA = 0.5f;
}