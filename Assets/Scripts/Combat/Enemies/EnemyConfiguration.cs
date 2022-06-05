using UnityEngine;
using ADL.Combat.Enemies.AI;

namespace ADL.Combat.Enemies
{
    /// <summary>
    /// The configuration of various stats and attributes for a type of enemy
    /// </summary>
    public struct EnemyConfiguration
    {
        /// <summary>
        /// The path relative to the resources folder where the enemy prefab can be found
        /// </summary>
        public string PrefabLocation;
        /// <summary>
        /// The AI type of the enemy
        /// </summary>
        public AIType AIType;
        /// <summary>
        /// The maximum health of the enemy
        /// </summary>
        public int MaxHealth;
        /// <summary>
        /// The movement speed of the enemy
        /// </summary>
        public float MoveSpeed;
        /// <summary>
        /// The experience that this enemy rewards the player when it dies
        /// </summary>
        public int ExperienceReward;
        /// <summary>
        /// The base damage this enemy does with attack
        /// </summary>
        public float Damage;
        /// <summary>
        /// The attack speed of this enemy
        /// </summary>
        public float AttackSpeed;
        /// <summary>
        /// The AOE of melee attacks made by this enemy
        /// </summary>
        public float Aoe;
        /// <summary>
        /// The distance from the player under which underwhich which this enemy will become aggro'd
        /// </summary>
        public float AgroDistance;
        /// <summary>
        /// The distance from the player over which this enemy will start losing aggro
        /// </summary>
        public float DeAgroDistance;
        /// <summary>
        /// The duration of absent aggro before which the enemy will de-aggro
        /// </summary>
        public float MinAgroDuration;
        /// <summary>
        /// The width of spawn space this enemy needs to spawn
        /// </summary>
        public float Width;
        /// <summary>
        /// The height of spawn space this enemy needs to spawn
        /// </summary>
        public float Height;
        /// <summary>
        /// A vector relative to the bottom center of the enemy indicating where attacks from the enemy originate
        /// </summary>
        public Vector2 AttackOrigin;
    }
}