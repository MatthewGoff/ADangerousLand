using UnityEngine;
using MessagePack;

namespace ADL.Combat.Enemies.AI
{
    [MessagePack.Union(0, typeof(BasicAI))]
    public interface EnemyAI
    {
        Vector2 FixedUpdate(EnemyManager manager);
    }
}