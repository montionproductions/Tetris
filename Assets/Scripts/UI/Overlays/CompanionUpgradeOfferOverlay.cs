using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionUpgradeOfferOverlay : MonoBehaviour
{
    public static CompanionUpgradeOfferOverlay I { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private Image companionImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text affinityText;
    [SerializeField] private Button upgradeWithCoinsButton;
    [SerializeField] private Button watchAdGiftButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private int coinUpgradeCost = 50;
    [SerializeField] private int affinityXpPerUpgrade = 50;
    [SerializeField] private int affinityXpPerAdGift = 75;

    private Action onContinue;
    private CompanionDefinition selectedCompanion;

    private void Awake()
    {
        I = this;

        if (upgradeWithCoinsButton != null)
            upgradeWithCoinsButton.onClick.AddListener(UpgradeWithCoins);

        if (watchAdGiftButton != null)
            watchAdGiftButton.onClick.AddListener(UpgradeWithAd);

        if (continueButton != null)
            continueButton.onClick.AddListener(Continue);

        Hide();
    }

    public void Show(Action continueCallback)
    {
        onContinue = continueCallback;
        selectedCompanion = FindFirstUnlockedCompanion();

        if (selectedCompanion == null)
        {
            Continue();
            return;
        }

        if (root != null)
            root.SetActive(true);

        Refresh();
    }

    private CompanionDefinition FindFirstUnlockedCompanion()
    {
        if (CompanionCollectionManager.I == null)
            return null;

        foreach (CompanionDefinition companion in CompanionCollectionManager.I.Companions)
        {
            if (companion == null) continue;

            if (CompanionCollectionManager.I.IsUnlocked(companion.id))
                return companion;
        }

        return null;
    }

    private void Refresh()
    {
        if (selectedCompanion == null)
            return;

        CompanionSaveData data =
            CompanionCollectionManager.I.GetData(selectedCompanion.id);

        if (titleText != null)
            titleText.text = selectedCompanion.displayName;

        if (affinityText != null)
            affinityText.text = $"Afinidad {data.affinityLevel}";

        if (companionImage != null)
        {
            companionImage.sprite = selectedCompanion.placeholderPortrait;
            companionImage.enabled = selectedCompanion.placeholderPortrait != null;
        }

        if (upgradeWithCoinsButton != null)
        {
            bool canPay = SoftCurrencyManager.I != null &&
                          SoftCurrencyManager.I.Coins >= coinUpgradeCost;

            upgradeWithCoinsButton.interactable = canPay;
        }
    }

    private void UpgradeWithCoins()
    {
        if (selectedCompanion == null)
            return;

        if (SoftCurrencyManager.I == null)
            return;

        bool spent = SoftCurrencyManager.I.SpendCoins(coinUpgradeCost);

        if (!spent)
            return;

        CompanionCollectionManager.I.AddAffinityXp(
            selectedCompanion.id,
            affinityXpPerUpgrade
        );

        Refresh();
    }

    private void UpgradeWithAd()
    {
        if (selectedCompanion == null)
            return;

        // Aquí luego conectas AdMob rewarded.
        // Por ahora simulado:
        CompanionCollectionManager.I.AddAffinityXp(
            selectedCompanion.id,
            affinityXpPerAdGift
        );

        if (watchAdGiftButton != null)
            watchAdGiftButton.gameObject.SetActive(false);

        Refresh();
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