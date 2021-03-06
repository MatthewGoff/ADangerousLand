﻿using UnityEngine;
using System.Collections;

namespace ADL.Combat.Attacks
{
    public class DashMonoBehaviour : MonoBehaviour
    {
        private DashManager Manager;

        private SpriteRenderer SpriteRenderer;
        private float Lifetime;

        void Start()
        {
            Lifetime = 0.5f;
            SpriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine("Fade");
        }

        private IEnumerator Fade()
        {
            for (float i = 0; i < Lifetime; i += Time.deltaTime)
            {
                if (i >= 3 * Time.deltaTime)
                {
                    Destroy(GetComponent<BoxCollider2D>());
                }
                SpriteRenderer.color = new Color(255, 255, 255, 1 - i / Lifetime);
                yield return null;
            }
            Manager.Expire();
            Destroy(gameObject);
        }

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

        public void AssignManager(DashManager manager)
        {
            Manager = manager;
        }
    }
}