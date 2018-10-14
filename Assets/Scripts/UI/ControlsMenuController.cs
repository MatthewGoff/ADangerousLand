using UnityEngine;

namespace ADL
{
    public class ControlsMenuController : MonoBehaviour
    {

        public void ClosePressed()
        {
            GameManager.Singleton.TakeInput(GameInputType.Escape);
        }
    }
}
