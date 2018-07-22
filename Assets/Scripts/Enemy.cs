using UnityEngine;

public class Enemy
{
    public bool Awake;
    private Vector3 Position;
    private GameObject GameObject;

    public Enemy(WorldLocation worldLocation)
    {
        Position = new Vector3(worldLocation.X, worldLocation.Y);
    }


}