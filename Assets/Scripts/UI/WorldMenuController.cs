using UnityEngine;

public class WorldMenuController : MonoBehaviour {

    public void PlayPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.StartPlay);
    }

    public void CreatePressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.OpenNewWorldMenu);
    }

    public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void DeletePressed()
    {

    }
}
