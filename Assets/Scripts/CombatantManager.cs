using MessagePack;
using UnityEngine;

[MessagePackObject]
public abstract class CombatantManager
{
    [IgnoreMember] public int Team;

    public abstract Vector2 GetPosition();
    public abstract int RecieveHit(AttackManager attackManager, float damage, Vector2 knockback);
    public abstract void RecieveExp(int exp);
}
