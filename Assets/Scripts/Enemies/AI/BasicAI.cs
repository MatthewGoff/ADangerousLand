using UnityEngine;
using System;

[Serializable]
public class BasicAI : EnemyAI
{
    public Vector2 Update(EnemyManager manager)
    {
        Vector2 playerPosition = GameManager.Singleton.World.PlayerManager.GetPlayerPosition();
        Vector2 myPosition = manager.MonoBehaviour.transform.position;
        float distance = (playerPosition - myPosition).magnitude;
        if (distance < manager.Aoe)
        {
            manager.SlashAttack(playerPosition);
        }

        return GameManager.Singleton.World.PlayerManager.GetPlayerPosition();
    }
}