using UnityEngine;
using MessagePack;

[MessagePack.Union(0, typeof(BasicAI))]
public interface EnemyAI
{
    Vector2 FixedUpdate(EnemyManager manager);
}
