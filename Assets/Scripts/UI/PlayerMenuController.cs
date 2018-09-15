using UnityEngine;

public class PlayerMenuController : MonoBehaviour {

	public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void CreatePressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.OpenNewPlayerMenu);
    }

    public void DeletePressed()
    {

    }

    public void NextPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.OpenWorldMenu);
    }
}
