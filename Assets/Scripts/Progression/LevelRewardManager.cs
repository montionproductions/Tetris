using System.Collections.Generic;
using UnityEngine;

public class LevelRewardManager : MonoBehaviour
{
    public static LevelRewardManager I { get; private set; }

    [SerializeField] private List<LevelRewardDefinition> rewards = new();

    private readonly HashSet<string> claimedRewards = new();

    private const string SaveKey = "cbh_claimed_level_rewards";

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    private void OnEnable()
    {
        if (PlayerLevelManager.I != null)
            PlayerLevelManager.I.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        if (PlayerLevelManager.I != null)
            PlayerLevelManager.I.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        List<LevelRewardDefinition> rewardsForLevel = GetRewardsForLevel(newLevel);

        foreach (LevelRewardDefinition reward in rewardsForLevel)
        {
            GrantReward(reward);
        }
    }

    public List<LevelRewardDefinition> GetRewardsForLevel(int level)
    {
        List<LevelRewardDefinition> result = new();

        foreach (LevelRewardDefinition reward in rewards)
        {
            if (reward != null && reward.level == level)
                result.Add(reward);
        }

        return result;
    }

    public LevelRewardDefinition GetNextReward(int currentLevel)
    {
        LevelRewardDefinition best = null;

        foreach (LevelRewardDefinition reward in rewards)
        {
            if (reward == null) continue;
            if (reward.level <= currentLevel) continue;

            if (best == null || reward.level < best.level)
                best = reward;
        }

        return best;
    }

    private void GrantReward(LevelRewardDefinition reward)
    {
        string claimKey = GetClaimKey(reward);

        if (claimedRewards.Contains(claimKey))
            return;

        bool granted = false;

        switch (reward.rewardType)
        {
            case LevelRewardType.Companion:
                granted = CompanionCollectionManager.I != null &&
                          CompanionCollectionManager.I.UnlockCompanionById(reward.rewardId);
                break;

            case LevelRewardType.PowerUp:
                granted = PowerUpUnlockManager.I != null &&
                          PowerUpUnlockManager.I.UnlockPowerUp(reward.rewardId);
                break;

            case LevelRewardType.Outfit:
                granted = CompanionCollectionManager.I != null &&
                          CompanionCollectionManager.I.UnlockOutfitById(reward.rewardId);
                break;

            case LevelRewardType.Currency:
            case LevelRewardType.StoryChapter:
                granted = true;
                break;
        }

        claimedRewards.Add(claimKey);
        Save();

        if (granted && RewardRevealOverlay.I != null)
        {
            RewardRevealOverlay.I.Show(reward);
        }
    }

    private string GetClaimKey(LevelRewardDefinition reward)
    {
        return $"{reward.level}_{reward.rewardType}_{reward.rewardId}";
    }

    private void Save()
    {
        PlayerPrefs.SetString(SaveKey, string.Join("|", claimedRewards));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        claimedRewards.Clear();

        string raw = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(raw))
            return;

        string[] parts = raw.Split('|');

        foreach (string part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
                claimedRewards.Add(part);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Reset Claimed Rewards")]
    private void DebugReset()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        claimedRewards.Clear();
    }
#endif
}