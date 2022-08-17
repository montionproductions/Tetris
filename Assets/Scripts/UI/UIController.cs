using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text ScoreText;
    public TMP_Text LevelText;
    public TMP_Text LinesText;
    public GameObject StartCounter;
    public TMP_Text TimeText;

    public int scoreMultiplier = 110;

    static private Game gameController;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameObject.FindObjectOfType<Game>();

        //ScoreText = GameObject.Find("ScoreText").GetComponent<TMP_Text>();
        //LevelText = GameObject.Find("LevelText").GetComponent<TMP_Text>();
        //LinesText = GameObject.Find("LinesText").GetComponent<TMP_Text>();

        // Find start timer
        var Timer = GameObject.Find("Timer");
        if (Timer != null)
            StartCounter = Timer;

        //TimeText = GameObject.Find("TimeText").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        ResetText();
    }

    // Update is called once per frame
    void Update()
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
        ScoreText.text = Game._score.ToString();
    }

    public void UpdateLevel(int level)
    {
        Game._level = level;
        LevelText.text = "Level:\n<size=130%>" + Game._level.ToString();
    }

    private void UpdateLines()
    {
        LinesText.text = "Lines:\n<size=130%>" + Game._lines.ToString();
    }

    public void AddLine(int line)
    {
        Game._lines += line;
        Game._score = Game._lines * scoreMultiplier;

        if (LeaderboardController.UpdateHighScore(Game._score))
        {
            gameController.OnNewHighScoreWrote();
            Game._highScore = Game._score;
        }

        UpdateLines();
        UpdateScore();
        //UpdateLevel();
    }

    private void UpdateTime()
    {
        var minutes = (int)Game.TimeTimer / 60;
        var seconds = (int)Game.TimeTimer % 60;
        var timeConverted = minutes.ToString("0") + "." + seconds.ToString();

        TimeText.text = "Time:\n<size=130%>" + timeConverted;
    }
}
