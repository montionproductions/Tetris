using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionDebugMenu : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Info")]
    [SerializeField] private TMP_Text infoText;

    [Header("XP Buttons")]
    [SerializeField] private Button add50XpButton;
    [SerializeField] private Button add100XpButton;
    [SerializeField] private Button add500XpButton;
    [SerializeField] private Button levelUpButton;

    [Header("Reward Buttons")]
    [SerializeField] private Button grantNextRewardButton;
    [SerializeField] private Button unlockAllCompanionsButton;
    [SerializeField] private Button unlockAllPowerUpsButton;

    [Header("Reset Buttons")]
    [SerializeField] private Button resetPlayerLevelButton;
    [SerializeField] private Button resetCompanionsButton;
    [SerializeField] private Button resetPowerUpsButton;
    [SerializeField] private Button resetRoundButton;
    [SerializeField] private Button resetAllButton;


    private void Awake()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        gameObject.SetActive(false);
        return;
#endif

        if (root != null)
            root.SetActive(false);

        RegisterButtons();
    }

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Input.GetKeyDown(KeyCode.F1))
            Toggle();

        RefreshInfo();
#endif
    }

    private void RegisterButtons()
    {
        if (add50XpButton != null)
            add50XpButton.onClick.AddListener(() => AddXp(50));

        if (add100XpButton != null)
            add100XpButton.onClick.AddListener(() => AddXp(100));

        if (add500XpButton != null)
            add500XpButton.onClick.AddListener(() => AddXp(500));

        if (levelUpButton != null)
            levelUpButton.onClick.AddListener(ForceLevelUp);

        if (grantNextRewardButton != null)
            grantNextRewardButton.onClick.AddListener(GrantNextReward);

        if (unlockAllCompanionsButton != null)
            unlockAllCompanionsButton.onClick.AddListener(UnlockAllCompanions);

        if (unlockAllPowerUpsButton != null)
            unlockAllPowerUpsButton.onClick.AddListener(UnlockAllPowerUps);

        if (resetPlayerLevelButton != null)
            resetPlayerLevelButton.onClick.AddListener(ResetPlayerLevel);

        if (resetCompanionsButton != null)
            resetCompanionsButton.onClick.AddListener(ResetCompanions);

        if (resetPowerUpsButton != null)
            resetPowerUpsButton.onClick.AddListener(ResetPowerUps);

        if (resetRoundButton != null)
            resetRoundButton.onClick.AddListener(ResetRound);

        if (resetAllButton != null)
            resetAllButton.onClick.AddListener(ResetAll);
    }

    public void Toggle()
    {
        if (root == null)
            return;

        root.SetActive(!root.activeSelf);
        RefreshInfo();
    }

    private void AddXp(int amount)
    {
        PlayerLevelManager.I?.AddXp(amount);
        RefreshInfo();
    }

    private void ForceLevelUp()
    {
        if (PlayerLevelManager.I == null)
            return;

        int missingXp = PlayerLevelManager.I.GetXpRequiredForNextLevel()
                        - PlayerLevelManager.I.GetCurrentXpInLevel();

        PlayerLevelManager.I.AddXp(Mathf.Max(1, missingXp));
        RefreshInfo();
    }

    private void GrantNextReward()
    {
        if (LevelRewardManager.I == null || PlayerLevelManager.I == null)
            return;

        LevelRewardDefinition nextReward =
            LevelRewardManager.I.GetNextReward(PlayerLevelManager.I.Level - 1);

        if (nextReward == null)
        {
            Debug.Log("[Debug] No next reward found.");
            return;
        }

        LevelRewardManager.I.DebugGrantReward(nextReward);
        RefreshInfo();
    }

    private void UnlockAllCompanions()
    {
        if (CompanionCollectionManager.I == null)
            return;

        foreach (CompanionDefinition companion in CompanionCollectionManager.I.Companions)
        {
            if (companion == null) continue;
            CompanionCollectionManager.I.UnlockCompanionById(companion.id);
        }

        RefreshInfo();
    }

    private void UnlockAllPowerUps()
    {
        if (PowerUpUnlockManager.I == null)
            return;

        foreach (PowerUpDefinition powerUp in PowerUpUnlockManager.I.PowerUps)
        {
            if (powerUp == null) continue;
            PowerUpUnlockManager.I.UnlockPowerUp(powerUp.id);
        }

        RefreshInfo();
    }

    private void ResetPlayerLevel()
    {
        PlayerLevelManager.I?.DebugResetProgression();
        RefreshInfo();
    }

    private void ResetCompanions()
    {
        CompanionCollectionManager.I?.DebugResetCompanions();
        RefreshInfo();
    }

    private void ResetPowerUps()
    {
        PowerUpUnlockManager.I?.DebugResetPowerUps();
        RefreshInfo();
    }

    private void ResetAll()
    {
        PlayerLevelManager.I?.DebugResetProgression();
        RoundFlowManager.I?.DebugResetRounds();

        CompanionCollectionManager.I?.DebugResetCompanions();
        PowerUpUnlockManager.I?.DebugResetPowerUps();
        LevelRewardManager.I?.DebugResetClaimedRewards();
        SoftCurrencyManager.I?.DebugResetCurrency();
        TutorialManager.I?.DebugResetTutorial();

        RefreshInfo();
    }

    private void ResetRound()
    {
        RoundFlowManager.I?.DebugResetRounds();
        RefreshInfo();
    }

    private void RefreshInfo()
    {
        if (infoText == null)
            return;

        if (PlayerLevelManager.I == null)
        {
            infoText.text = "PlayerLevelManager not found.";
            return;
        }

        string roundInfo = "RoundFlowManager not found.";

        if (RoundFlowManager.I != null)
        {
            roundInfo =
                $"Round: {RoundFlowManager.I.CurrentRound}\n" +
                $"Round XP: {RoundFlowManager.I.CurrentRoundXp} / {RoundFlowManager.I.RequiredRoundXp}\n" +
                $"Round State: {RoundFlowManager.I.State}";
        }

        infoText.text =
            $"Player Level: {PlayerLevelManager.I.Level}\n" +
            $"Player XP: {PlayerLevelManager.I.GetCurrentXpInLevel()} / {PlayerLevelManager.I.GetXpRequiredForNextLevel()}\n" +
            $"Total XP: {PlayerLevelManager.I.TotalLifetimeXp}\n" +
            $"Available XP: {PlayerLevelManager.I.AvailableXp}\n\n" +
            roundInfo;
    }
}
