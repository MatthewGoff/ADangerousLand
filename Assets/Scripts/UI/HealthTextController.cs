using UnityEngine;
using UnityEngine.UI;

public class HealthTextController : MonoBehaviour
{
    private PlayerManager Player;
    private Text Text;
    private bool PlayerInitialized = false;

    void Start () {
        Text = GetComponent<Text>();
	}
	
	void Update () {
		if (PlayerInitialized)
        {
            float health = Player.CurrentHealth;
            health = Mathf.Floor(health * 10) / 10;
            Text.text = health.ToString() + "/" + Player.MaxHealth.ToString();
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
