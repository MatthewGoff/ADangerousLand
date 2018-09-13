using UnityEngine;

public class SlashManager
{
    private CombatantManager Originator;

    public SlashManager(CombatantManager originator, Vector2 position, float angle, float aoe)
    {
        Originator = originator;

        GameObject slash = GameObject.Instantiate(Prefabs.SLASH_PREFAB, position, Quaternion.Euler(0, 0, angle));
        slash.transform.localScale = new Vector3(aoe, aoe, 1);
        slash.GetComponent<SlashMonoBehaviour>().AssignManager(this);
    }

    public void ResolveCollision(CombatantManager other)
    {
        if (other.Team != Originator.Team)
        {
            int exp = other.RecieveHit();
            Originator.RecieveExp(exp);
        }
    }
}
