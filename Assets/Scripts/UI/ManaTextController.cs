using UnityEngine;
using UnityEngine.UI;

public class ManaTextController : MonoBehaviour
{
    private PlayerManager Player;
    private Text Text;
    private bool PlayerInitialized = false;

    void Start()
    {
        Text = GetComponent<Text>();
    }

    void Update()
    {
        if (PlayerInitialized)
        {
            float mana = Player.CurrentMana;
            mana = Mathf.Floor(mana * 10) / 10;
            Text.text = mana.ToString() + "/" + Player.MaxMana.ToString();
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
