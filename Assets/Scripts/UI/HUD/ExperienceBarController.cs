using UnityEngine;

public class ExperienceBarController : MonoBehaviour
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
            int min = Configuration.GetLevelExperience(player.Level);
            int max = Configuration.GetLevelExperience(player.Level+1);
            int goal = max - min;
            int progress = player.Experience - min;
            Rect.localScale = new Vector2((float)progress / goal, 1);
        }
    }
}