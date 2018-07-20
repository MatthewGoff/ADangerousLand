using System.Collections.Generic;

public class Configuration
{
    public static readonly Dictionary<Terrain, float> MOVEMENT_MULTIPLIERS = new Dictionary<Terrain, float>()
    {
        { Terrain.GrassShort, 1f },
        { Terrain.GrassMedium, 1f},
        { Terrain.GrassTall, 1f},
        { Terrain.Sand, 1f},
        { Terrain.River, 0.5f},
        { Terrain.Tree, 0.5f},
        { Terrain.Ocean, 0.5f},
        { Terrain.Mountain, 1f},
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