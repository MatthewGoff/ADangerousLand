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
            float stamina = Util.Round(player.CurrentStamina, 0.1f);
            float maxStamina = Util.Round(Configuration.PLAYER_MAX_STAMINA(player.MaxStaminaPoints), 0.1f);
            Text.text = Util.Truncate(stamina.ToString(), 4) + "/" + Util.Truncate(maxStamina.ToString(), 4);
        }

    }
}
