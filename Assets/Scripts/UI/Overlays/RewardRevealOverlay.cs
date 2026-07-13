using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardRevealOverlay : MonoBehaviour
{
    public static event Action<LevelRewardDefinition> OnRewardShown;
    public static event Action<LevelRewardDefinition> OnRewardRedeemed;
    public static RewardRevealOverlay I { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button continueButton;

    private Action onContinue;
    private LevelRewardDefinition currentReward;

    private void Awake()
    {
        I = this;

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(Continue);
        }

        Hide();
    }

    public void Show(LevelRewardDefinition reward, Action continueCallback = null)
    {
        if (reward == null)
        {
            Debug.LogWarning("[RewardRevealOverlay] Tried to show null reward.");
            continueCallback?.Invoke();
            return;
        }

        Debug.Log($"[RewardRevealOverlay] Show reward: {reward.title} / {reward.rewardId} / {reward.rewardType}");

        onContinue = continueCallback;
        currentReward = reward;

        // Limpia contenido viejo primero
        if (titleText != null)
            titleText.text = "";

        if (descriptionText != null)
            descriptionText.text = "";

        if (previewImage != null)
        {
            previewImage.sprite = null;
            previewImage.enabled = false;
        }

        // Pinta contenido nuevo
        if (titleText != null)
            titleText.text = reward.title;

        if (descriptionText != null)
            descriptionText.text = reward.description;

        if (previewImage != null)
        {
            previewImage.sprite = reward.localPreview;
            previewImage.enabled = reward.localPreview != null;
        }

        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);

        TMP_Text buttonLabel = continueButton != null ? continueButton.GetComponentInChildren<TMP_Text>() : null;
        if (buttonLabel != null)
            buttonLabel.text = "REDIMIR";

        OnRewardShown?.Invoke(reward);
    }

    private void Continue()
    {
        LevelRewardDefinition redeemedReward = currentReward;
        Hide();

        Action callback = onContinue;
        onContinue = null;
        currentReward = null;

        OnRewardRedeemed?.Invoke(redeemedReward);

        callback?.Invoke();
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
