using UnityEngine;
using ADL.Core;
using ADL.Util;

namespace ADL.Combat.Attacks
{
    public class DashManager : AttackManager
    {
        private CombatantManager Originator;
        private float Damage;
        private Vector2 DashVector;
        private Vector2 Position;

        public DashManager(CombatantManager originator, Vector2 position, Vector2 destination, float damage)
        {
            Originator = originator;
            Damage = damage;

            Position = position;
            DashVector = destination - position;
            float dashAngle = Vector2.SignedAngle(Vector2.right, DashVector);

            GameObject dash = GameObject.Instantiate(Prefabs.DASH_PREFAB, position, Quaternion.Euler(0, 0, dashAngle));
            dash.transform.localScale = new Vector3(DashVector.magnitude, 0.5f, 1);
            dash.GetComponent<DashMonoBehaviour>().AssignManager(this);

            base.InitExpirationListeners();
        }

        public void ResolveCollision(CombatantManager other)
        {
            if (other.Team != Originator.Team)
            {
                Vector2 DashNormal = Helpers.ClockwiseNormalVector(DashVector).normalized * 2;

                Vector2 enemyVector = other.GetPosition() - Position;
                float angle = Vector2.SignedAngle(DashVector, enemyVector);
                if (angle > 0)
                {
                    DashNormal *= -1;
                }

                int exp = other.RecieveHit(this, Damage, DashNormal);
                Originator.RecieveExp(exp);
            }
        }
    }
}