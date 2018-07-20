using UnityEngine;

public class FogController
{
    public static int OUTER_RADIUS = 11;
    public static readonly int FOG_UPDATE_MARGIN = 5;
    public static int INNER_RADIUS = 7;
    private static readonly float FOG_ALPHA = 0.5f;

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
        if (distanceToPlayers < OUTER_RADIUS)
        {
            if (!Exposed)
            {
                Exposed = true;
            }
            float alpha = FOG_ALPHA * (distanceToPlayers - INNER_RADIUS) / (OUTER_RADIUS - INNER_RADIUS);
            FogRenderer.color = new Color(0, 0, 0, alpha);
        }
        else if (Exposed)
        {
            FogRenderer.color = new Color(0, 0, 0, FOG_ALPHA);
        }
    }

    private void UpdateRenderer()
    {
        FogRenderer.color = new Color(0, 0, 0, FOG_ALPHA);
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
        FogGameObject = GameObject.Instantiate(Prefabs.FOG_PREFAB, new Vector3(MyTile.Location.X, MyTile.Location.Y, 0), Quaternion.identity);
        FogRenderer = FogGameObject.GetComponent<SpriteRenderer>();
        if (Exposed)
        {
            FogRenderer.color = new Color(0, 0, 0, FOG_ALPHA);
        }
    }
}