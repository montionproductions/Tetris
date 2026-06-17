using System;
using UnityEngine;

public class RoundFlowManager : MonoBehaviour
{
    public static RoundFlowManager I { get; private set; }

    public RoundState State { get; private set; } = RoundState.None;

    public int CurrentRound { get; private set; } = 1;
    public int CurrentRoundXp { get; private set; }
    public int RequiredRoundXp { get; private set; }

    public event Action<int, int> OnRoundXpChanged;
    public event Action<int> OnRoundStarted;
    public event Action<int> OnRoundCompleted;

    [Header("Round XP")]
    [SerializeField] private int baseRequiredXp = 100;
    [SerializeField] private float requiredXpGrowth = 1.18f;

    [Header("Rewards")]
    [SerializeField] private int baseCoinsPerRound = 40;

    private const string SaveKeyRound = "cbh_current_round";

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
        StartRound();
    }

    public void StartRound()
    {
        State = RoundState.Playing;

        CurrentRoundXp = 0;
        RequiredRoundXp = GetRequiredXpForRound(CurrentRound);

        OnRoundStarted?.Invoke(CurrentRound);
        OnRoundXpChanged?.Invoke(CurrentRoundXp, RequiredRoundXp);

        Debug.Log($"[RoundFlow] Started round {CurrentRound}");
    }

    public void AddRoundXp(int amount)
    {
        if (State != RoundState.Playing)
            return;

        if (amount <= 0)
            return;

        CurrentRoundXp += amount;

        if (CurrentRoundXp >= RequiredRoundXp)
        {
            CurrentRoundXp = RequiredRoundXp;
            OnRoundXpChanged?.Invoke(CurrentRoundXp, RequiredRoundXp);
            CompleteRound();
            return;
        }

        OnRoundXpChanged?.Invoke(CurrentRoundXp, RequiredRoundXp);
    }

    private void CompleteRound()
    {
        if (State != RoundState.Playing)
            return;

        State = RoundState.Completing;

        Debug.Log($"[RoundFlow] Completed round {CurrentRound}");

        // Sube nivel global exactamente al terminar ronda.
        PlayerLevelManager.I?.AddXp(GetPlayerXpForCompletedRound());

        // Da monedas base.
        SoftCurrencyManager.I?.AddCoins(GetCoinsForCompletedRound());

        OnRoundCompleted?.Invoke(CurrentRound);

        // Pausa gameplay.
        PauseGameplayForReward();

        // Mostrar recompensa de nivel si existe.
        ShowRoundCompleteFlow();
    }

    private void ShowRoundCompleteFlow()
    {
        State = RoundState.Reward;

        if (RoundCompleteOverlay.I != null)
        {
            RoundCompleteOverlay.I.Show(
                CurrentRound,
                GetCoinsForCompletedRound(),
                ContinueToUpgradePhase
            );
        }
        else
        {
            ContinueToUpgradePhase();
        }
    }

    public void ContinueToUpgradePhase()
    {
        State = RoundState.Upgrade;

        if (CompanionUpgradeOfferOverlay.I != null)
        {
            CompanionUpgradeOfferOverlay.I.Show(ContinueToNextRound);
        }
        else
        {
            ContinueToNextRound();
        }
    }

    public void ContinueToNextRound()
    {
        CurrentRound++;
        Save();

        ResumeGameplayForNextRound();
        StartRound();
    }

    private void PauseGameplayForReward()
    {
        ShakeCamera.StopShake();

        Game.isPaused = true;
        Time.timeScale = 0f;
    }

    private void ResumeGameplayForNextRound()
    {
        Game.isPaused = false;
        Time.timeScale = 1f;

        // Aquí puedes resetear tablero o iniciar una nueva partida.
        if (GridGenerator.grid != null)
            GridGenerator.DeleteAllBoxes();

        Game._linesCounter = 0;
    }

    private int GetRequiredXpForRound(int round)
    {
        return Mathf.RoundToInt(baseRequiredXp * Mathf.Pow(requiredXpGrowth, round - 1));
    }

    private int GetPlayerXpForCompletedRound()
    {
        // Esta XP alimenta el nivel global / reward track.
        // Puede ser fijo para que cada ronda equivalga a un nivel si así quieres.
        return 100;
    }

    private int GetCoinsForCompletedRound()
    {
        return baseCoinsPerRound + CurrentRound * 5;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(SaveKeyRound, CurrentRound);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        CurrentRound = PlayerPrefs.GetInt(SaveKeyRound, 1);
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Complete Round")]
    private void DebugCompleteRound()
    {
        CurrentRoundXp = RequiredRoundXp;
        CompleteRound();
    }

    [ContextMenu("Debug Reset Rounds")]
    private void DebugResetRounds()
    {
        PlayerPrefs.DeleteKey(SaveKeyRound);
        CurrentRound = 1;
        StartRound();
    }
#endif
}