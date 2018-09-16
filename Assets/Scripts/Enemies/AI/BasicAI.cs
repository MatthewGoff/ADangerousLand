using UnityEngine;

public class BasicAI : EnemyAI
{
    private readonly EnemyManager Manager;

    public BasicAI(EnemyManager manager)
    {
        Manager = manager;
    }

    public Vector2 Update()
    {
        Vector2 playerPosition = Manager.World.PlayerManager.GetPlayerPosition();
        Vector2 myPosition = Manager.MonoBehaviour.transform.position;
        float distance = (playerPosition - myPosition).magnitude;
        if (distance < Manager.Aoe)
        {
            Manager.SlashAttack(playerPosition);
        }

        return Manager.World.PlayerManager.GetPlayerPosition();
    }
}