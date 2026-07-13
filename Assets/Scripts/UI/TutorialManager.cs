using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>Onboarding contextual de la primera sesión, sin configuración manual en la escena.</summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager I { get; private set; }
    private enum Visual { None, Swipe, Rotate, Drop, Goal, Reward, Companion, Upgrade, FillCell, ClearRow, ClearColumn }
    private const string CompletedKey = "cbh_tutorial_completed_v3";
    private const string MovementKey = "cbh_tutorial_movement_v3";

    private GameObject panel;
    private Text title;
    private Text body;
    private Button button;
    private Text buttonText;
    private int movementStep;
    private bool movementActive;
    private bool rewardExplained;
    private RectTransform demoArea;
    private RectTransform piece;
    private RectTransform hand;
    private RectTransform arrow;
    private RectTransform progressFill;
    private RectTransform gift;
    private RectTransform powerOrb;
    private Visual visual;
    private Coroutine onboardingRoutine;
    private Coroutine powerUpRoutine;
    private PowerUpDefinition pendingPowerUp;
    private PowerUpDefinition activePowerUpTutorial;
    private bool lineLessonActive;

    public static void EnsureExists()
    {
        if (FindFirstObjectByType<TutorialManager>() != null) return;
        GameObject instance = new GameObject("TutorialManager");
        DontDestroyOnLoad(instance);
        instance.AddComponent<TutorialManager>();
    }

    private void Awake()
    {
        I = this;
        BuildUi();
        Grup.OnPlayerMove += HandleMove;
        RewardRevealOverlay.OnRewardShown += HandleRewardShown;
        RewardRevealOverlay.OnRewardRedeemed += HandleRewardRedeemed;
        PowerUpUnlockManager.OnPowerUpUnlocked += HandlePowerUpUnlocked;
        DragAndDropElement.OnPowerUpUsed += HandlePowerUpUsed;
        GridGenerator.OnLineCompleted += HandleTutorialLineCompleted;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        RestartOnboarding();
    }

    private void OnDestroy()
    {
        Grup.OnPlayerMove -= HandleMove;
        RewardRevealOverlay.OnRewardShown -= HandleRewardShown;
        RewardRevealOverlay.OnRewardRedeemed -= HandleRewardRedeemed;
        PowerUpUnlockManager.OnPowerUpUnlocked -= HandlePowerUpUnlocked;
        DragAndDropElement.OnPowerUpUsed -= HandlePowerUpUsed;
        GridGenerator.OnLineCompleted -= HandleTutorialLineCompleted;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (I == this) I = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RestartOnboarding();
    }

    private void RestartOnboarding()
    {
        if (onboardingRoutine != null)
            StopCoroutine(onboardingRoutine);
        if (powerUpRoutine != null)
            StopCoroutine(powerUpRoutine);

        movementActive = false;
        movementStep = 0;
        rewardExplained = false;
        lineLessonActive = false;
        pendingPowerUp = null;
        activePowerUpTutorial = null;
        Hide();
        onboardingRoutine = StartCoroutine(BeginWhenPlaying());
        FindPendingUnlockedPowerUp();
    }

    public void DebugResetTutorial()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        PlayerPrefs.DeleteKey(CompletedKey);
        PlayerPrefs.DeleteKey(MovementKey);
        if (PowerUpUnlockManager.I != null)
        {
            foreach (PowerUpDefinition powerUp in PowerUpUnlockManager.I.PowerUps)
                if (powerUp != null) PlayerPrefs.DeleteKey(GetPowerUpTutorialKey(powerUp.id));
        }
        PlayerPrefs.Save();
        RestartOnboarding();
