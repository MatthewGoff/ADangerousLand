using System.Collections.Generic;
using UnityEngine;

public class Prefabs
{
    public static Dictionary<TerrainType, (float, GameObject)[]> TERRAIN_PREFABS { get; private set; }
    public static GameObject FOG_PREFAB { get; private set; }
    public static GameObject PLAYER_PREFAB { get; private set; }
    public static GameObject ENEMY_PREFAB { get; private set; }
    public static GameObject BLACK_HIGHLIGHT_PREFAB { get; private set; }
    public static GameObject CAMERA_PREFAB { get; private set; }


    public static void LoadPrefabs()
    {
        TERRAIN_PREFABS = new Dictionary<TerrainType, (float, GameObject)[]>();
        TERRAIN_PREFABS.Add(TerrainType.GrassShort,
            new (float, GameObject)[]
            {
                (1f, (GameObject)Resources.Load("prefabs/terrain/GrassShort", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainType.Sand,
            new(float, GameObject)[]
            {
                (1f, (GameObject)Resources.Load("prefabs/terrain/Sand", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainType.River,
            new(float, GameObject)[]
            {
                (1f, (GameObject)Resources.Load("prefabs/terrain/River", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainType.Ocean,
            new(float, GameObject)[]
            {
                (0.1f, (GameObject)Resources.Load("prefabs/terrain/Ocean1", typeof(GameObject))),
                (0.9f, (GameObject)Resources.Load("prefabs/terrain/Ocean2", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainType.GrassMedium,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassMedium1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassMedium2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassMedium3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassMedium4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassMedium5", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainType.GrassTall,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassTall1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassTall2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassTall3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassTall4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/GrassTall5", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainType.Mountain,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Mountain1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Mountain2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Mountain3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Mountain4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Mountain5", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainType.Tree,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Tree1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Tree2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Tree3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Tree4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("prefabs/terrain/Tree5", typeof(GameObject)))
            }
        );

        FOG_PREFAB =             (GameObject)Resources.Load("prefabs/Fog", typeof(GameObject));
        PLAYER_PREFAB =          (GameObject)Resources.Load("prefabs/Player", typeof(GameObject));
        ENEMY_PREFAB =           (GameObject)Resources.Load("prefabs/Enemy", typeof(GameObject));
        BLACK_HIGHLIGHT_PREFAB = (GameObject)Resources.Load("prefabs/BlackHighlight", typeof(GameObject));
        CAMERA_PREFAB =          (GameObject)Resources.Load("prefabs/PlayerCamera", typeof(GameObject));
    }

    public static GameObject GetRandomTerrainVarient(TerrainType terrain)
    {
        (float prob, GameObject prefab)[] varients = TERRAIN_PREFABS[terrain];

        float random = Random.Range(0f, 1f);
        float accumulator = 0f;
        for (int x=0; x<varients.GetLength(0); x++)
        {
            accumulator += varients[x].prob;
            if (random<=accumulator)
            {
                return varients[x].prefab;
            }
        }
        return varients[0].prefab;
    }
}
