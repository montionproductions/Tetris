using UnityEngine;
using UnityEngine.UI;

public class XpBarView : MonoBehaviour
{
    [SerializeField] private Slider xpSlider;
    [SerializeField] private Image nextRewardIcon;
    [SerializeField] private Animator animator;

    private int lastLevel = 1;

    private void OnEnable()
    {
        if (PlayerLevelManager.I != null)
        {
            PlayerLevelManager.I.OnXpChanged += Refresh;
            PlayerLevelManager.I.OnLevelUp += HandleLevelUp;

            lastLevel = PlayerLevelManager.I.Level;

            Refresh(
                PlayerLevelManager.I.GetCurrentXpInLevel(),
                PlayerLevelManager.I.GetXpRequiredForNextLevel(),
                PlayerLevelManager.I.Level
            );
        }
    }

    private void OnDisable()
    {
        if (PlayerLevelManager.I != null)
        {
            PlayerLevelManager.I.OnXpChanged -= Refresh;
            PlayerLevelManager.I.OnLevelUp -= HandleLevelUp;
        }
    }

    private void Refresh(int currentXp, int requiredXp, int level)
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = Mathf.Max(1, requiredXp);
            xpSlider.value = Mathf.Clamp(currentXp, 0, requiredXp);
        }

        RefreshNextRewardIcon(level);
    }

    private void RefreshNextRewardIcon(int level)
    {
        if (nextRewardIcon == null)
            return;

        if (LevelRewardManager.I == null)
            return;

        LevelRewardDefinition nextReward = LevelRewardManager.I.GetNextReward(level);

        if (nextReward == null || nextReward.localPreview == null)
        {
            nextRewardIcon.enabled = false;
            return;
        }

        nextRewardIcon.enabled = true;
        nextRewardIcon.sprite = nextReward.localPreview;
    }

    private void HandleLevelUp(int newLevel)
    {
        lastLevel = newLevel;

        if (animator != null)
            animator.SetTrigger("LevelUp");
    }
}