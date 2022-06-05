using UnityEngine;
using MessagePack;

namespace ADL.Combat.Enemies.AI
{
    /// <summary>
    /// Interface for an Enemy AI
    /// </summary>
    [MessagePack.Union(0, typeof(BasicAI))]
    public interface EnemyAI
    {
        /// <summary>
        /// Update the AI and return the prescribed movement target
        /// </summary>
        /// <param name="manager">
        /// The Manager of this AI
        /// </param>
        /// <returns>
        /// The position to which the AI is directing the enemy
        /// </returns>
        Vector2 FixedUpdate(EnemyManager manager);
    }
}