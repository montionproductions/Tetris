using System;
using UnityEngine;

public class SoftCurrencyManager : MonoBehaviour
{
    public static SoftCurrencyManager I { get; private set; }

    public int Coins { get; private set; }

    public event Action<int> OnCoinsChanged;

    private const string SaveKey = "cbh_soft_coins";

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        Coins = PlayerPrefs.GetInt(SaveKey, 0);
        OnCoinsChanged?.Invoke(Coins);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        Coins += amount;
        Save();

        OnCoinsChanged?.Invoke(Coins);
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return false;
        if (Coins < amount) return false;

        Coins -= amount;
        Save();

        OnCoinsChanged?.Invoke(Coins);
        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(SaveKey, Coins);
        PlayerPrefs.Save();
    }

    public void DebugResetCurrency()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Coins = 0;
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(Coins);
#endif
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Add 500 Coins")]
    private void DebugAddCoins()
    {
        AddCoins(500);
    }

    [ContextMenu("Debug Reset Coins")]
    private void DebugResetCoins()
    {
        Coins = 0;
        Save();
        OnCoinsChanged?.Invoke(Coins);
    }
#endif
}
