using UnityEngine;

public class NewPlayerMenuController : MonoBehaviour {

    public void CreatePressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }
}
