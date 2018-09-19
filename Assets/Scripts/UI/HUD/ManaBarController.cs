using UnityEngine;

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
            Rect.localScale = new Vector2(player.CurrentStamina / player.MaxStamina, 1);
        }
    }
}