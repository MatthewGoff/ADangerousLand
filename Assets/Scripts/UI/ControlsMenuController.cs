using UnityEngine;

public class ControlsMenuController : MonoBehaviour {

    public void ClosePressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }
}
