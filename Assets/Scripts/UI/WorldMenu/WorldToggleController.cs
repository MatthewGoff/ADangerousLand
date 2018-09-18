using UnityEngine;
using UnityEngine.UI;

public class WorldToggleController : MonoBehaviour
{
    public int WorldIdentifier;

    public GameObject NameText;
    public GameObject SeedText;

    public void Populate(int worldIdentifier)
    {
        WorldIdentifier = worldIdentifier;
        WorldPersistenceMetaData metaData = WorldPersistenceManager.GetWorldPersistenceMetaData(WorldIdentifier);

        NameText.GetComponent<Text>().text = metaData.Name;
        SeedText.GetComponent<Text>().text = "Seed: " + metaData.Seed.ToString();
    }
}
