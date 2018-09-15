﻿using UnityEngine;

public class BoltManager
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
    }

    public void ResolveCollision(CombatantManager other)
    {
        if (other.Team != Originator.Team)
        {
            int exp = other.RecieveHit(Damage);
            Originator.RecieveExp(exp);
            GameObject.Destroy(Bolt.gameObject);
        }
    }
}