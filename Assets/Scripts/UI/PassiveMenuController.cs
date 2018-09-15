using UnityEngine;
using UnityEngine.UI;

public class PassiveMenuController : MonoBehaviour {

    public GameObject RemainingPoints;

    public GameObject AttackDamageButton;
    public GameObject AttackSpeedButton;
    public GameObject MovementSpeedButton;
    public GameObject SightRadiusButton;
    public GameObject ProjectileDamageButton;
    public GameObject MeleeAoeButton;
    public GameObject HeathRegenButton;
    public GameObject StaminaRegenButton;

    public GameObject AttackDamageCurrentField;
    public GameObject AttackDamageNextField;
    public GameObject AttackSpeedCurrentField;
    public GameObject AttackSpeedNextField;
    public GameObject MoveSpeedCurrentField;
    public GameObject MoveSpeedNextField;
    public GameObject SightRadiusCurrentField;
    public GameObject SightRadiusNextField;
    public GameObject ProjectileDamageCurrentField;
    public GameObject ProjectileDamageNextField;
    public GameObject MeleeAoeCurrentField;
    public GameObject MeleeAoeNextField;
    public GameObject HealthRegenCurrentField;
    public GameObject HealthRegenNextField;
    public GameObject StaminaRegenCurrentField;
    public GameObject StaminaRegenNextField;

    private Text AttackDamageCurrentText;
    private Text AttackDamageNextText;
    private Text AttackSpeedCurrentText;
    private Text AttackSpeedNextText;
    private Text MoveSpeedCurrentText;
    private Text MoveSpeedNextText;
    private Text SightRadiusCurrentText;
    private Text SightRadiusNextText;
    private Text ProjectileDamageCurrentText;
    private Text ProjectileDamageNextText;
    private Text MeleeAoeCurrentText;
    private Text MeleeAoeNextText;
    private Text HealthRegenCurrentText;
    private Text HealthRegenNextText;
    private Text StaminaRegenCurrentText;
    private Text StaminaRegenNextText;

    private bool Initialized = false;

    private void Initialize () {
        AttackDamageCurrentText = AttackDamageCurrentField.GetComponent<Text>();
        AttackDamageNextText = AttackDamageNextField.GetComponent<Text>();
        AttackSpeedCurrentText = AttackSpeedCurrentField.GetComponent<Text>();
        AttackSpeedNextText = AttackSpeedNextField.GetComponent<Text>();
        MoveSpeedCurrentText = MoveSpeedCurrentField.GetComponent<Text>();
        MoveSpeedNextText = MoveSpeedNextField.GetComponent<Text>();
        SightRadiusCurrentText = SightRadiusCurrentField.GetComponent<Text>();
        SightRadiusNextText = SightRadiusNextField.GetComponent<Text>();
        ProjectileDamageCurrentText = ProjectileDamageCurrentField.GetComponent<Text>();
        ProjectileDamageNextText = ProjectileDamageNextField.GetComponent<Text>();
        MeleeAoeCurrentText = MeleeAoeCurrentField.GetComponent<Text>();
        MeleeAoeNextText = MeleeAoeNextField.GetComponent<Text>();
        HealthRegenCurrentText = HealthRegenCurrentField.GetComponent<Text>();
        HealthRegenNextText = HealthRegenNextField.GetComponent<Text>();
        StaminaRegenCurrentText = StaminaRegenCurrentField.GetComponent<Text>();
        StaminaRegenNextText = StaminaRegenNextField.GetComponent<Text>();

	}

	public void UpdateUI () {
        if (!Initialized)
        {
            Initialize();
            Initialized = true;
        }

        if (GameManager.Singleton.World.PlayerManager.PassivePoints == 0)
        {
            RemainingPoints.SetActive(false);

            AttackDamageButton.SetActive(false);
            AttackSpeedButton.SetActive(false);
            MovementSpeedButton.SetActive(false);
            SightRadiusButton.SetActive(false);
            ProjectileDamageButton.SetActive(false);
            MeleeAoeButton.SetActive(false);
            HeathRegenButton.SetActive(false);
            StaminaRegenButton.SetActive(false);
        }
        else
        {
            RemainingPoints.SetActive(true);
            RemainingPoints.GetComponent<Text>().text = GameManager.Singleton.World.PlayerManager.PassivePoints.ToString() + " Points Remaining!";

            AttackDamageButton.SetActive(true);
            AttackSpeedButton.SetActive(true);
            MovementSpeedButton.SetActive(true);
            SightRadiusButton.SetActive(true);
            ProjectileDamageButton.SetActive(true);
            MeleeAoeButton.SetActive(true);
            HeathRegenButton.SetActive(true);
            StaminaRegenButton.SetActive(true);
        }


        AttackDamageCurrentText.text = GameManager.Singleton.World.PlayerManager.AttackDamage.ToString();
        AttackDamageNextText.text = GameManager.Singleton.World.PlayerManager.GetNextAttackDamage().ToString();
        AttackSpeedCurrentText.text = GameManager.Singleton.World.PlayerManager.AttackSpeed.ToString();
        AttackSpeedNextText.text = GameManager.Singleton.World.PlayerManager.GetNextAttackSpeed().ToString();
        MoveSpeedCurrentText.text = GameManager.Singleton.World.PlayerManager.MoveSpeed.ToString();
        MoveSpeedNextText.text = GameManager.Singleton.World.PlayerManager.GetNextMoveSpeed().ToString();
        SightRadiusCurrentText.text = GameManager.Singleton.World.PlayerManager.SightRadiusNear.ToString();
        SightRadiusNextText.text = GameManager.Singleton.World.PlayerManager.GetNextSightRadius().ToString();
        ProjectileDamageCurrentText.text = (GameManager.Singleton.World.PlayerManager.ProjectileDamage*100).ToString();
        ProjectileDamageNextText.text = (GameManager.Singleton.World.PlayerManager.GetNextProjectileDamage()*100).ToString();
        MeleeAoeCurrentText.text = GameManager.Singleton.World.PlayerManager.MeleeAoe.ToString();
        MeleeAoeNextText.text = GameManager.Singleton.World.PlayerManager.GetNextMeleeAoe().ToString();
        HealthRegenCurrentText.text = GameManager.Singleton.World.PlayerManager.HealthRegen.ToString();
        HealthRegenNextText.text = GameManager.Singleton.World.PlayerManager.GetNextHealthRegen().ToString();
        StaminaRegenCurrentText.text = GameManager.Singleton.World.PlayerManager.StaminaRegen.ToString();
        StaminaRegenNextText.text = GameManager.Singleton.World.PlayerManager.GetNextStaminaRegen().ToString();
    }

    public void UpgradeAttackDamage()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeAttackDamage();
        UpdateUI();
    }

    public void UpgradeAttackSpeed()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeAttackSpeed();
        UpdateUI();
    }

    public void UpgradeMoveSpeed()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeMoveSpeed();
        UpdateUI();
    }

    public void UpgradeSightRadius()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeSightRadius();
        UpdateUI();
    }

    public void UpgradeProjectileDamage()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeProjectileDamage();
        UpdateUI();
    }

    public void UpgradeMeleeAoe()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeMeleeAoe();
        UpdateUI();
    }

    public void UpgradeHealthRegen()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeHealthRegen();
        UpdateUI();
    }

    public void UpgradeStaminaRegen()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeStaminaRegen();
        UpdateUI();
    }
}
