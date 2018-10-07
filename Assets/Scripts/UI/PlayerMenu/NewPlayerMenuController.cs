using UnityEngine;
using UnityEngine.UI;

public class NewPlayerMenuController : MonoBehaviour {

    public GameObject BodySprite;
    public GameObject NameField;
    public GameObject ColorSlider;
    public DeathPenaltyType DeathPenalty;

    public void Awake()
    {
        DeathPenalty = DeathPenaltyType.Softcore;
    }

    public void Update()
    {
        BodySprite.GetComponent<Image>().color = Color.HSVToRGB(ColorSlider.GetComponent<Slider>().value, 1, 1);
    }

    public bool NoFocus()
    {
        return !NameField.GetComponent<InputField>().isFocused;
    }

    public void CreatePressed()
    {
        PlayerPersistenceManager.CreatePlayer(NameField.GetComponent<InputField>().text, ColorSlider.GetComponent<Slider>().value, DeathPenalty);
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
