using System.Collections.Generic;
using System;
using UnityEngine;

public class CompanionCollectionManager : MonoBehaviour
{
    public static event Action<CompanionDefinition> OnCompanionUnlocked;
    public static CompanionCollectionManager I { get; private set; }

    [SerializeField] private List<CompanionDefinition> companions = new();

    private readonly Dictionary<string, CompanionSaveData> saveData = new();

    public IReadOnlyList<CompanionDefinition> Companions => companions;

    private const string SaveKey = "cbh_companion_save";

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

    private void Start()
    {
        // Migra partidas anteriores: un companion desbloqueado siempre concede su power-up.
        foreach (CompanionDefinition companion in companions)
            if (companion != null && IsUnlocked(companion.id) && !string.IsNullOrEmpty(companion.linkedPowerUpId))
                PowerUpUnlockManager.I?.UnlockPowerUp(companion.linkedPowerUpId);
    }

    public CompanionDefinition FindCompanionById(string companionId)
    {
        return companions.Find(c => c != null && c.id == companionId);
    }

    public CompanionSaveData GetData(string companionId)
    {
        return GetOrCreateData(companionId);
    }

    public bool IsUnlocked(string companionId)
    {
        return GetOrCreateData(companionId).unlocked;
    }

    public bool UnlockCompanionById(string companionId)
    {
        CompanionDefinition companion = FindCompanionById(companionId);

        if (companion == null)
        {
            Debug.LogWarning($"[Companions] Companion not found: {companionId}");
            return false;
        }

        CompanionSaveData data = GetOrCreateData(companionId);

        if (data.unlocked)
            return false;

        data.unlocked = true;

        if (data.affinityLevel <= 0)
            data.affinityLevel = 1;

        Save();

        if (!string.IsNullOrEmpty(companion.linkedPowerUpId))
            PowerUpUnlockManager.I?.UnlockPowerUp(companion.linkedPowerUpId);

        Debug.Log($"[Companions] Unlocked companion: {companion.displayName}");
        OnCompanionUnlocked?.Invoke(companion);
        return true;
    }

    public bool UnlockOutfitById(string outfitId)
    {
        foreach (CompanionDefinition companion in companions)
        {
            if (companion == null || companion.outfits == null)
                continue;

            foreach (CompanionOutfitDefinition outfit in companion.outfits)
            {
                if (outfit == null) continue;
                if (outfit.outfitId != outfitId) continue;

                CompanionSaveData data = GetOrCreateData(companion.id);

                if (!data.unlockedOutfitIds.Contains(outfitId))
                {
                    data.unlockedOutfitIds.Add(outfitId);
                    Save();
                    Debug.Log($"[Companions] Unlocked outfit: {outfit.displayName}");
                    return true;
                }

                return false;
            }
        }

        Debug.LogWarning($"[Companions] Outfit not found: {outfitId}");
        return false;
    }

    public bool AddAffinityXp(string companionId, int xpAmount)
    {
        if (xpAmount <= 0) return false;

        CompanionSaveData data = GetOrCreateData(companionId);

        if (!data.unlocked)
            return false;

        data.affinityXp += xpAmount;

        bool leveledUp = false;

        while (data.affinityXp >= GetAffinityXpRequired(data.affinityLevel))
        {
            data.affinityXp -= GetAffinityXpRequired(data.affinityLevel);
            data.affinityLevel++;
            leveledUp = true;

            TryUnlockAffinityRewards(companionId, data.affinityLevel);
        }

        Save();
        return leveledUp;
    }

    public bool SpendPlayerXpOnAffinity(string companionId, int amount)
    {
        if (PlayerLevelManager.I == null) return false;
        if (!PlayerLevelManager.I.SpendAvailableXp(amount)) return false;

        AddAffinityXp(companionId, amount);
        return true;
    }

    public int GetAffinityXpRequired(int affinityLevel)
    {
        int safeLevel = Mathf.Max(1, affinityLevel);
        return Mathf.RoundToInt(100 * Mathf.Pow(1.35f, safeLevel - 1));
    }

    private void TryUnlockAffinityRewards(string companionId, int newAffinityLevel)
    {
        CompanionDefinition companion = FindCompanionById(companionId);

        if (companion == null || companion.outfits == null)
            return;

        foreach (CompanionOutfitDefinition outfit in companion.outfits)
        {
            if (outfit.requiredAffinityLevel == newAffinityLevel)
            {
                UnlockOutfitById(outfit.outfitId);
            }
        }
    }

    private CompanionSaveData GetOrCreateData(string id)
    {
        if (saveData.TryGetValue(id, out CompanionSaveData existing))
            return existing;

        CompanionSaveData created = new CompanionSaveData
        {
            id = id,
            unlocked = false,
            affinityLevel = 1,
            affinityXp = 0,
            unlockedOutfitIds = new List<string>()
        };

        saveData[id] = created;
        return created;
    }

    public void Save()
    {
        CompanionSaveWrapper wrapper = new CompanionSaveWrapper
        {
            companions = new List<CompanionSaveData>(saveData.Values)
        };

        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        saveData.Clear();

        string json = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(json))
            return;

        CompanionSaveWrapper wrapper = JsonUtility.FromJson<CompanionSaveWrapper>(json);

        if (wrapper?.companions == null)
            return;

        foreach (CompanionSaveData data in wrapper.companions)
        {
            if (!string.IsNullOrEmpty(data.id))
            {
                if (data.unlockedOutfitIds == null)
                    data.unlockedOutfitIds = new List<string>();

                saveData[data.id] = data;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Reset Companions")]
    private void DebugReset()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        saveData.Clear();
    }
#endif

    public void DebugResetCompanions()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        PlayerPrefs.DeleteKey(SaveKey);
        saveData.Clear();

        Save();

        Debug.Log("[Debug] Companions reset.");
#endif
    }
}
