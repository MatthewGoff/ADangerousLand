﻿using UnityEngine;
using UnityEngine.UI;

namespace ADL
{
    public class HealthTextController : MonoBehaviour
    {
        private Text Text;

        void Start()
        {
            Text = GetComponent<Text>();
        }

        void Update()
        {
            if (GameManager.Singleton.World != null
                    && GameManager.Singleton.World.PlayerManager != null)
            {
                PlayerManager player = GameManager.Singleton.World.PlayerManager;
                float health = Util.Round(player.CurrentHealth, 0.1f);
                float maxHealth = Util.Round(Configuration.PLAYER_MAX_HEALTH(player.MaxHealthPoints), 0.1f);
                Text.text = Util.Truncate(health.ToString(), 4) + "/" + Util.Truncate(maxHealth.ToString(), 4);
            }
        }
    }
}