using UnityEngine;
using ADL.Core;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// Conducts execution and resolution of a bolt type attack
    /// </summary>
    /// <remarks>
    /// See the Manager/Monobehaviour design pattern
    /// </remarks>
    public class BoltManager : AttackManager
    {
        /// <summary>
        /// The CombatantManager of the combatant initiating the attack
        /// </summary>
        private CombatantManager Originator;
        /// <summary>
        /// The GameObject with the sprite and collider for this attack
        /// </summary>
        private GameObject Bolt;
        /// <summary>
        /// The ammount of damage this attack will do
        /// </summary>
        private float Damage;

        /// <summary>
        /// Create a BoltManager. Immediatly executes a bolt attack
        /// </summary>
        /// <param name="originator">
        /// The CombatantManager of the combatant initiating the attack
        /// </param>
        /// <param name="position">
        /// The position from which the attack originates
        /// </param>
        /// <param name="angle">
        /// The counterclockwise angle from the Vector (1,0) along which the bolt will travel
        /// </param>
        /// <param name="damage">
        /// The damage the bolt will do on impact
        /// </param>
        public BoltManager(CombatantManager originator, Vector2 position, float angle, float damage)
        {
            Originator = originator;
            Damage = damage;

            Bolt = GameObject.Instantiate(Prefabs.BOLT_PREFAB, position, Quaternion.Euler(0, 0, angle));
            Bolt.GetComponent<BoltMonoBehaviour>().AssignManager(this);

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
                knockback = knockback.normalized * 0.5f;
                int exp = other.RecieveHit(this, Damage, knockback);
                Originator.RecieveExp(exp);
                Expire();
                GameObject.Destroy(Bolt.gameObject);
            }
        }
    }
}
