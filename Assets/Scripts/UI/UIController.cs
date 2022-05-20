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

    private int _score = 0;
    private int _level = 1;
    private int _lines = 0;

    private Game gameController;

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
        if (!Game.isGameStarted)
            return;

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
        _score = 0;
        UpdateScore();

        _level = 1;
        UpdateLevel(_level);

        _lines = 0;
        UpdateLines();

        StartCounter.SetActive(true);
    }

    private void UpdateScore()
    {
        ScoreText.text = _score.ToString();
    }

    public void UpdateLevel(int level)
    {
        _level = level;
        LevelText.text = "Level:\n<size=130%>" + _level.ToString();
    }

    private void UpdateLines()
    {
        LinesText.text = "Lines:\n<size=130%>" + _lines.ToString();
    }

    public void AddLine(int line)
    {
        _lines += line;
        _score = _lines * scoreMultiplier;
        //_level = GetLevel();

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
