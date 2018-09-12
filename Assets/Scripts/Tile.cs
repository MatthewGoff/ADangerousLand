using UnityEngine;

public class Tile {

    public TerrainType TerrainType { get; set; }
    public bool Awake { get; private set; } = false;

    private GameObject FogGameObject;
    private SpriteRenderer FogRenderer;
    private float LastFrameFog = -1f;
    private bool Exposed = false;
    private GameObject TerrainGameObject;
    private readonly WorldLocation WorldLocation;
    
    public Tile(WorldLocation worldLocation)
    {
        WorldLocation = worldLocation;
    }

    public void WakeUp()
    {
        Awake = true;
        CreateGameObjects();
    }

    public void Sleep()
    {
        if (Awake)
        {
            Awake = false;
            GameObject.Destroy(TerrainGameObject);
            GameObject.Destroy(FogGameObject);
            GameManager.Singleton.GameObjectCount -= 2;
        }
    }

    public void Update(Treadmill treadmill)
    {
        bool onTreadmill = treadmill.OnTreadmill(WorldLocation.Tuple);
        if (onTreadmill && !Awake)
        {
            WakeUp();
        }
        else if (!onTreadmill && Awake)
        {
            Sleep();
        }
        float distanceToPlayer = Util.EuclidianDistance(treadmill.Center, WorldLocation.Tuple);
        UpdateFog(distanceToPlayer);
    }

    public void UpdateFog(float distanceToPlayer)
    {
        if (!Awake)
        {
            return;
        }
        if (distanceToPlayer < Configuration.FOG_OUTER_RADIUS)
        {
            if (!Exposed)
            {
                Exposed = true;
            }
            float alpha = Configuration.FOG_ALPHA * (distanceToPlayer - Configuration.FOG_INNER_RADIUS) / (Configuration.FOG_OUTER_RADIUS - Configuration.FOG_INNER_RADIUS);
            if (alpha != LastFrameFog)
            {
                FogRenderer.color = new Color(0, 0, 0, alpha);
                LastFrameFog = alpha;
            }
        }
        else if (Exposed)
        {
            FogRenderer.color = new Color(0, 0, 0, Configuration.FOG_ALPHA);
        }
    }

    public void CreateGameObjects()
    {
        GameObject prefab = Prefabs.GetRandomTerrainVarient(TerrainType.Subtype);
        TerrainGameObject = GameObject.Instantiate(prefab, new Vector3(WorldLocation.X, WorldLocation.Y, 0), Quaternion.identity);

        FogGameObject = GameObject.Instantiate(Prefabs.FOG_PREFAB, new Vector3(WorldLocation.X, WorldLocation.Y, 0), Quaternion.identity);
        FogRenderer = FogGameObject.GetComponent<SpriteRenderer>();
        if (Exposed)
        {
            FogRenderer.color = new Color(0, 0, 0, Configuration.FOG_ALPHA);
        }

        GameManager.Singleton.GameObjectCount += 2;
    }
}
