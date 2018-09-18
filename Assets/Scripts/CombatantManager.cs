using MessagePack;

[MessagePackObject]
public abstract class CombatantManager
{
    [IgnoreMember] public int Team;

    public abstract int RecieveHit(float damage);
    public abstract void RecieveExp(int exp);
}
