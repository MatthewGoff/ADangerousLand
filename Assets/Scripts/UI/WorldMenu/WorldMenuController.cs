using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ADL
{
    public class WorldMenuController : MonoBehaviour
    {

        public GameObject ContentPort;
        public GameObject ToggleGroup;
        public GameObject WorldTogglePrefab;
        public List<GameObject> WorldToggles;
        public World SelectedWorld;
        public GameObject NextButton;
        public GameObject DeleteButton;

        public void Awake()
        {
            WorldToggles = new List<GameObject>();
        }

        public void OnEnable()
        {
            UpdateWorldToggles();
        }

        public void BackPressed()
        {
            GameManager.Singleton.TakeInput(GameInputType.Escape);
        }

        public void CreatePressed()
        {
            GameManager.Singleton.TakeInput(GameInputType.OpenNewWorldMenu);
        }

        public void DeletePressed()
        {
            GameObject toDelete = null;
            foreach (Toggle toggle in ToggleGroup.GetComponent<ToggleGroup>().ActiveToggles())
            {
                toDelete = toggle.gameObject;
                WorldPersistenceManager.DeleteWorld(toDelete.GetComponent<WorldToggleController>().WorldIdentifier);
            }

            WorldToggles.Remove(toDelete);
            Destroy(toDelete);
            UpdateWorldToggles();
        }

        public void PlayPressed()
        {
            SelectedWorld = GetSelectedWorld();
            GameManager.Singleton.TakeInput(GameInputType.StartPlay);
        }

        private World GetSelectedWorld()
        {
            World world = null;
            foreach (Toggle toggle in ToggleGroup.GetComponent<ToggleGroup>().ActiveToggles())
            {
                int worldIdentifier = toggle.GetComponent<WorldToggleController>().WorldIdentifier;
                world = WorldPersistenceManager.LoadWorld(worldIdentifier);
            }
            return world;
        }

        public void Update()
        {
            if (ToggleGroup.GetComponent<ToggleGroup>().AnyTogglesOn())
            {
                NextButton.GetComponent<Button>().interactable = true;
                DeleteButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                NextButton.GetComponent<Button>().interactable = false;
                DeleteButton.GetComponent<Button>().interactable = false;
            }
        }

        public void SelectWorld(int toSelect)
        {
            foreach (GameObject worldToggle in WorldToggles)
            {
                if (worldToggle.GetComponent<WorldToggleController>().WorldIdentifier == toSelect)
                {
                    worldToggle.GetComponent<Toggle>().isOn = true;
                }
            }
        }

        public void UpdateWorldToggles()
        {
            foreach (GameObject worldToggle in WorldToggles)
            {
                Destroy(worldToggle);
            }
            WorldToggles.Clear();

            List<int> worldIdentifiers = WorldPersistenceManager.GetWorldIdentifiers();
            int i = 0;
            int worldToggleHeight = 84;
            int selectionWindowHeight = 339;
            foreach (int worldIdentifier in worldIdentifiers)
            {
                GameObject newWorldToggle = Instantiate(WorldTogglePrefab, Vector3.zero, Quaternion.identity, ContentPort.transform);
                newWorldToggle.GetComponent<WorldToggleController>().Populate(worldIdentifier);
                newWorldToggle.GetComponent<Toggle>().group = ToggleGroup.GetComponent<ToggleGroup>();
                newWorldToggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(256, -10 - (i * (worldToggleHeight + 10)));
                WorldToggles.Add(newWorldToggle);
                i++;
            }
            RectTransform contentRect = ContentPort.GetComponent<RectTransform>();
            float contentHeight = Mathf.Max(10 + (i * (worldToggleHeight + 10)), selectionWindowHeight);
            contentRect.sizeDelta = new Vector2(17, contentHeight);

            if (WorldToggles.Count != 0)
            {
                WorldToggles.First().GetComponent<Toggle>().isOn = true;
            }
        }
    }
}