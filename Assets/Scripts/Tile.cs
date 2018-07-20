using UnityEngine;

public class Tile {

    public Terrain TerrainType { get; set; }
    private GameObject TerrainGameObject;
    //private GameObject BlackHighlightGameObject;
    public readonly (int X, int Y) Location;
    public bool Awake { get; private set; } = false;

    private FogController MyFogController;

    public Tile((int X, int Y) location)
    {
        Location = location;
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
        TerrainGameObject = GameObject.Instantiate(prefab, new Vector3(Location.X, Location.Y, 0), Quaternion.identity);
        //BlackHighlightGameObject = GameObject.Instantiate(Prefabs.BLACK_HIGHLIGHT_PREFAB, new Vector3(Location.X, Location.Y, 0), Quaternion.identity);
    }

    public float DistanceToPlayer(Vector2 playerPosition)
    {
        Vector2 location = new Vector3(Location.X, Location.Y);
        return (location - playerPosition).magnitude;
    }
}
