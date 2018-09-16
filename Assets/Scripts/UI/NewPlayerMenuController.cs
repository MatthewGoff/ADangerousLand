using UnityEngine;
using UnityEngine.UI;

public class NewPlayerMenuController : MonoBehaviour {

    public GameObject PlayerMenu;

    public GameObject NameField;
    public DeathPenaltyType DeathPenalty;

    public void Awake()
    {
        DeathPenalty = DeathPenaltyType.Softcore;
    }

    public void CreatePressed()
    {
        int playerIdentifier = PlayerPersistenceManager.CreatePlayer(NameField.GetComponent<InputField>().text, DeathPenalty);
        PlayerMenu.GetComponent<PlayerMenuController>().UpdateCharacterToggles();
        PlayerMenu.GetComponent<PlayerMenuController>().SelectPlayer(playerIdentifier);
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void SoftcoreSelected()
    {
        DeathPenalty = DeathPenaltyType.Softcore;
    }

    public void NormalcoreSelected()
    {
        DeathPenalty = DeathPenaltyType.Normalcore;
    }

    public void HardcoreSelected()
    {
        DeathPenalty = DeathPenaltyType.Hardcore;
    }
}
