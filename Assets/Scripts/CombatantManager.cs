public abstract class CombatantManager
{
    public int Team;

    protected int MaxHealth;
    protected int CurrentHealth;

    public abstract void RecieveHit();
}
