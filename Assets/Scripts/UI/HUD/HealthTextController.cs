using UnityEngine;
using UnityEngine.UI;
using ADL.Core;
using ADL.Util;
using ADL.Combat.Player;
namespace ADL.UI
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
                float health = Helpers.Round(player.CurrentHealth, 0.1f);
                float maxHealth = Helpers.Round(Configuration.PLAYER_MAX_HEALTH(player.MaxHealthPoints), 0.1f);
                Text.text = Helpers.Truncate(health.ToString(), 4) + "/" + Helpers.Truncate(maxHealth.ToString(), 4);
            }
        }
    }
}