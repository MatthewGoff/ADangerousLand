namespace ADL.Combat
{
    /// <summary>
    /// Interface for classes which have a hitbox and a CombatantManager
    /// </summary>
    /// <remarks>
    /// Intented to be used with MonoBehaviours which need to provide the CombatantManager to a collision resolution
    /// </remarks>
    public interface IHitboxOwner
    {
        /// <summary>
        /// Get the CombatantManager associated with this hitbox owner
        /// </summary>
        /// <returns>
        /// The CombatantManager associated with this hitbox owner
        /// </returns>
        CombatantManager GetCombatantManager();
    }
}