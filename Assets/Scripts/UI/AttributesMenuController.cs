using UnityEngine;
using UnityEngine.UI;

public class AttributesMenuController : MonoBehaviour {

    public GameObject RemainingPointsText;

    public GameObject AttackDamageButton;
    public GameObject AttackSpeedButton;
    public GameObject MovementSpeedButton;
    public GameObject SightRadiusButton;
    public GameObject ProjectileDamageButton;
    public GameObject MaxLifeButton;
    public GameObject MaxStaminaButton;
    public GameObject HeathRegenButton;
    public GameObject StaminaRegenButton;

    public GameObject AttackDamageCurrentText;
    public GameObject AttackSpeedCurrentText;
    public GameObject MoveSpeedCurrentText;
    public GameObject SightRadiusCurrentText;
    public GameObject ProjectileDamageCurrentText;
    public GameObject MaxHealthCurrentText;
    public GameObject MaxStaminaCurrentText;
    public GameObject HealthRegenCurrentText;
    public GameObject StaminaRegenCurrentText;

    public GameObject StatTooltip;
    public GameObject StatTooltipTitleText;
    public GameObject StatTooltipDescriptionText;
    public GameObject StatTooltipAsymptoteText;
    public GameObject StatTooltipPointsInvestedText;
    public GameObject StatTooltipBaseValueText;
    public GameObject StatTooltipEffectiveValueText;

    public GameObject ButtonTooltip;
    public GameObject ButtonTooltipCurrentValueText;
    public GameObject ButtonTooltipNextValueText;

    public GameObject HeadSprite;
    public GameObject BodySprite;
    public GameObject WeaponSprite;
    public GameObject NameText;
    public GameObject LevelText;
    public GameObject DeathPenaltyText;

    private PlayerManager Player;

