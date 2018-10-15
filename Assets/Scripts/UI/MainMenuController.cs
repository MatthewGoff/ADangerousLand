using UnityEngine;
using ADL.Core;

namespace ADL.UI
{
    public class MainMenuController : MonoBehaviour
    {

        public void PlayPressed()
        {
            GameManager.Singleton.TakeInput(GameInputType.OpenPlayerMenu);
        }

        public void OptionsPressed()
        {

        }

        public void CreditsPressed()
        {

        }

        public void QuitPressed()
        {
            GameManager.Singleton.TakeInput(GameInputType.Escape);
        }
    }
}