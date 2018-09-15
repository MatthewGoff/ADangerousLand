using UnityEngine;

public class InfoMenuController : MonoBehaviour {

	public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }
}
