using UnityEngine;
using UnityEngine.UI;
using System;

public class NewWorldMenuController : MonoBehaviour
{

    public GameObject WorldMenu;

    public GameObject NameField;
    public GameObject SeedField;

    public void CreatePressed()
    {
        int worldIdentifier = WorldPersistenceManager.CreateWorld(NameField.GetComponent<InputField>().text, Int32.Parse(SeedField.GetComponent<InputField>().text));
        WorldMenu.GetComponent<WorldMenuController>().UpdateWorldToggles();
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }
}
