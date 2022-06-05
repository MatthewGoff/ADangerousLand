using UnityEngine;
using ADL.Core;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// Conducts execution and resolution of a stomp type attack
    /// </summary>
    /// <remarks>
    /// See the Manager/Monobehaviour design pattern
    /// </remarks>
    public class StompManager : AttackManager
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
        /// Create a StompManager. Immediatly executes a stomp attack
        /// </summary>
        /// <param name="originator">
        /// The CombatantManager of the combatant initiating the attack
        /// </param>
        /// <param name="position">
        /// The center of the stomp
        /// </param>
        /// <param name="damage">
        /// The ammount of damage this attack will do
        /// </param>
        public StompManager(CombatantManager originator, Vector2 position, float damage)
        {
            Originator = originator;
            Damage = damage;

            GameObject stomp = GameObject.Instantiate(Prefabs.STOMP_PREFAB, position, Quaternion.identity);
            stomp.GetComponent<StompMonoBehaviour>().AssignManager(this);

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
                Vector2 knockback = other.GetPosition() - Originator.GetPosition();
                knockback = knockback.normalized * 5;
                int exp = other.RecieveHit(this, Damage, knockback);
                Originator.RecieveExp(exp);
            }
        }
    }
}
