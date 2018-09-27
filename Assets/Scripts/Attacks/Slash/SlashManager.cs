using UnityEngine;

public class SlashManager : AttackManager
{
    private CombatantManager Originator;
    private float Damage;

    public SlashManager(CombatantManager originator, Vector2 position, float angle, float aoe, float damage)
    {
        Originator = originator;
        Damage = damage;

        GameObject slash = GameObject.Instantiate(Prefabs.SLASH_PREFAB, position, Quaternion.Euler(0, 0, angle));
        slash.transform.localScale = new Vector3(aoe, aoe, 1);
        slash.GetComponent<SlashMonoBehaviour>().AssignManager(this);

        base.InitExpirationListeners();
    }

    public void ResolveCollision(CombatantManager other)
    {
        if (other.Team != Originator.Team)
        {
            Vector2 knockback = other.GetPosition() - Originator.GetPosition();
            knockback = knockback.normalized;
            int exp = other.RecieveHit(this, Damage, knockback);
            Originator.RecieveExp(exp);
        }
    }
}
