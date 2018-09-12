using UnityEngine;

public class EnemyAI
{
    private readonly EnemyManager Manager;

    public EnemyAI(EnemyManager manager)
    {
        Manager = manager;
    }

    public Vector2 Update()
    {
        Vector2 playerPosition = Manager.World.PlayerManager.GetPlayerPosition();
        Vector2 myPosition = Manager.MonoBehaviour.transform.position;
        float distance = (playerPosition - myPosition).magnitude;
        if (distance < 2f)
        {
            Manager.SlashAttack(playerPosition);
        }

        return Manager.World.PlayerManager.GetPlayerPosition();
    }
}