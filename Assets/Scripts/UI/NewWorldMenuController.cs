using UnityEngine;

public class NewWorldMenuController : MonoBehaviour {

    public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void CreatePressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }
}
