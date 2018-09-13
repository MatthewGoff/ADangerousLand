using UnityEngine;

public class SlashManager
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
    }

    public void ResolveCollision(CombatantManager other)
    {
        if (other.Team != Originator.Team)
        {
            int exp = other.RecieveHit(Damage);
            Originator.RecieveExp(exp);
        }
    }
}
