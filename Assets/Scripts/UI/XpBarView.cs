using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class XpBarView : MonoBehaviour
{
    [SerializeField] private Slider xpSlider;
    [SerializeField] private Image nextRewardIcon;
    [SerializeField] private Animator animator;

    [Header("Line Point Sparks")]
    [SerializeField, Min(1)] private int sparkCount = 7;
    [SerializeField] private Vector2 sparkSize = new Vector2(14f, 14f);
    [SerializeField] private Color sparkColor = new Color(1f, .85f, .15f, 1f);
    [SerializeField, Min(0f)] private float spawnSpread = 35f;
    [SerializeField, Min(0f)] private float spawnDelayStep = .055f;
    [SerializeField, Min(.01f)] private float flightDuration = .48f;
    [SerializeField] private float arcHeight = 90f;
    [SerializeField, Min(0f)] private float startScale = 1f;
    [SerializeField, Min(0f)] private float endScale = .25f;
    [SerializeField, Min(0f)] private float completionDelay = .72f;

    private void OnEnable()
    {
        GridGenerator.OnLineCompleted += PlayPointsFlight;
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
        GridGenerator.OnLineCompleted -= PlayPointsFlight;
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

    private void PlayPointsFlight(Vector3 worldPosition, int points)
    {
        if (xpSlider == null || Camera.main == null)
            return;

        StartCoroutine(AnimatePoints(worldPosition, points));
    }

    private IEnumerator AnimatePoints(Vector3 worldPosition, int points)
    {
        Canvas canvas = xpSlider.GetComponentInParent<Canvas>();
        if (canvas == null) yield break;

        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransform target = xpSlider.transform as RectTransform;
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 startScreen = Camera.main.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, startScreen, uiCamera, out Vector2 start);
        Vector2 targetScreen = RectTransformUtility.WorldToScreenPoint(uiCamera, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreen, uiCamera, out Vector2 end);

        for (int i = 0; i < sparkCount; i++)
        {
            GameObject spark = new GameObject("LinePointSpark", typeof(RectTransform), typeof(Image));
            spark.transform.SetParent(canvas.transform, false);
            RectTransform sparkRect = spark.transform as RectTransform;
            sparkRect.sizeDelta = sparkSize;
            sparkRect.anchoredPosition = start + Random.insideUnitCircle * spawnSpread;
            Image image = spark.GetComponent<Image>();
            image.color = sparkColor;
            image.raycastTarget = false;
            StartCoroutine(FlySpark(sparkRect, end, i * spawnDelayStep));
        }

        yield return new WaitForSecondsRealtime(completionDelay);
        if (animator != null)
            animator.SetTrigger("Complete");
        StartCoroutine(PulseBar());
    }

    private IEnumerator FlySpark(RectTransform spark, Vector2 target, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Vector2 origin = spark.anchoredPosition;
        for (float elapsed = 0f; elapsed < flightDuration; elapsed += Time.unscaledDeltaTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / flightDuration);
            Vector2 arc = Vector2.up * Mathf.Sin(t * Mathf.PI) * arcHeight;
            spark.anchoredPosition = Vector2.Lerp(origin, target, t) + arc;
            spark.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
            yield return null;
        }
        Destroy(spark.gameObject);
    }

    private IEnumerator PulseBar()
    {
        RectTransform rect = xpSlider.transform as RectTransform;
        for (float elapsed = 0f; elapsed < .35f; elapsed += Time.unscaledDeltaTime)
        {
            float scale = 1f + Mathf.Sin(elapsed / .35f * Mathf.PI) * .16f;
            rect.localScale = Vector3.one * scale;
            yield return null;
        }
        rect.localScale = Vector3.one;
    }
}
