using UnityEngine;

namespace ADL
{
    public class HealthBarController : MonoBehaviour
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
                Rect.localScale = new Vector2(player.CurrentHealth / Configuration.PLAYER_MAX_HEALTH(player.MaxHealthPoints), 1);
            }
        }
    }
}