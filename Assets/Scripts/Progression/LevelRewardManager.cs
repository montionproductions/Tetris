using System.Collections.Generic;
using UnityEngine;

public class LevelRewardManager : MonoBehaviour
{
    public static LevelRewardManager I { get; private set; }

    [SerializeField] private List<LevelRewardDefinition> rewards = new();

    private readonly HashSet<string> claimedRewards = new();

    private const string SaveKey = "cbh_claimed_level_rewards";

    public LevelRewardDefinition LastGrantedReward { get; private set; }

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

    private bool subscribedToLevelManager;

    private void Start()
    {
        TrySubscribeToPlayerLevelManager();
    }

    private void OnEnable()
    {
        TrySubscribeToPlayerLevelManager();
    }

    private void OnDisable()
    {
        if (PlayerLevelManager.I != null && subscribedToLevelManager)
        {
            PlayerLevelManager.I.OnLevelUp -= HandleLevelUp;
            subscribedToLevelManager = false;
        }
    }

    private void TrySubscribeToPlayerLevelManager()
    {
        if (subscribedToLevelManager)
            return;

        if (PlayerLevelManager.I == null)
            return;

        PlayerLevelManager.I.OnLevelUp += HandleLevelUp;
        subscribedToLevelManager = true;

        Debug.Log("[LevelReward] Subscribed to PlayerLevelManager.");
    }

    private void HandleLevelUp(int newLevel)
    {
        Debug.Log($"[LevelReward] HandleLevelUp: {newLevel}");

        List<LevelRewardDefinition> rewardsForLevel = GetRewardsForLevel(newLevel);

        Debug.Log($"[LevelReward] Rewards found for level {newLevel}: {rewardsForLevel.Count}");

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

    public void DebugGrantReward(LevelRewardDefinition reward)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        GrantReward(reward);
#endif
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

        if (granted)
        {
            LastGrantedReward = reward;
            Debug.Log($"[LevelReward] LastGrantedReward set: {reward.title} / {reward.rewardId}");
        }
        else
        {
            Debug.LogWarning($"[LevelReward] Reward was not granted: {reward.title} / {reward.rewardId}");
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
        LastGrantedReward = null;
    }
#endif

    public void DebugResetClaimedRewards()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        PlayerPrefs.DeleteKey(SaveKey);
        claimedRewards.Clear();
        LastGrantedReward = null;

        Debug.Log("[Debug] Claimed level rewards reset.");
#endif
    }
}
