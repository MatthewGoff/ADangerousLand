using UnityEngine;

public class PauseMenuController : MonoBehaviour {

	public void ResumePressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void OptionsPressed()
    {

    }

    public void InfoMenuPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.OpenInfoMenu);
    }

    public void ExitPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.SaveAndExit);
    }
}
