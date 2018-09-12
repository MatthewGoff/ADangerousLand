using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashManager
{
    private CombatantManager Originator;

    public SlashManager(CombatantManager originator, Vector2 position, Quaternion angle, float aoe)
    {
        Originator = originator;

        GameObject slash = GameObject.Instantiate(Prefabs.SLASH_PREFAB, position, angle);
        slash.transform.localScale = new Vector3(aoe, aoe, 1);
        slash.GetComponent<SlashMonoBehaviour>().AssignManager(this);
    }

    public void ResolveCollision(CombatantManager other)
    {
        if (other.Team != Originator.Team)
        {
            other.RecieveHit();
        }
    }
}
