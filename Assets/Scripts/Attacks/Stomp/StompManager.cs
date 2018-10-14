using UnityEngine;

namespace ADL
{
    public class StompManager : AttackManager
    {
        private CombatantManager Originator;
        private float Damage;

        public StompManager(CombatantManager originator, Vector2 position, float damage)
        {
            Originator = originator;
            Damage = damage;

            GameObject stomp = GameObject.Instantiate(Prefabs.STOMP_PREFAB, position, Quaternion.identity);
            stomp.GetComponent<StompMonoBehaviour>().AssignManager(this);

            base.InitExpirationListeners();
        }

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
