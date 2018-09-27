using UnityEngine;

public class BoltManager : AttackManager
{
    private CombatantManager Originator;
    private GameObject Bolt;
    private float Damage;

    public BoltManager(CombatantManager originator, Vector2 position, float angle, float damage)
    {
        Originator = originator;
        Damage = damage;

        Bolt = GameObject.Instantiate(Prefabs.BOLT_PREFAB, position, Quaternion.Euler(0,0,angle));
        Bolt.GetComponent<BoltMonoBehaviour>().AssignManager(this);

        base.InitExpirationListeners();
    }

    public void ResolveCollision(CombatantManager other)
    {
        if (other.Team != Originator.Team)
        {
            Vector2 knockback = other.GetPosition() - Originator.GetPosition();
            knockback = knockback.normalized * 0.5f;
            int exp = other.RecieveHit(this, Damage, knockback);
            Originator.RecieveExp(exp);
            Expire();
            GameObject.Destroy(Bolt.gameObject);
        }
    }
}
