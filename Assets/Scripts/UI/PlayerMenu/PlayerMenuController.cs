using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerMenuController : MonoBehaviour {

    public GameObject ContentPort;
    public GameObject ToggleGroup;
    public GameObject CharacterTogglePrefab;
    public List<GameObject> CharacterToggles;
    public PlayerManager SelectedPlayer;

    public void Awake()
    {
        CharacterToggles = new List<GameObject>();
    }

    public void OnEnable()
    {
        UpdateCharacterToggles();
    }

    public void BackPressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.Escape);
    }

    public void CreatePressed()
    {
        GameManager.Singleton.TakeInput(GameInputType.OpenNewPlayerMenu);
    }

    public void DeletePressed()
    {
        GameObject toDelete = null;
        foreach (Toggle toggle in ToggleGroup.GetComponent<ToggleGroup>().ActiveToggles())
        {
            toDelete = toggle.gameObject;
            PlayerPersistenceManager.DeletePlayer(toDelete.GetComponent<CharacterToggleController>().PlayerIdentifier);
        }

        CharacterToggles.Remove(toDelete);
        Destroy(toDelete);
        UpdateCharacterToggles();
    }

    public void NextPressed()
    {
        SelectedPlayer = GetSelectedPlayer();
        GameManager.Singleton.TakeInput(GameInputType.OpenWorldMenu);
    }

    private PlayerManager GetSelectedPlayer()
    {
        PlayerManager playerManager = null;
        foreach (Toggle toggle in ToggleGroup.GetComponent<ToggleGroup>().ActiveToggles())
        {
            int playerIdentifier = toggle.GetComponent<CharacterToggleController>().PlayerIdentifier;
            playerManager = PlayerPersistenceManager.LoadPlayer(playerIdentifier);
        }
        return playerManager;
    }

    public void UpdateCharacterToggles()
    {
        foreach(GameObject characterToggle in CharacterToggles)
        {
            Destroy(characterToggle);
        }
        CharacterToggles.Clear();

        List<int> playerIdentifiers = PlayerPersistenceManager.GetPlayerIdentifiers();
        int i = 0;
        int characterToggleHeight = 84;
        int selectionWindowHeight = 339;
        foreach(int PlayerIdentifier in playerIdentifiers)
        {
            GameObject newCharacterToggle = Instantiate(CharacterTogglePrefab, Vector3.zero, Quaternion.identity, ContentPort.transform);
            newCharacterToggle.GetComponent<CharacterToggleController>().Populate(PlayerIdentifier);
            newCharacterToggle.GetComponent<Toggle>().group = ToggleGroup.GetComponent<ToggleGroup>();
            newCharacterToggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(256, - 10 - (i * (characterToggleHeight+10)));
            CharacterToggles.Add(newCharacterToggle);
            i++;
        }
        RectTransform contentRect = ContentPort.GetComponent<RectTransform>();
        float contentHeight = Mathf.Max(10 + (i * (characterToggleHeight + 10)), selectionWindowHeight);
        contentRect.sizeDelta = new Vector2(17, contentHeight);
    }
}
