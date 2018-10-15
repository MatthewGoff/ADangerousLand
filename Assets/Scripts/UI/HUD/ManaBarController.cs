using UnityEngine;
using ADL.Core;
using ADL.Combat.Player;

namespace ADL.UI
{
    public class ManaBarController : MonoBehaviour
    {
        private RectTransform Rect;

        private void Start()
        {
            Rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (GameManager.Singleton.World != null
                && GameManager.Singleton.World.PlayerManager != null)
            {
                PlayerManager player = GameManager.Singleton.World.PlayerManager;
                Rect.localScale = new Vector2(player.CurrentStamina / Configuration.PLAYER_MAX_STAMINA(player.MaxStaminaPoints), 1);
            }
        }
    }
}