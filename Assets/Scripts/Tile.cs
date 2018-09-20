using UnityEngine;
using MessagePack;

[MessagePackObject]
public class Tile
{
    [IgnoreMember] public bool Awake = false;

    [IgnoreMember] private GameObject FogGameObject;
    [IgnoreMember] private SpriteRenderer FogRenderer;
    [IgnoreMember] private GameObject TerrainGameObject;
    [IgnoreMember] private float LastFrameFog = -1f;

    [Key(0)] public TerrainType TerrainType { get; set; }
    [Key(1)] public bool Exposed = false;
    [Key(2)] public readonly WorldLocation WorldLocation;
    
    public Tile(WorldLocation worldLocation)
    {
        WorldLocation = worldLocation;
    }

    [SerializationConstructor]
    public Tile(TerrainType terrainType, bool exposed, WorldLocation worldLocation)
    {
        TerrainType = terrainType;
        Exposed = exposed;
        WorldLocation = worldLocation;
    }

    public void WakeUp()
    {
        if (!Awake)
        {
            Awake = true;
            CreateGameObjects();
        }
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

    public void FixedUpdate()
    {
        bool onTreadmill = GameManager.Singleton.World.OnTreadmill(WorldLocation);
        if (onTreadmill && !Awake)
        {
            WakeUp();
        }
        else if (!onTreadmill && Awake)
        {
            Sleep();
        }
    }

    public void Update()
    {
        if (!Awake)
        {
            return;
        }
        
        Vector2 playerPosition = GameManager.Singleton.World.PlayerManager.GetPlayerPosition();
        float fogInnerRadius = GameManager.Singleton.World.PlayerManager.GetSightRadiusNear();
        float fogOuterRadius = GameManager.Singleton.World.PlayerManager.GetSightRadiusFar();
        float distanceToPlayer = Util.EuclidianDistance(playerPosition, WorldLocation.Tuple);

        if (distanceToPlayer < fogOuterRadius)
        {
            if (!Exposed)
            {
                Exposed = true;
            }
            float alpha = Configuration.FOG_ALPHA * (distanceToPlayer - fogInnerRadius) / (fogOuterRadius - fogInnerRadius);
            if (alpha != LastFrameFog)
            {
                FogRenderer.color = new Color(0, 0, 0, alpha);
                LastFrameFog = alpha;
            }
        }
        else if (Exposed)
        {
            float alpha = Configuration.FOG_ALPHA;
            if (alpha != LastFrameFog)
            {
                FogRenderer.color = new Color(0, 0, 0, alpha);
                LastFrameFog = alpha;
            }
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
    }
}
