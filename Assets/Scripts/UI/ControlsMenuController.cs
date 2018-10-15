using UnityEngine;
using ADL.Core;

namespace ADL.UI
{
    public class ControlsMenuController : MonoBehaviour
    {

        public void ClosePressed()
        {
            GameManager.Singleton.TakeInput(GameInputType.Escape);
        }
    }
}
