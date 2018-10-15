using System.Collections.Generic;

namespace ADL.Combat.Attacks
{
    public abstract class AttackManager
    {

        public delegate void ExpirationListener(AttackManager attackManager);

        protected List<ExpirationListener> ExpirationListeners;

        protected void InitExpirationListeners()
        {
            ExpirationListeners = new List<ExpirationListener>();
        }

        public void AddExpirationListner(ExpirationListener expirationListener)
        {
            ExpirationListeners.Add(expirationListener);
        }

        public void Expire()
        {
            foreach (ExpirationListener expirationListener in ExpirationListeners)
            {
                expirationListener(this);
            }
        }
    }
}