    public void Awake()
    {
        HeadSprite.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        BodySprite.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        WeaponSprite.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void UpdateUI ()
    {
        NameText.GetComponent<Text>().text = Player.Name;
        BodySprite.GetComponent<Image>().color = Color.HSVToRGB(Player.Color, 1, 1);
        LevelText.GetComponent<Text>().text = "Level " + Player.Level.ToString() + " Adventurer";
        if (Player.DeathPenalty == DeathPenaltyType.Softcore)
        {
            DeathPenaltyText.GetComponent<Text>().text = "Softcore";
        }
        else if (Player.DeathPenalty == DeathPenaltyType.Normalcore)
        {
            DeathPenaltyText.GetComponent<Text>().text = "Normalcore";
        }
        else if (Player.DeathPenalty == DeathPenaltyType.Hardcore)
        {
            DeathPenaltyText.GetComponent<Text>().text = "Hardcore";
        }

        if (GameManager.Singleton.World != null && GameManager.Singleton.World.PlayerManager.PassivePoints == 0)
        {
            RemainingPointsText.SetActive(false);

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
            RemainingPointsText.SetActive(true);
            RemainingPointsText.GetComponent<Text>().text = GameManager.Singleton.World.PlayerManager.PassivePoints.ToString() + " Points Remaining!";

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

        AttackDamageCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_DAMAGE(Player.AttackDamagePoints), 0.1f).ToString();
        AttackSpeedCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_SPEED(Player.AttackSpeedPoints), 0.1f).ToString();
        MoveSpeedCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MOVE_SPEED(Player.MoveSpeedPoints), 0.1f).ToString();
        SightRadiusCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_SIGHT_RADIUS(Player.SightRadiusPoints), 0.1f).ToString();
        ProjectileDamageCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_PROJECTILE_DAMAGE(Player.ProjectileDamagePoints) * 100, 0.1f).ToString() + "%";
        MaxHealthCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_HEALTH(Player.MaxHealthPoints), 0.1f).ToString();
        MaxStaminaCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_STAMINA(Player.MaxStaminaPoints), 0.1f).ToString();
        HealthRegenCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_HEALTH_REGEN(Player.HealthRegenPoints), 0.01f).ToString();
        StaminaRegenCurrentText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_STAMINA_REGEN(Player.StaminaRegenPoints), 0.1f).ToString();
    }

    public void OnEnable()
    {
        Player = GameManager.Singleton.World.PlayerManager;
        UpdateUI();
    }

    public void Update()
    {
        Vector2 mousePosition = new Vector2(Mathf.Round(Input.mousePosition.x), Mathf.Round(Input.mousePosition.y));
        StatTooltip.GetComponent<RectTransform>().position = mousePosition;
        ButtonTooltip.GetComponent<RectTransform>().position = mousePosition;
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

    public void OnCursorExit()
    {
        StatTooltip.SetActive(false);
        ButtonTooltip.SetActive(false);
    }

    public void OnCursorEnterAttackDamage()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Attack Damage";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Damage dealt with attacks";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "(points + 10)";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.AttackDamagePoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_DAMAGE(Player.AttackDamagePoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_DAMAGE(Player.AttackDamagePoints), 0.1f).ToString();
    }

    public void OnCursorEnterAttackSpeed()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Attack Speed";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Maximum attacks per second";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "60";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.AttackSpeedPoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_SPEED(Player.AttackSpeedPoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_SPEED(Player.AttackSpeedPoints), 0.1f).ToString();
    }

    public void OnCursorEnterMoveSpeed()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Move Speed";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Tiles traversed per second";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "30";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.MoveSpeedPoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MOVE_SPEED(Player.MoveSpeedPoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MOVE_SPEED(Player.MoveSpeedPoints), 0.1f).ToString();
    }

    public void OnCursorEnterSightRadius()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Sight Radius";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Radius of fully visable tiles around player";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "20";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.SightRadiusPoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_SIGHT_RADIUS(Player.SightRadiusPoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_SIGHT_RADIUS(Player.SightRadiusPoints), 0.1f).ToString();
    }

    public void OnCursorEnterProjectileDamage()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Projectile Damage";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Percentage of (Attack Damage) dealt by projectiles";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "100%";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.ProjectileDamagePoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_PROJECTILE_DAMAGE(Player.ProjectileDamagePoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_PROJECTILE_DAMAGE(Player.ProjectileDamagePoints), 0.1f).ToString();
    }

    public void OnCursorEnterMaxHealth()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Maximum Health";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Damage you can take before dying";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "(points + 30)";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.MaxHealthPoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_HEALTH(Player.MaxHealthPoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_HEALTH(Player.MaxHealthPoints), 0.1f).ToString();
    }

    public void OnCursorEnterMaxStamina()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Maximum Stamina";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Stamina you can spend on abilities";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "(points + 30)";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.MaxStaminaPoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_STAMINA(Player.MaxStaminaPoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_STAMINA(Player.MaxStaminaPoints), 0.1f).ToString();
    }

    public void OnCursorEnterHealthRegen()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Health Regen";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Health recovered per second";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "5";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.HealthRegenPoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_HEALTH_REGEN(Player.HealthRegenPoints), 0.01f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_HEALTH_REGEN(Player.HealthRegenPoints), 0.01f).ToString();
    }

    public void OnCursorEnterStaminaRegen()
    {
        StatTooltip.SetActive(true);
        StatTooltipTitleText.GetComponent<Text>().text = "Stamina Regen";
        StatTooltipDescriptionText.GetComponent<Text>().text = "Stamina recovered per second";
        StatTooltipAsymptoteText.GetComponent<Text>().text = "2";
        StatTooltipPointsInvestedText.GetComponent<Text>().text = Player.StaminaRegenPoints.ToString();
        StatTooltipBaseValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_STAMINA_REGEN(Player.StaminaRegenPoints), 0.1f).ToString();
        StatTooltipEffectiveValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_STAMINA_REGEN(Player.StaminaRegenPoints), 0.1f).ToString();
    }

    public void OnCursorEnterAttackDamageButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_DAMAGE(Player.AttackDamagePoints), 0.1f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_DAMAGE(Player.AttackDamagePoints + 1), 0.1f).ToString();
    }

    public void OnCursorEnterAttackSpeedButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_SPEED(Player.AttackSpeedPoints), 0.1f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_ATTACK_SPEED(Player.AttackSpeedPoints + 1), 0.1f).ToString();
    }

    public void OnCursorEnterMoveSeedButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MOVE_SPEED(Player.MoveSpeedPoints), 0.1f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MOVE_SPEED(Player.MoveSpeedPoints + 1), 0.1f).ToString();
    }

    public void OnCursorEnterSightRadiusButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_SIGHT_RADIUS(Player.SightRadiusPoints), 0.1f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_SIGHT_RADIUS(Player.SightRadiusPoints + 1), 0.1f).ToString();
    }

    public void OnCursorEnterProjectileDamageButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_PROJECTILE_DAMAGE(Player.ProjectileDamagePoints) * 100, 0.1f).ToString() + "%";
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_PROJECTILE_DAMAGE(Player.ProjectileDamagePoints + 1) * 100, 0.1f).ToString() + "%";
    }

    public void OnCursorEnterMaxHealthButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_HEALTH(Player.MaxHealthPoints), 0.1f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_HEALTH(Player.MaxHealthPoints + 1), 0.1f).ToString();
    }
    
    public void OnCursorEnterMaxStaminaButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_STAMINA(Player.MaxStaminaPoints), 0.1f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_MAX_STAMINA(Player.MaxStaminaPoints + 1), 0.1f).ToString();
    }

    public void OnCursorEnterHealthRegenButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_HEALTH_REGEN(Player.HealthRegenPoints), 0.01f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_HEALTH_REGEN(Player.HealthRegenPoints + 1), 0.01f).ToString();
    }

    public void OnCursorEnterStaminaRegenButton()
    {
        ButtonTooltip.SetActive(true);
        ButtonTooltipCurrentValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_STAMINA_REGEN(Player.StaminaRegenPoints), 0.1f).ToString();
        ButtonTooltipNextValueText.GetComponent<Text>().text = Util.Round(Configuration.PLAYER_STAMINA_REGEN(Player.StaminaRegenPoints + 1), 0.1f).ToString();
    }
}
