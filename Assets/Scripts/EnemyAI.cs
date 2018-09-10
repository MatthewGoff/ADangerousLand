using UnityEngine;

public class EnemyAI
{
    private readonly EnemyManager Manager;

    public EnemyAI(EnemyManager manager)
    {
        Manager = manager;
    }

    public Vector2 GetMoveTarget()
    {
        return Manager.World.PlayerManager.GetPlayerPosition();
    }
}