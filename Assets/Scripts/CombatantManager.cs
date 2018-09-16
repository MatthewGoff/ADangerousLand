using System;

[Serializable]
public abstract class CombatantManager
{
    [NonSerialized] public int Team;

    public abstract int RecieveHit(float damage);
    public abstract void RecieveExp(int exp);
}
