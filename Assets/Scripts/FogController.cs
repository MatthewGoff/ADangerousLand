using UnityEngine;

public class FogController
{
    private Tile MyTile;
    private GameObject FogGameObject;
    private SpriteRenderer FogRenderer;
    private bool Exposed = false;

    public FogController(Tile tile)
    {
        MyTile = tile;
    }

    public void Update(float distanceToPlayers)
    {
        if (!MyTile.Awake)
        {
            return;
        }
        if (distanceToPlayers < Configuration.FOG_OUTER_RADIUS)
        {
            if (!Exposed)
            {
                Exposed = true;
            }
            float alpha = Configuration.FOG_ALPHA * (distanceToPlayers - Configuration.FOG_INNER_RADIUS) / (Configuration.FOG_OUTER_RADIUS - Configuration.FOG_INNER_RADIUS);
            FogRenderer.color = new Color(0, 0, 0, alpha);
        }
        else if (Exposed)
        {
            FogRenderer.color = new Color(0, 0, 0, Configuration.FOG_ALPHA);
        }
    }

    private void UpdateRenderer()
    {
        FogRenderer.color = new Color(0, 0, 0, Configuration.FOG_ALPHA);
    }

    public void WakeUp()
    {
        CreateGameObject();
    }

    public void Sleep()
    {
        GameObject.Destroy(FogGameObject);
    }

    private void CreateGameObject()
    {
        FogGameObject = GameObject.Instantiate(Prefabs.FOG_PREFAB, new Vector3(MyTile.WorldLocation.X, MyTile.WorldLocation.Y, 0), Quaternion.identity);
        FogRenderer = FogGameObject.GetComponent<SpriteRenderer>();
        if (Exposed)
        {
            FogRenderer.color = new Color(0, 0, 0, Configuration.FOG_ALPHA);
        }
    }
}