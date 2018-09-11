using UnityEngine;

public class ManaBarController : MonoBehaviour
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
            Rect.localScale = new Vector2(Player.CurrentMana / Player.MaxMana, 1);
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