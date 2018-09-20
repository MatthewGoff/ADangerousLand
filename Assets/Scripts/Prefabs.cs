using System.Collections.Generic;
using UnityEngine;

public class Prefabs
{
    public static Dictionary<TerrainSubtypeEnum, (float, GameObject)[]> TERRAIN_PREFABS { get; private set; }
    public static GameObject FOG_PREFAB { get; private set; }
    public static GameObject PLAYER_PREFAB { get; private set; }
    public static GameObject ENEMY_PREFAB { get; private set; }
    public static GameObject BLACK_HIGHLIGHT_PREFAB { get; private set; }
    public static GameObject CAMERA_PREFAB { get; private set; }
    public static GameObject EVENT_SYSTEM_PREFAB { get; private set; }
    public static GameObject GAME_INFO_PREFAB { get; private set; }
    public static GameObject PAUSED_MENU_PREFAB { get; private set; }
    public static GameObject SPLASH_SCREEN_PREFAB { get; private set; }
    public static GameObject SLASH_PREFAB { get; private set; }
    public static GameObject BOLT_PREFAB { get; private set; }
    public static GameObject HEALTH_BAR_PREFAB { get; private set; }
    public static GameObject DAMAGE_NUMBER_CANVAS_PREFAB { get; private set; }
    public static GameObject DAMAGE_NUMBER_PREFAB { get; private set; }

    public static void LoadPrefabs()
    {
        TERRAIN_PREFABS = new Dictionary<TerrainSubtypeEnum, (float, GameObject)[]>();
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.Grass_Short,
            new (float, GameObject)[]
            {
                (1f, (GameObject)Resources.Load("Prefabs/Terrain/GrassShort", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.Sand,
            new(float, GameObject)[]
            {
                (1f, (GameObject)Resources.Load("Prefabs/Terrain/Sand", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.River,
            new(float, GameObject)[]
            {
                (1f, (GameObject)Resources.Load("Prefabs/Terrain/River", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.Ocean,
            new(float, GameObject)[]
            {
                (0.1f, (GameObject)Resources.Load("Prefabs/Terrain/Ocean1", typeof(GameObject))),
                (0.9f, (GameObject)Resources.Load("Prefabs/Terrain/Ocean2", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.Grass_Medium,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassMedium1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassMedium2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassMedium3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassMedium4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassMedium5", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.Grass_Tall,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassTall1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassTall2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassTall3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassTall4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/GrassTall5", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.Mountain,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Mountain1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Mountain2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Mountain3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Mountain4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Mountain5", typeof(GameObject)))
            }
        );
        TERRAIN_PREFABS.Add(TerrainSubtypeEnum.Tree,
            new(float, GameObject)[]
            {
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Tree1", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Tree2", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Tree3", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Tree4", typeof(GameObject))),
                (0.2f, (GameObject)Resources.Load("Prefabs/Terrain/Tree5", typeof(GameObject)))
            }
        );

        ENEMY_PREFAB =                (GameObject)Resources.Load("Prefabs/Enemies/Enemy", typeof(GameObject));
        FOG_PREFAB =                  (GameObject)Resources.Load("Prefabs/Fog", typeof(GameObject));
        PLAYER_PREFAB =               (GameObject)Resources.Load("Prefabs/Player", typeof(GameObject));
        BLACK_HIGHLIGHT_PREFAB =      (GameObject)Resources.Load("Prefabs/BlackHighlight", typeof(GameObject));
        CAMERA_PREFAB =               (GameObject)Resources.Load("Prefabs/PlayerCamera", typeof(GameObject));
        EVENT_SYSTEM_PREFAB =         (GameObject)Resources.Load("Prefabs/EventSystem");
        GAME_INFO_PREFAB =            (GameObject)Resources.Load("Prefabs/Canvases/GameInfo");
        PAUSED_MENU_PREFAB =          (GameObject)Resources.Load("Prefabs/Canvases/PausedMenu");
        SPLASH_SCREEN_PREFAB =        (GameObject)Resources.Load("Prefabs/Canvases/SplashScreen");
        SLASH_PREFAB =                (GameObject)Resources.Load("Prefabs/Slash");
        BOLT_PREFAB =                 (GameObject)Resources.Load("Prefabs/Bolt");
        HEALTH_BAR_PREFAB =           (GameObject)Resources.Load("Prefabs/HealthBar");
        DAMAGE_NUMBER_CANVAS_PREFAB = (GameObject)Resources.Load("Prefabs/DamageNumberCanvas");
        DAMAGE_NUMBER_PREFAB =        (GameObject)Resources.Load("Prefabs/DamageNumber");
    }

    public static GameObject GetRandomTerrainVarient(TerrainSubtypeEnum terrainType)
    {
        (float prob, GameObject prefab)[] varients = TERRAIN_PREFABS[terrainType];

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
