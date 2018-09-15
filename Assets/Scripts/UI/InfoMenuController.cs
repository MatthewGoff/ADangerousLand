using UnityEngine;
using UnityEngine.UI;

public class InfoMenuController : MonoBehaviour {

    public GameObject RandomSeedText;
    public GameObject PlayerLocationText;

	public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void OnEnable()
    {
        RandomSeedText.GetComponent<Text>().text = "Random Seed: " + GameManager.Singleton.World.GetRandomSeed().ToString();
        PlayerLocationText.GetComponent<Text>().text = "Player Location: " + GameManager.Singleton.World.GetPlayerLocation().ToString();
    }
}
