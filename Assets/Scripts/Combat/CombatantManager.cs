using MessagePack;
using UnityEngine;
using ADL.Combat.Attacks;

namespace ADL.Combat
{
    /// <summary>
    /// Abstract class representing a combatant (player or enemy)
    /// </summary>
    [MessagePackObject]
    public abstract class CombatantManager
    {
        /// <summary>
        /// Team of the combatant (0 for player, 1 for enemy).
        /// </summary>
        [IgnoreMember] public int Team;

        /// <summary>
        /// Get the position of the position of the combatant
        /// </summary>
        /// <returns>
        /// The position of the combatant
        /// </returns>
        public abstract Vector2 GetPosition();

        /// <summary>
        /// Recieve a hit from an attack
        /// </summary>
        /// <param name="attackManager">
        /// The AttackManager of the incoming attack
        /// </param>
        /// <param name="damage">
        /// The damage of the incoming attack
        /// </param>
        /// <param name="knockback">
        /// The knockback of the incoming attack
        /// </param>
        /// <returns>
        /// The experience rewarded if this attack kills the player
        /// </returns>
        public abstract int RecieveHit(AttackManager attackManager, float damage, Vector2 knockback);

        /// <summary>
        /// Recieve experience
        /// </summary>
        /// <param name="exp">
        /// The amount of experience recieved
        /// </param>
        public abstract void RecieveExp(int exp);
    }
}