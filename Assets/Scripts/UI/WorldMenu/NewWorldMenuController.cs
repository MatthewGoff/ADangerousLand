﻿using UnityEngine;
using UnityEngine.UI;
using ADL.Core;
using ADL.Persistence;

namespace ADL.UI
{
    public class NewWorldMenuController : MonoBehaviour
    {

        public GameObject NameField;
        public GameObject SeedField;
        public GameObject NaNText;

        public void OnEnable()
        {
            NaNText.SetActive(false);

            System.Random rng = new System.Random();
            SeedField.GetComponent<InputField>().text = rng.Next(-int.MaxValue, int.MaxValue).ToString();
        }

        public bool NoFocus()
        {
            return !NameField.GetComponent<InputField>().isFocused;
        }

        public void CreatePressed()
        {
            int seed;
            if (int.TryParse(SeedField.GetComponent<InputField>().text, out seed))
            {
                int worldIdentifier = WorldPersistenceManager.CreateWorld(NameField.GetComponent<InputField>().text, seed);
                GameManager.Singleton.TakeInput(GameInputType.Escape);
            }
            else
            {
                NaNText.SetActive(true);
            }
        }

        public void BackPressed()
        {
            GameManager.Singleton.TakeInput(GameInputType.Escape);
        }
    }
}