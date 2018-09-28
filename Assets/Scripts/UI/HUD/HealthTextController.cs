using UnityEngine;
using UnityEngine.UI;

public class HealthTextController : MonoBehaviour
{
    private Text Text;

    void Start () {
        Text = GetComponent<Text>();
	}
	
	void Update () {
		if (GameManager.Singleton.World != null
                && GameManager.Singleton.World.PlayerManager != null)
        {
            PlayerManager player = GameManager.Singleton.World.PlayerManager;
            float health = player.CurrentHealth;
            health = Mathf.Floor(health * 10) / 10;
            Text.text = health.ToString() + "/" + Configuration.PLAYER_MAX_LIFE(player.MaxHealthPoints).ToString();
        }
	}
}
