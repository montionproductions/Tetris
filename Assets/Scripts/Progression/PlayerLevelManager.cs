using System;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    public static PlayerLevelManager I { get; private set; }

    public int Level { get; private set; } = 1;
    public int TotalLifetimeXp { get; private set; }
    public int AvailableXp { get; private set; }

    public event Action<int> OnLevelUp;
    public event Action<int, int, int> OnXpChanged;
    // currentXpInLevel, requiredXpForNextLevel, level

    [Header("XP Curve")]
    [SerializeField] private int level2RequiredTotalXp = 100;
    [SerializeField] private float levelGrowth = 1.35f;

    private const string SaveKeyLevel = "cbh_level";
    private const string SaveKeyTotalXp = "cbh_total_xp";
    private const string SaveKeyAvailableXp = "cbh_available_xp";

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

    public void AddXp(int amount)
    {
        if (amount <= 0) return;

        int oldLevel = Level;

        TotalLifetimeXp += amount;
        AvailableXp += amount;

        Level = CalculateLevelFromTotalXp(TotalLifetimeXp);

        if (Level > oldLevel)
        {
            for (int lvl = oldLevel + 1; lvl <= Level; lvl++)
            {
                OnLevelUp?.Invoke(lvl);
            }
        }

        Save();
        NotifyXpChanged();
    }

    public bool HasAvailableXp(int amount)
    {
        return AvailableXp >= amount;
    }

    public bool SpendAvailableXp(int amount)
    {
        if (amount <= 0) return false;
        if (AvailableXp < amount) return false;

        AvailableXp -= amount;
        Save();
        NotifyXpChanged();
        return true;
    }

    public int GetTotalXpRequiredForLevel(int level)
    {
        if (level <= 1) return 0;

        int total = 0;

        for (int i = 2; i <= level; i++)
        {
            total += GetXpRequiredFromPreviousLevel(i);
        }

        return total;
    }

    public int GetXpRequiredFromPreviousLevel(int targetLevel)
    {
        if (targetLevel <= 1) return 0;

        return Mathf.RoundToInt(level2RequiredTotalXp * Mathf.Pow(levelGrowth, targetLevel - 2));
    }

    public int GetCurrentXpInLevel()
    {
        int currentLevelStartXp = GetTotalXpRequiredForLevel(Level);
        return Mathf.Max(0, TotalLifetimeXp - currentLevelStartXp);
    }

    public int GetXpRequiredForNextLevel()
    {
        return GetXpRequiredFromPreviousLevel(Level + 1);
    }

    private int CalculateLevelFromTotalXp(int totalXp)
    {
        int calculatedLevel = 1;

        while (totalXp >= GetTotalXpRequiredForLevel(calculatedLevel + 1))
        {
            calculatedLevel++;
        }

        return calculatedLevel;
    }

    private void NotifyXpChanged()
    {
        OnXpChanged?.Invoke(
            GetCurrentXpInLevel(),
            GetXpRequiredForNextLevel(),
            Level
        );
    }

    private void Save()
    {
        PlayerPrefs.SetInt(SaveKeyLevel, Level);
        PlayerPrefs.SetInt(SaveKeyTotalXp, TotalLifetimeXp);
        PlayerPrefs.SetInt(SaveKeyAvailableXp, AvailableXp);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        TotalLifetimeXp = PlayerPrefs.GetInt(SaveKeyTotalXp, 0);
        AvailableXp = PlayerPrefs.GetInt(SaveKeyAvailableXp, 0);
        Level = CalculateLevelFromTotalXp(TotalLifetimeXp);

        NotifyXpChanged();
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Add 100 XP")]
    private void DebugAdd100Xp()
    {
        AddXp(100);
    }

    [ContextMenu("Debug Reset Progression")]
    private void DebugReset()
    {
        PlayerPrefs.DeleteKey(SaveKeyLevel);
        PlayerPrefs.DeleteKey(SaveKeyTotalXp);
        PlayerPrefs.DeleteKey(SaveKeyAvailableXp);

        Level = 1;
        TotalLifetimeXp = 0;
        AvailableXp = 0;

        NotifyXpChanged();
    }
#endif
}