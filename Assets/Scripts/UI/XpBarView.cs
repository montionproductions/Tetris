using UnityEngine;
using UnityEngine.UI;

public class XpBarView : MonoBehaviour
{
    [SerializeField] private Slider xpSlider;
    [SerializeField] private Image nextRewardIcon;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        if (RoundFlowManager.I != null)
        {
            RoundFlowManager.I.OnRoundXpChanged += Refresh;
            RoundFlowManager.I.OnRoundCompleted += HandleRoundCompleted;

            Refresh(
                RoundFlowManager.I.CurrentRoundXp,
                RoundFlowManager.I.RequiredRoundXp
            );
        }

        RefreshNextRewardIcon();
    }

    private void OnDisable()
    {
        if (RoundFlowManager.I != null)
        {
            RoundFlowManager.I.OnRoundXpChanged -= Refresh;
            RoundFlowManager.I.OnRoundCompleted -= HandleRoundCompleted;
        }
    }

    private void Refresh(int currentXp, int requiredXp)
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = Mathf.Max(1, requiredXp);
            xpSlider.value = Mathf.Clamp(currentXp, 0, requiredXp);
        }

        RefreshNextRewardIcon();
    }

    private void RefreshNextRewardIcon()
    {
        if (nextRewardIcon == null)
            return;

        if (LevelRewardManager.I == null || PlayerLevelManager.I == null)
            return;

        LevelRewardDefinition nextReward =
            LevelRewardManager.I.GetNextReward(PlayerLevelManager.I.Level);

        if (nextReward == null || nextReward.localPreview == null)
        {
            nextRewardIcon.enabled = false;
            return;
        }

        nextRewardIcon.enabled = true;
        nextRewardIcon.sprite = nextReward.localPreview;
    }

    private void HandleRoundCompleted(int round)
    {
        if (animator != null)
            animator.SetTrigger("Complete");
    }
}