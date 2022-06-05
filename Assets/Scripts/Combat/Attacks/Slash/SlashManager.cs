using UnityEngine;
using ADL.Core;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// Conducts execution and resolution of a slash type attack
    /// </summary>
    /// <remarks>
    /// See the Manager/Monobehaviour design pattern
    /// </remarks>
    public class SlashManager : AttackManager
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
        /// Create a SlashManager. Immediatly executes a slash attack
        /// </summary>
        /// <param name="originator">
        /// The CombatatManager of the combatatant initiating the attack
        /// </param>
        /// <param name="position">
        /// The position from which the attack originates
        /// </param>
        /// <param name="angle">
        /// The counter clockwise angle fromt the vector (1,0) determining the center of the attack wedge
        /// </param>
        /// <param name="aoe">
        /// The radius of the wedge of the attack
        /// </param>
        /// <param name="damage">
        /// The damage the attack will do
        /// </param>
        public SlashManager(CombatantManager originator, Vector2 position, float angle, float aoe, float damage)
        {
            Originator = originator;
            Damage = damage;

            GameObject slash = GameObject.Instantiate(Prefabs.SLASH_PREFAB, position, Quaternion.Euler(0, 0, angle));
            slash.transform.localScale = new Vector3(aoe, aoe, 1);
            slash.GetComponent<SlashMonoBehaviour>().AssignManager(this);

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
                knockback = knockback.normalized;
                int exp = other.RecieveHit(this, Damage, knockback);
                Originator.RecieveExp(exp);
            }
        }
    }
}