#endif
    }

    private void HandlePowerUpUnlocked(PowerUpDefinition powerUp)
    {
        if (powerUp == null || PlayerPrefs.GetInt(GetPowerUpTutorialKey(powerUp.id), 0) == 1)
            return;

        pendingPowerUp = powerUp;
        StartPendingPowerUpTutorial();
    }

    private void FindPendingUnlockedPowerUp()
    {
        if (PowerUpUnlockManager.I == null) return;
        foreach (PowerUpDefinition powerUp in PowerUpUnlockManager.I.PowerUps)
        {
            if (powerUp == null || !PowerUpUnlockManager.I.IsUnlocked(powerUp.id)) continue;
            if (PlayerPrefs.GetInt(GetPowerUpTutorialKey(powerUp.id), 0) == 1) continue;
            pendingPowerUp = powerUp;
            StartPendingPowerUpTutorial();
            return;
        }
    }

    private void StartPendingPowerUpTutorial()
    {
        if (powerUpRoutine != null) StopCoroutine(powerUpRoutine);
        powerUpRoutine = StartCoroutine(ShowPowerUpWhenReady());
    }

    private IEnumerator ShowPowerUpWhenReady()
    {
        while (pendingPowerUp != null &&
               (PlayerPrefs.GetInt(CompletedKey, 0) == 0 ||
                !Game.isGameStarted ||
                RoundFlowManager.I == null ||
                RoundFlowManager.I.State != RoundState.Playing ||
                panel.activeSelf))
            yield return null;

        if (pendingPowerUp == null) yield break;

        activePowerUpTutorial = pendingPowerUp;
        pendingPowerUp = null;
        Game.gameInstance?.SetupPowerUpTutorialBoard(activePowerUpTutorial.type);
        Visual tutorialVisual = GetPowerUpVisual(activePowerUpTutorial.type);
        Show("ARRASTRA EL POWER-UP", GetPowerUpSymbol(activePowerUpTutorial.type), null, null, tutorialVisual);
    }

    private void HandlePowerUpUsed(DragAndDropElement.PowerUpType usedType)
    {
        if (activePowerUpTutorial == null || !Matches(activePowerUpTutorial.type, usedType)) return;

        PlayerPrefs.SetInt(GetPowerUpTutorialKey(activePowerUpTutorial.id), 1);
        PlayerPrefs.Save();
        activePowerUpTutorial = null;
        Hide();
        FindPendingUnlockedPowerUp();
    }

    private static bool Matches(PowerUpType definition, DragAndDropElement.PowerUpType used)
    {
        return (definition == PowerUpType.FillSingleCell && used == DragAndDropElement.PowerUpType.CompleteRow) ||
               (definition == PowerUpType.ClearColumn && used == DragAndDropElement.PowerUpType.DeleteColum) ||
               (definition == PowerUpType.ClearRow && used == DragAndDropElement.PowerUpType.DeleteRow);
    }

    private static Visual GetPowerUpVisual(PowerUpType type)
    {
        if (type == PowerUpType.ClearRow) return Visual.ClearRow;
        if (type == PowerUpType.ClearColumn) return Visual.ClearColumn;
        return Visual.FillCell;
    }

    private static string GetPowerUpSymbol(PowerUpType type)
    {
        if (type == PowerUpType.ClearRow) return "●  →  ━━━━━";
        if (type == PowerUpType.ClearColumn) return "●  →  ┃";
        return "●  →  □";
    }

    private static string GetPowerUpTutorialKey(string id) => "cbh_power_tutorial_" + id;

    private void Update()
    {
        AnimateVisual();
        if (PlayerPrefs.GetInt(CompletedKey, 0) == 1 || panel.activeSelf) return;
        if (RoundFlowManager.I != null && RoundFlowManager.I.State == RoundState.Upgrade)
            ShowUpgradeStep();
    }

    private IEnumerator BeginWhenPlaying()
    {
        if (PlayerPrefs.GetInt(CompletedKey, 0) == 1) yield break;
        while (!Game.isGameStarted || Game.currentFigure == null) yield return null;

        if (PlayerPrefs.GetInt(MovementKey, 0) == 0)
        {
            movementActive = true;
            Show("MUEVE", "DESLIZA  ↔", null, null, Visual.Swipe);
        }
        else
        {
            Game.gameInstance?.SetupGuidedFirstLine();
            ShowObjective();
        }
    }

    private void HandleMove(Grup.PlayerMove move)
    {
        if (!movementActive) return;
        if (movementStep == 0 && move == Grup.PlayerMove.Horizontal)
        {
            movementStep = 1;
            Show("GIRA", "DESLIZA  ↑", null, null, Visual.Rotate);
        }
        else if (movementStep == 1 && move == Grup.PlayerMove.Rotate)
        {
            movementStep = 2;
            Show("SUELTA", "DESLIZA RÁPIDO  ↓", null, null, Visual.Drop);
        }
        else if (movementStep == 2 && move == Grup.PlayerMove.HardDrop)
        {
            movementActive = false;
            PlayerPrefs.SetInt(MovementKey, 1);
            PlayerPrefs.Save();
            Hide();
            StartCoroutine(PrepareLineLesson());
        }
    }

    private IEnumerator PrepareLineLesson()
    {
        // Espera a que la caída termine y el juego cree la siguiente pieza.
        yield return null;
        Game.gameInstance?.SetupGuidedFirstLine();
        ShowObjective();
    }

    private void ShowObjective()
    {
        int goal = RoundFlowManager.I != null ? RoundFlowManager.I.RequiredRoundXp : 100;
        lineLessonActive = true;
        Show("COMPLETA UNA LÍNEA", $"LÍNEA = PUNTOS   META {goal}", null, null, Visual.Goal);
    }

    private void HandleTutorialLineCompleted(Vector3 position, int points)
    {
        if (!lineLessonActive) return;
        lineLessonActive = false;
        Hide();
    }

    private void HandleRewardShown(LevelRewardDefinition reward)
    {
        if (PlayerPrefs.GetInt(CompletedKey, 0) == 1 || rewardExplained) return;
        rewardExplained = true;
        Show("¡PREMIO!", "TOCA  REDIMIR", "→", Hide, Visual.Reward);
    }

    private void HandleRewardRedeemed(LevelRewardDefinition reward)
    {
        if (PlayerPrefs.GetInt(CompletedKey, 0) == 1 || reward == null) return;
        string extra = "";
        if (reward.rewardType == LevelRewardType.Companion && CompanionCollectionManager.I != null)
        {
            CompanionDefinition companion = CompanionCollectionManager.I.FindCompanionById(reward.rewardId);
            if (companion != null)
                extra = string.IsNullOrWhiteSpace(companion.linkedPowerUpId)
                    ? $" {companion.displayName} puede subir su afinidad y desbloquear mejoras."
                    : $" {companion.displayName} está vinculado al power-up {companion.linkedPowerUpId}.";
        }
        Show("COMPANION + POWER-UP", string.IsNullOrEmpty(extra) ? "CADA UNO TIENE UN PODER" : extra, "→", Hide, Visual.Companion);
    }

    private void ShowUpgradeStep()
    {
        Show("MEJORA TU COMPANION", "MONEDAS  →  MÁS PODER", "✓", CompleteTutorial, Visual.Upgrade);
    }

    private void CompleteTutorial()
    {
        PlayerPrefs.SetInt(CompletedKey, 1);
        PlayerPrefs.Save();
        Hide();
    }

    private void Show(string heading, string message, string action, UnityEngine.Events.UnityAction callback = null, Visual animation = Visual.None)
    {
        visual = animation;
        title.text = heading;
        body.text = message;
        button.gameObject.SetActive(!string.IsNullOrEmpty(action));
        button.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(action))
        {
            buttonText.text = action;
            button.onClick.AddListener(callback ?? Hide);
        }
        panel.SetActive(true);
        ResetVisuals();
        panel.transform.SetAsLastSibling();
    }

    private void Hide() => panel.SetActive(false);

    private void BuildUi()
    {
        GameObject canvasObject = new GameObject("TutorialCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        panel = new GameObject("TutorialCard", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);
        RectTransform rect = (RectTransform)panel.transform;
        rect.anchorMin = new Vector2(.06f, .57f);
        rect.anchorMax = new Vector2(.94f, .94f);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(.035f, .055f, .11f, .96f);

        title = CreateText("Title", panel.transform, 46, FontStyle.Bold, new Vector2(.06f, .80f), new Vector2(.94f, .96f));
        title.alignment = TextAnchor.MiddleCenter;
        body = CreateText("Body", panel.transform, 30, FontStyle.Bold, new Vector2(.06f, .20f), new Vector2(.60f, .34f));
        body.alignment = TextAnchor.MiddleCenter;

        demoArea = CreateRect("Animation", panel.transform, new Vector2(.08f, .35f), new Vector2(.92f, .79f), Color.clear);
        piece = CreateRect("Piece", demoArea, new Vector2(.40f, .34f), new Vector2(.60f, .70f), new Color(.2f, .75f, 1f, 1f));
        hand = CreateRect("Finger", demoArea, new Vector2(.46f, .05f), new Vector2(.54f, .23f), Color.white);
        arrow = CreateRect("Direction", demoArea, new Vector2(.25f, .05f), new Vector2(.75f, .25f), Color.clear);
        Text arrowText = CreateText("ArrowText", arrow, 54, FontStyle.Bold, Vector2.zero, Vector2.one);
        arrowText.alignment = TextAnchor.MiddleCenter;
        arrowText.text = "↔";

        RectTransform progressBack = CreateRect("ProgressBack", demoArea, new Vector2(.12f, .38f), new Vector2(.88f, .62f), new Color(.12f, .16f, .25f, 1f));
        progressFill = CreateRect("ProgressFill", progressBack, Vector2.zero, Vector2.one, new Color(.3f, .95f, .48f, 1f));
        progressFill.pivot = new Vector2(0f, .5f);
        gift = CreateRect("Gift", demoArea, new Vector2(.39f, .20f), new Vector2(.61f, .72f), new Color(1f, .35f, .55f, 1f));
        CreateRect("Ribbon", gift, new Vector2(.42f, 0f), new Vector2(.58f, 1f), new Color(1f, .85f, .2f, 1f));
        powerOrb = CreateRect("PowerOrb", demoArea, new Vector2(.68f, .32f), new Vector2(.84f, .64f), new Color(1f, .82f, .18f, 1f));

        GameObject buttonObject = new GameObject("Action", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(panel.transform, false);
        RectTransform buttonRect = (RectTransform)buttonObject.transform;
        buttonRect.anchorMin = new Vector2(.64f, .06f);
        buttonRect.anchorMax = new Vector2(.94f, .22f);
        buttonRect.offsetMin = buttonRect.offsetMax = Vector2.zero;
        buttonObject.GetComponent<Image>().color = new Color(.2f, .72f, .95f, 1f);
        button = buttonObject.GetComponent<Button>();
        buttonText = CreateText("Label", buttonObject.transform, 28, FontStyle.Bold, Vector2.zero, Vector2.one);
        buttonText.alignment = TextAnchor.MiddleCenter;
        panel.SetActive(false);
    }

    private void AnimateVisual()
    {
        if (panel == null || !panel.activeSelf) return;
        float cycle = Mathf.Repeat(Time.unscaledTime, 1.4f) / 1.4f;
        float smooth = Mathf.SmoothStep(0f, 1f, cycle);
        float pulse = 1f + Mathf.Sin(Time.unscaledTime * 5f) * .10f;

        if (visual == Visual.Swipe)
        {
            hand.anchoredPosition = new Vector2(Mathf.Lerp(-170f, 170f, smooth), -5f);
            piece.anchoredPosition = new Vector2(Mathf.Lerp(-90f, 90f, smooth), 15f);
        }
        else if (visual == Visual.Rotate)
        {
            hand.anchoredPosition = new Vector2(110f * Mathf.Sin(cycle * Mathf.PI * 2f), -10f + 65f * smooth);
            piece.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, -90f, smooth));
        }
        else if (visual == Visual.Drop)
        {
            hand.anchoredPosition = new Vector2(0f, Mathf.Lerp(70f, -75f, smooth));
            piece.anchoredPosition = new Vector2(0f, Mathf.Lerp(75f, -45f, smooth));
        }
        else if (visual == Visual.Goal)
        {
            progressFill.localScale = new Vector3(smooth, 1f, 1f);
        }
        else if (visual == Visual.Reward)
        {
            gift.localScale = Vector3.one * pulse;
            gift.localEulerAngles = new Vector3(0f, 0f, Mathf.Sin(Time.unscaledTime * 7f) * 7f);
        }
        else if (visual == Visual.Companion)
        {
            piece.localScale = Vector3.one * pulse;
            powerOrb.anchoredPosition = new Vector2(Mathf.Cos(Time.unscaledTime * 3f) * 145f, Mathf.Sin(Time.unscaledTime * 3f) * 45f);
        }
        else if (visual == Visual.Upgrade)
        {
            piece.localScale = Vector3.one * Mathf.Lerp(.8f, 1.25f, smooth);
            powerOrb.localScale = Vector3.one * (1.2f - smooth);
            powerOrb.anchoredPosition = Vector2.Lerp(new Vector2(150f, 0f), Vector2.zero, smooth);
        }
        else if (visual == Visual.FillCell || visual == Visual.ClearRow || visual == Visual.ClearColumn)
        {
            Vector2 start = new Vector2(-230f, -15f);
            Vector2 destination = Vector2.zero;
            powerOrb.anchoredPosition = Vector2.Lerp(start, destination, smooth);
            hand.anchoredPosition = powerOrb.anchoredPosition + new Vector2(0f, -65f);
            powerOrb.localScale = Vector3.one * (1f + Mathf.Sin(Time.unscaledTime * 8f) * .12f);
            piece.localScale = Vector3.one * (1f + Mathf.Max(0f, Mathf.Sin((cycle - .65f) * Mathf.PI * 3f)) * .18f);
        }
    }

    private void ResetVisuals()
    {
        bool powerUpVisual = visual == Visual.FillCell || visual == Visual.ClearRow || visual == Visual.ClearColumn;
        piece.gameObject.SetActive(visual == Visual.Swipe || visual == Visual.Rotate || visual == Visual.Drop || visual == Visual.Companion || visual == Visual.Upgrade || powerUpVisual);
        hand.gameObject.SetActive(visual == Visual.Swipe || visual == Visual.Rotate || visual == Visual.Drop || powerUpVisual);
        arrow.gameObject.SetActive(visual == Visual.Swipe || visual == Visual.Rotate || visual == Visual.Drop);
        progressFill.parent.gameObject.SetActive(visual == Visual.Goal);
        gift.gameObject.SetActive(visual == Visual.Reward);
        powerOrb.gameObject.SetActive(visual == Visual.Companion || visual == Visual.Upgrade || powerUpVisual);
        piece.anchoredPosition = Vector2.zero;
        piece.localScale = Vector3.one;
        piece.localRotation = Quaternion.identity;
        hand.anchoredPosition = Vector2.zero;
        powerOrb.localScale = Vector3.one;
        if (visual == Visual.ClearRow)
        {
            piece.anchorMin = new Vector2(.12f, .43f);
            piece.anchorMax = new Vector2(.88f, .57f);
        }
        else if (visual == Visual.ClearColumn)
        {
            piece.anchorMin = new Vector2(.45f, .08f);
            piece.anchorMax = new Vector2(.55f, .92f);
        }
        else
        {
            piece.anchorMin = new Vector2(.40f, .34f);
            piece.anchorMax = new Vector2(.60f, .70f);
        }
        piece.offsetMin = piece.offsetMax = Vector2.zero;
        Text direction = arrow.GetComponentInChildren<Text>();
        if (direction != null)
            direction.text = visual == Visual.Swipe ? "↔" : visual == Visual.Rotate ? "↻" : "↓";
    }

    private static RectTransform CreateRect(string name, Transform parent, Vector2 min, Vector2 max, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)obj.transform;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        obj.GetComponent<Image>().color = color;
        obj.GetComponent<Image>().raycastTarget = false;
        return rect;
    }

    private static Text CreateText(string name, Transform parent, int size, FontStyle style, Vector2 min, Vector2 max)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)obj.transform;
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        Text text = obj.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }
}
