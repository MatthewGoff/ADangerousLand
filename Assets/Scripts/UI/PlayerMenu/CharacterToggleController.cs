using UnityEngine;
using UnityEngine.UI;

namespace ADL
{
    public class CharacterToggleController : MonoBehaviour
    {
        public int PlayerIdentifier;

        public GameObject Image;
        public GameObject NameText;
        public GameObject LevelText;
        public GameObject DeathPenaltyText;

        public void Populate(int playerIdentifier)
        {
            PlayerIdentifier = playerIdentifier;
            PlayerPersistenceMetaData metaData = PlayerPersistenceManager.GetPlayerPersistenceMetaData(PlayerIdentifier);

            NameText.GetComponent<Text>().text = metaData.Name;
            Image.GetComponent<Image>().color = Color.HSVToRGB(metaData.Color, 1, 1);
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
}