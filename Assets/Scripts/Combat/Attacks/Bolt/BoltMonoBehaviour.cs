using UnityEngine;
using System.Collections;
using ADL.Core;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// MonoBehaviour for a bolt attack
    /// </summary>
    /// <remarks>
    /// See the Manager/MonoBehaviour design pattern
    /// </remarks>
    public class BoltMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The SpriteRenderer of the GameObject attached to this MonoBehaviour
        /// </summary>
        private SpriteRenderer Renderer;
        /// <summary>
        /// The manager of this MonoBehaviour
        /// </summary>
        private BoltManager Manager;
        /// <summary>
        /// The speed in tiles per second that the bolt will travel
        /// </summary>
        private float Speed = 10;
        /// <summary>
        /// The distance this bolt will travel before despawning
        /// </summary>
        private float Distance = 30;
        /// <summary>
        /// The direction in which the bolt will travel
        /// </summary>
        private Vector2 VelocityVector;

        /// <summary>
        /// Gets called when the object attached to this MonoBehaviour is instantiated
        /// </summary>
        void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
            VelocityVector = Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector2.right;
            StartCoroutine("Fly");
        }

        /// <summary>
        /// Progress the bolt's position each frame; Destory the bolt after a set distance
        /// </summary>
        /// <returns>
        /// Required return type of a coroutine
        /// </returns>
        private IEnumerator Fly()
        {
            for (float i = 0f; i < Distance; i += Speed * Time.deltaTime)
            {
                transform.position = (Vector2)transform.position + VelocityVector * Speed * Time.deltaTime;
                yield return null;
            }
            Manager.Expire();
            Destroy(gameObject);
        }

        /// <summary>
        /// Gets called each frame
        /// </summary>
        public void Update()
        {
            Renderer.color = new Color(1, 1, 1, GameManager.Singleton.World.GetVisibilityLevel(transform.position));
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
        public void AssignManager(BoltManager manager)
        {
            Manager = manager;
        }
    }
}