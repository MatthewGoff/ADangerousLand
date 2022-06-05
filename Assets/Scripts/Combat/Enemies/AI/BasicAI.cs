using UnityEngine;
using MessagePack;
using ADL.Core;

namespace ADL.Combat.Enemies.AI
{
    /// <summary>
    /// A very rudimentary AI which walks strait towards the player and attacks whenever possible
    /// </summary>
    [MessagePackObject]
    public class BasicAI : EnemyAI
    {
        /// <summary>
        /// Whether this enemy is currently aggo'd
        /// </summary>
        [Key(0)] public bool Agroed;
        /// <summary>
        /// The duration after which this enemy will stop being aggro'd
        /// </summary>
        [Key(1)] public float AgroCountdown;

        /// <summary>
        /// Create a BasicAI
        /// </summary>
        public BasicAI()
        {
            Agroed = false;
            AgroCountdown = 0f;
        }
        
        /// <summary>
        /// Serialization Constructor
        /// </summary>
        /// <remarks>
        /// See documentation of member fields
        /// </remarks>
        /// <param name="agroed"></param>
        /// <param name="agroCountdown"></param>
        [SerializationConstructor]
        public BasicAI(bool agroed, float agroCountdown)
        {
            Agroed = agroed;
            AgroCountdown = agroCountdown;
        }

        /// <summary>
        /// Should be called every fixed update cycle
        /// </summary>
        /// <param name="manager">
        /// The manager of this enemy
        /// </param>
        /// <returns>
        /// The AI perscribed movement target for this enemy
        /// </returns>
        public Vector2 FixedUpdate(EnemyManager manager)
        {
            Vector2 playerPosition = GameManager.Singleton.World.PlayerManager.GetCenter();
            Vector2 myPosition = manager.GetCenter();
            float distance = (playerPosition - myPosition).magnitude;
            if (distance < manager.Aoe + 1f)
            {
                manager.SlashAttack(playerPosition);
            }

            if ((distance < manager.AgroDistance)
                || (distance < manager.DeAgroDistance && Agroed))
            {
                Agroed = true;
                AgroCountdown = manager.MinAgroDuration;
            }

            AgroCountdown -= Time.fixedDeltaTime;
            if (AgroCountdown <= 0f)
            {
                Agroed = false;
            }

            if (Agroed)
            {
                return playerPosition - myPosition;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }
}