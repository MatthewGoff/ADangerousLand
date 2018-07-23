using UnityEngine;

public class EnemyManager
{
    public bool Awake;
    private Vector3 Position;
    private GameObject GameObject;

    public EnemyManager(WorldLocation worldLocation)
    {
        Position = new Vector3(worldLocation.X, worldLocation.Y);
    }


}