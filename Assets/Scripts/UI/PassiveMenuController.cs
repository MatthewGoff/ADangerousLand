using UnityEngine;
using UnityEngine.UI;

public class PassiveMenuController : MonoBehaviour {

    public GameObject RemainingPoints;

    public GameObject AttackDamageButton;
    public GameObject AttackSpeedButton;
    public GameObject MovementSpeedButton;
    public GameObject SightRadiusButton;
    public GameObject ProjectileDamageButton;
    public GameObject MaxLifeButton;
    public GameObject MaxStaminaButton;
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
    public GameObject MaxLifeCurrentField;
    public GameObject MaxLifeNextField;
    public GameObject MaxStaminaCurrentField;
    public GameObject MaxStaminaNextField;
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
    private Text MaxLifeCurrentText;
    private Text MaxLifeNextText;
    private Text MaxStaminaCurrentText;
    private Text MaxStaminaNextText;
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
        MaxLifeCurrentText = MaxLifeCurrentField.GetComponent<Text>();
        MaxLifeNextText = MaxLifeNextField.GetComponent<Text>();
        MaxStaminaCurrentText = MaxStaminaCurrentField.GetComponent<Text>();
        MaxStaminaNextText = MaxStaminaNextField.GetComponent<Text>();
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
            MaxLifeButton.SetActive(false);
            MaxStaminaButton.SetActive(false);
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
            MaxLifeButton.SetActive(true);
            MaxStaminaButton.SetActive(true);
            HeathRegenButton.SetActive(true);
            StaminaRegenButton.SetActive(true);
        }

        PlayerManager player = GameManager.Singleton.World.PlayerManager;

        AttackDamageCurrentText.text = Configuration.PLAYER_ATTACK_DAMAGE(player.AttackDamagePoints).ToString();
        AttackDamageNextText.text = Configuration.PLAYER_ATTACK_DAMAGE(player.AttackDamagePoints + 1).ToString();
        AttackSpeedCurrentText.text = Configuration.PLAYER_ATTACK_SPEED(player.AttackSpeedPoints).ToString();
        AttackSpeedNextText.text = Configuration.PLAYER_ATTACK_SPEED(player.AttackSpeedPoints + 1).ToString();
        MoveSpeedCurrentText.text = Configuration.PLAYER_MOVE_SPEED(player.MoveSpeedPoints).ToString();
        MoveSpeedNextText.text = Configuration.PLAYER_MOVE_SPEED(player.MoveSpeedPoints + 1).ToString();
        SightRadiusCurrentText.text = Configuration.PLAYER_SIGHT_RADIUS(player.SightRadiusPoints).ToString();
        SightRadiusNextText.text = Configuration.PLAYER_SIGHT_RADIUS(player.SightRadiusPoints + 1).ToString();
        ProjectileDamageCurrentText.text = (Configuration.PLAYER_PROJECTILE_DAMAGE(player.ProjectileDamagePoints) * 100).ToString();
        ProjectileDamageNextText.text = (Configuration.PLAYER_PROJECTILE_DAMAGE(player.ProjectileDamagePoints + 1) * 100).ToString();
        MaxLifeCurrentText.text = Configuration.PLAYER_MAX_LIFE(player.MaxHealthPoints).ToString();
        MaxLifeNextText.text = Configuration.PLAYER_MAX_LIFE(player.MaxHealthPoints + 1).ToString();
        MaxStaminaCurrentText.text = Configuration.PLAYER_MAX_STAMINA(player.MaxStaminaPoints).ToString();
        MaxStaminaNextText.text = Configuration.PLAYER_MAX_STAMINA(player.MaxStaminaPoints + 1).ToString();
        HealthRegenCurrentText.text = Configuration.PLAYER_LIFE_REGEN(player.HealthRegenPoints).ToString();
        HealthRegenNextText.text = Configuration.PLAYER_LIFE_REGEN(player.HealthRegenPoints + 1).ToString();
        StaminaRegenCurrentText.text = Configuration.PLAYER_STAMINA_REGEN(player.StaminaRegenPoints).ToString();
        StaminaRegenNextText.text = Configuration.PLAYER_STAMINA_REGEN(player.StaminaRegenPoints + 1).ToString();
    }

    public void OnEnable()
    {
        UpdateUI();
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

    public void UpgradeMaxHealth()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeMaxHealth();
        UpdateUI();
    }

    public void UpgradeMaxStamina()
    {
        GameManager.Singleton.World.PlayerManager.UpgradeMaxStamina();
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
