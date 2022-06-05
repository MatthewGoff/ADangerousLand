using System.Collections.Generic;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// Abstraction of a class which conducts execution and resolution of an attack
    /// </summary>
    /// <remarks>
    /// See the Immunization design pattern
    /// </remarks>
    public abstract class AttackManager
    {
        /// <summary>
        /// Delegate for a method to call when the attack expires
        /// </summary>
        /// <param name="attackManager">
        /// the AttacManager expireing
        /// </param>
        public delegate void ExpirationListener(AttackManager attackManager);

        /// <summary>
        /// A list of ExpirationListners to be called when this attack expires
        /// </summary>
        protected List<ExpirationListener> ExpirationListeners;

        /// <summary>
        /// Create an empty list of ExpirationListeners
        /// </summary>
        protected void InitExpirationListeners()
        {
            ExpirationListeners = new List<ExpirationListener>();
        }

        /// <summary>
        /// Add a method to this AttackManager's list of ExpirationListeners
        /// </summary>
        /// <param name="expirationListener">
        /// The method to be called when this attack expires
        /// </param>
        public void AddExpirationListner(ExpirationListener expirationListener)
        {
            ExpirationListeners.Add(expirationListener);
        }

        /// <summary>
        /// Call all of the ExpirationListenters
        /// </summary>
        public void Expire()
        {
            foreach (ExpirationListener expirationListener in ExpirationListeners)
            {
                expirationListener(this);
            }
        }
    }
}
