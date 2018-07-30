using UnityEngine;

public class EnemyManager
{
    public bool Awake;
    public Vector2 Position { get; private set; }
    private EnemyMonoBehaviour MonoBehaviour;
    private Chunk Chunk;

    public EnemyManager(WorldLocation worldLocation, Chunk chunk)
    {
        Position = new Vector2(worldLocation.X, worldLocation.Y);
        Chunk = chunk;
    }

    public void Update(Treadmill treadmill)
    {
        if (Awake)
        {
            bool onTreadmill = treadmill.OnTreadmill(MonoBehaviour.transform.position);
            if (!onTreadmill)
            {
                Sleep();
            }
        }
        else
        {
            bool onTreadmill = treadmill.OnTreadmill(Position);
            if (onTreadmill)
            {
                WakeUp();
            }
        }
    }

    public void WakeUp()
    {
        Awake = true;
        GameObject GameObject = GameObject.Instantiate(Prefabs.ENEMY_PREFAB, Position, Quaternion.identity);
        MonoBehaviour = GameObject.GetComponent<EnemyMonoBehaviour>();
        MonoBehaviour.AssignManager(this);
        GameManager.Singleton.GameObjectCount++;
    }

    public void Sleep()
    {
        Awake = false;
        Position = MonoBehaviour.transform.position;
        MonoBehaviour.Destory();
    }

    public void Die()
    {
        MonoBehaviour.Destory();
        Chunk.EnemyHasDied(this);
    }
}