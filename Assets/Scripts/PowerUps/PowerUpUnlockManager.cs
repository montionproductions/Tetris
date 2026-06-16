using System.Collections.Generic;
using UnityEngine;

public class PowerUpUnlockManager : MonoBehaviour
{
    public static PowerUpUnlockManager I { get; private set; }

    [SerializeField] private List<PowerUpDefinition> powerUps = new();
    public IReadOnlyList<PowerUpDefinition> PowerUps => powerUps;

    private readonly HashSet<string> unlockedPowerUps = new();

    private const string SaveKey = "cbh_unlocked_powerups";

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

    public PowerUpDefinition FindPowerUpById(string id)
    {
        return powerUps.Find(p => p != null && p.id == id);
    }

    public bool IsUnlocked(string id)
    {
        return unlockedPowerUps.Contains(id);
    }

    public bool UnlockPowerUp(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        PowerUpDefinition powerUp = FindPowerUpById(id);

        if (powerUp == null)
        {
            Debug.LogWarning($"[PowerUps] Power-up not found: {id}");
            return false;
        }

        if (unlockedPowerUps.Contains(id))
            return false;

        unlockedPowerUps.Add(id);
        Save();

        Debug.Log($"[PowerUps] Unlocked: {powerUp.displayName}");
        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetString(SaveKey, string.Join("|", unlockedPowerUps));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        unlockedPowerUps.Clear();

        string raw = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(raw))
            return;

        string[] parts = raw.Split('|');

        foreach (string part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
                unlockedPowerUps.Add(part);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Reset PowerUps")]
    private void DebugReset()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        unlockedPowerUps.Clear();
    }
#endif

    public void DebugResetPowerUps()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        PlayerPrefs.DeleteKey(SaveKey);
        unlockedPowerUps.Clear();

        Debug.Log("[Debug] Power-ups reset.");
#endif
    }
}