using UnityEngine;
using UnityEngine.UI;

public class CharacterToggleController : MonoBehaviour {

    public int PlayerIdentifier;

    public GameObject NameText;
    public GameObject LevelText;
    public GameObject DeathPenaltyText;

    public void Populate(int playerIdentifier)
    {
        PlayerIdentifier = playerIdentifier;
        PlayerPersistenceMetaData metaData = PlayerPersistenceManager.GetPlayerPersistenceMetaData(PlayerIdentifier);

        NameText.GetComponent<Text>().text = metaData.Name;
        LevelText.GetComponent<Text>().text = "Level: " + metaData.Level.ToString();
        if (metaData.DeathPenalty == DeathPenaltyType.Softcore)
        {
            DeathPenaltyText.GetComponent<Text>().text = "Softcore";
        }
        else if (metaData.DeathPenalty == DeathPenaltyType.Normalcore)
        {
            DeathPenaltyText.GetComponent<Text>().text = "Normalcore";
        }
        else if (metaData.DeathPenalty == DeathPenaltyType.Hardcore)
        {
            DeathPenaltyText.GetComponent<Text>().text = "Hardcore";
        }
    }
}
