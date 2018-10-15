using UnityEngine;
using UnityEngine.UI;
using ADL.Core;
using ADL.Combat.Player;
using ADL.Util;

namespace ADL.UI
{
    public class ManaTextController : MonoBehaviour
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
                float stamina = Helpers.Round(player.CurrentStamina, 0.1f);
                float maxStamina = Helpers.Round(Configuration.PLAYER_MAX_STAMINA(player.MaxStaminaPoints), 0.1f);
                Text.text = Helpers.Truncate(stamina.ToString(), 4) + "/" + Helpers.Truncate(maxStamina.ToString(), 4);
            }

        }
    }
}
