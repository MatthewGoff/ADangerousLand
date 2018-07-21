using UnityEngine;

public class Tile {

    public TerrainType TerrainType { get; set; }
    private GameObject TerrainGameObject;
    //private GameObject BlackHighlightGameObject;
    public readonly WorldLocation WorldLocation;
    public bool Awake { get; private set; } = false;

    private FogController MyFogController;

    public Tile(WorldLocation worldLocation)
    {
        WorldLocation = worldLocation;
        MyFogController = new FogController(this);
    }

    public void WakeUp()
    {
        Awake = true;
        CreateGameObject();
        MyFogController.WakeUp();
    }

    public void Sleep()
    {
        Awake = false;
        GameObject.Destroy(TerrainGameObject);
        //GameObject.Destroy(BlackHighlightGameObject);
        MyFogController.Sleep();
    }

    public void UpdateFog(float distanceToPlayer)
    {
        MyFogController.Update(distanceToPlayer);
    }

    public void CreateGameObject()
    {
        GameObject prefab = Prefabs.GetRandomTerrainVarient(TerrainType);
        TerrainGameObject = GameObject.Instantiate(prefab, new Vector3(WorldLocation.X, WorldLocation.Y, 0), Quaternion.identity);
        //BlackHighlightGameObject = GameObject.Instantiate(Prefabs.BLACK_HIGHLIGHT_PREFAB, new Vector3(Location.X, Location.Y, 0), Quaternion.identity);
    }

    public float EuclidianDistanceToPlayer(Vector2 playerPosition)
    {
        Vector2 location = new Vector3(WorldLocation.X, WorldLocation.Y);
        return (location - playerPosition).magnitude;
    }

    public float SquareDistanceToPlayer(Vector2 playerPosition)
    {
        float x = System.Math.Abs(WorldLocation.X - playerPosition.x);
        float y = System.Math.Abs(WorldLocation.Y - playerPosition.y);
        return System.Math.Max(x, y);
    }
}
