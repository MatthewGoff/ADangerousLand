using UnityEngine;
using UnityEngine.UI;

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
            float mana = player.CurrentStamina;
            mana = Mathf.Floor(mana * 10) / 10;
            Text.text = mana.ToString() + "/" + Configuration.PLAYER_MAX_LIFE(player.MaxHealthPoints);
        }

    }
}
