using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Optional Legacy Texts")]
    public TMP_Text ScoreText;
    public TMP_Text LevelText;
    public TMP_Text LinesText;
    public TMP_Text TimeText;

    public GameObject StartCounter;

    [Header("Score")]
    public int scoreMultiplier = 110;

    [Header("XP")]
    [SerializeField] private int xpPerLine = 25;
    [SerializeField] private int xpTwoLinesBonus = 20;
    [SerializeField] private int xpThreeLinesBonus = 45;
    [SerializeField] private int xpFourLinesBonus = 80;

    private static Game gameController;

    private void Awake()
    {
        gameController = FindFirstObjectByType<Game>();

        GameObject timer = GameObject.Find("Timer");
        if (timer != null)
            StartCounter = timer;
    }

    private void Start()
    {
        ResetText();
    }

    private void Update()
    {
        if (Game.isPaused)
            return;

        UpdateTime();
    }

    public void ResetText()
    {
        Game._score = 0;
        UpdateScore();

        Game._level = 1;
        UpdateLevel(Game._level);

        Game._lines = 0;
        UpdateLines();
    }

    private void UpdateScore()
    {
        if (ScoreText != null)
            ScoreText.text = Game._score.ToString();
    }

    public void UpdateLevel(int level)
    {
        Game._level = level;

        if (LevelText != null)
            LevelText.text = "Level:\n<size=130%>" + Game._level.ToString();
    }

    private void UpdateLines()
    {
        if (LinesText != null)
            LinesText.text = "Lines:\n<size=130%>" + Game._lines.ToString();
    }

    public void AddLine(int line)
    {
        Game._lines += line;
        Game._score = Game._lines * scoreMultiplier;

        GiveRealtimeXp(line);

        if (LeaderboardController.UpdateHighScore(Game._score))
        {
            if (gameController != null)
                gameController.OnNewHighScoreWrote();

            Game._highScore = Game._score;
        }

        UpdateLines();
        UpdateScore();
    }

    private void GiveRealtimeXp(int line)
    {
        int xp = xpPerLine * Mathf.Max(1, line);

        // Bonus usando el contador actual de líneas consecutivas.
        // GridGenerator incrementa Game._linesCounter antes de evaluar los premios.
        if (Game._linesCounter == 2)
            xp += xpTwoLinesBonus;
        else if (Game._linesCounter == 3)
            xp += xpThreeLinesBonus;
        else if (Game._linesCounter >= 4)
            xp += xpFourLinesBonus;

        PlayerLevelManager.I?.AddXp(xp);
    }

    private void UpdateTime()
    {
        if (TimeText == null)
            return;

        int minutes = (int)Game.TimeTimer / 60;
        int seconds = (int)Game.TimeTimer % 60;
        string timeConverted = minutes.ToString("0") + "." + seconds.ToString();

        TimeText.text = "Time:\n<size=130%>" + timeConverted;
    }
}