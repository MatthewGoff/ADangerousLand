using System.Collections.Generic;
using UnityEngine;

public class Prefabs
{
    public static Dictionary<Terrain, GameObject> TERRAIN_PREFABS { get; private set; }
    public static GameObject FOG_PREFAB { get; private set; }
    public static GameObject PLAYER_PREFAB { get; private set; }
    public static GameObject ENEMY_PREFAB { get; private set; }
    public static GameObject BLACK_HIGHLIGHT_PREFAB { get; private set; }
    public static GameObject CAMERA_PREFAB { get; private set; }


    public static void LoadPrefabs()
    {
        TERRAIN_PREFABS = new Dictionary<Terrain, GameObject>
        {
            { Terrain.Grass,    (GameObject)Resources.Load("prefabs/Grass", typeof(GameObject)) },
            { Terrain.Tree,     (GameObject)Resources.Load("prefabs/Tree", typeof(GameObject)) },
            { Terrain.Mountain, (GameObject)Resources.Load("prefabs/Mountain", typeof(GameObject)) },
            { Terrain.Sand,     (GameObject)Resources.Load("prefabs/Sand", typeof(GameObject)) },
            { Terrain.Ocean,    (GameObject)Resources.Load("prefabs/Ocean", typeof(GameObject)) },
            { Terrain.River,    (GameObject)Resources.Load("prefabs/River", typeof(GameObject)) }
        };

        FOG_PREFAB =             (GameObject)Resources.Load("prefabs/Fog", typeof(GameObject));
        PLAYER_PREFAB =          (GameObject)Resources.Load("prefabs/Player", typeof(GameObject));
        ENEMY_PREFAB =           (GameObject)Resources.Load("prefabs/Enemy", typeof(GameObject));
        BLACK_HIGHLIGHT_PREFAB = (GameObject)Resources.Load("prefabs/BlackHighlight", typeof(GameObject));
        CAMERA_PREFAB =          (GameObject)Resources.Load("prefabs/PlayerCamera", typeof(GameObject));
    }
}
