using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundCompleteOverlay : MonoBehaviour
{
    public static RoundCompleteOverlay I { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button watchAdDoubleCoinsButton;

    private Action onContinue;
    private int currentCoinsReward;
    private bool doubled;

    private void Awake()
    {
        I = this;

        if (continueButton != null)
            continueButton.onClick.AddListener(Continue);

        if (watchAdDoubleCoinsButton != null)
            watchAdDoubleCoinsButton.onClick.AddListener(WatchAdForDoubleCoins);

        Hide();
    }

    public void Show(int round, int coinsReward, Action continueCallback)
    {
        currentCoinsReward = coinsReward;
        onContinue = continueCallback;
        doubled = false;

        if (root != null)
            root.SetActive(true);

        if (titleText != null)
            titleText.text = $"Nivel {round} completado";

        if (coinsText != null)
            coinsText.text = $"+{coinsReward} monedas";

        if (watchAdDoubleCoinsButton != null)
            watchAdDoubleCoinsButton.gameObject.SetActive(true);
    }

    private void WatchAdForDoubleCoins()
    {
        if (doubled)
            return;

        doubled = true;

        // Aquí luego conectas AdMob rewarded.
        // Por ahora lo simulamos:
        SoftCurrencyManager.I?.AddCoins(currentCoinsReward);

        if (coinsText != null)
            coinsText.text = $"+{currentCoinsReward * 2} monedas";

        if (watchAdDoubleCoinsButton != null)
            watchAdDoubleCoinsButton.gameObject.SetActive(false);
    }

    private void Continue()
    {
        Hide();
        onContinue?.Invoke();
    }

    private void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}