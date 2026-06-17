using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardRevealOverlay : MonoBehaviour
{
    public static RewardRevealOverlay I { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        I = this;

        if (continueButton != null)
            continueButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(LevelRewardDefinition reward)
    {
        if (reward == null) return;

        root.SetActive(true);

        if (titleText != null)
            titleText.text = reward.title;

        if (descriptionText != null)
            descriptionText.text = reward.description;

        if (previewImage != null)
        {
            previewImage.sprite = reward.localPreview;
            previewImage.enabled = reward.localPreview != null;
        }

        // Después aquí metemos animación premium.
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}