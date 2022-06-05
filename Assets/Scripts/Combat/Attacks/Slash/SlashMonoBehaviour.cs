using UnityEngine;
using System.Collections;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// MonoBehaviour for a bolt attack
    /// </summary>
    /// <remarks>
    /// See the Manager/MonoBehaviour design pattern
    /// </remarks>
    public class SlashMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The manager of this MonoBehaviour
        /// </summary>
        private SlashManager Manager;
        /// <summary>
        /// The SpriteRenderer of the GameObject attached to this MonoBehaviour
        /// </summary>
        private SpriteRenderer SpriteRenderer;
        /// <summary>
        /// The duration that the image of the wedge will persist
        /// </summary>
        private float Lifetime;

        /// <summary>
        /// Gets called when the object attached to this MonoBehaviour is instantiated
        /// </summary>
        void Awake()
        {
            Lifetime = 0.5f;
            SpriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine("Fade");
        }

        /// <summary>
        /// Fade the image of the attack and destroy it after a duration
        /// </summary>
        /// <returns>
        /// Required return type of a coroutine
        /// </returns>
        private IEnumerator Fade()
        {
            for (float i = 0; i < Lifetime; i += Time.deltaTime)
            {
                if (i >= 3 * Time.deltaTime)
                {
                    Destroy(GetComponent<PolygonCollider2D>());
                }
                SpriteRenderer.color = new Color(255, 255, 255, 1 - i / Lifetime);
                yield return null;
            }
            Manager.Expire();
            Destroy(gameObject);
        }

        /// <summary>
        /// Gets called when the collider attached to the same GameObject as this MonoBehaviour collides with another collider
        /// </summary>
        /// <param name="other">
        /// The collider with which this GameObject has collided
        /// </param>
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Hitbox")
            {
                IHitboxOwner monoBehaviour = other.gameObject.transform.parent.gameObject.GetComponent<IHitboxOwner>();
                if (monoBehaviour != null)
                {
                    Manager.ResolveCollision(monoBehaviour.GetCombatantManager());
                }
            }
        }

        /// <summary>
        /// Assign this MonoBehaviour a manager
        /// </summary>
        /// <param name="manager">
        /// The manager of this MonoBehaviour
        /// </param>
        public void AssignManager(SlashManager manager)
        {
            Manager = manager;
        }
    }
}