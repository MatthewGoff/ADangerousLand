using UnityEngine;
using ADL.Core;
using ADL.Util;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// Conducts execution and resolution of dash type attack
    /// </summary>
    /// <remarks>
    /// See the Manager/MonoBehaviour design pattern
    /// </remarks>
    public class DashManager : AttackManager
    {
        /// <summary>
        /// The CombatantManager of the combatant initiating the attack
        /// </summary>
        private CombatantManager Originator;
        /// <summary>
        /// The ammount of damage this attack will do
        /// </summary>
        private float Damage;
        /// <summary>
        /// The direction in which the combatant dashes. Used for knockback
        /// </summary>
        private Vector2 DashVector;
        /// <summary>
        /// The initial position from which the combatant dashes
        /// </summary>
        private Vector2 Position;

        /// <summary>
        /// Create a Dashmanager. Immediatly executes a dash
        /// </summary>
        /// <param name="originator">
        /// The combatantManager of the combatant initiating the attack
        /// </param>
        /// <param name="position">
        /// The initial position from which the combatant dashes
        /// </param>
        /// <param name="destination">
        /// The target destination of the dash.
        /// </param>
        /// <param name="damage">
        /// The ammount of damage this attack will do
        /// </param>
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

        /// <summary>
        /// Administer damage and experience as the result of a collision with this attack
        /// </summary>
        /// <param name="other">
        /// The CombatantManager of the combatant recieving the attack
        /// </param>
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