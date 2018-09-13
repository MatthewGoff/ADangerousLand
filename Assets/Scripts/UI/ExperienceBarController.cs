using UnityEngine;

public class ExperienceBarController : MonoBehaviour
{
    private PlayerManager Player;
    private bool PlayerInitialized = false;
    private RectTransform Rect;

    private void Start()
    {
        Rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (PlayerInitialized)
        {
            int min = Configuration.LEVEL_EXPERIENCE[Player.Level];
            int max = Configuration.LEVEL_EXPERIENCE[Player.Level+1];
            int goal = max - min;
            int progress = Player.Experience - min;
            Rect.localScale = new Vector2((float)progress / goal, 1);
        }
        else
        {
            if (GameManager.Singleton != null
                && GameManager.Singleton.World != null
                && GameManager.Singleton.World.PlayerManager != null)
            {
                Player = GameManager.Singleton.World.PlayerManager;
                PlayerInitialized = true;
            }
        }
    }
}