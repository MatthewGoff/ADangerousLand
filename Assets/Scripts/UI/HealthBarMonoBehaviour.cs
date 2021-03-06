﻿using UnityEngine;
using ADL.Core;

namespace ADL.UI
{
    public class HealthBarMonoBehaviour : MonoBehaviour
    {

        private SpriteRenderer Renderer;

        public void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            Renderer.color = new Color(1, 1, 1, GameManager.Singleton.World.GetVisibilityLevel(transform.position));
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void ShowHealth(float health)
        {
            if (health == 1f)
            {
                Renderer.enabled = false;
            }
            else
            {
                Renderer.enabled = true;
                transform.localScale = new Vector3(health, 1, 1);
            }
        }
    }
}