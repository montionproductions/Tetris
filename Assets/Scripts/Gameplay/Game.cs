using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public Transform[] figures;

    public Transform[] figuresPreview;
    public Transform[] nextFiguresSpawnPoints;

    public GameObject GameElements;
    public GameObject gameOverMenu;
    public GameObject inputScore;
    public GameObject highScoreText;
    public GameObject soundSystem;

    public ParticleSystem TwoLinesParticleSystem;
    public ParticleSystem ThreeLinesParticleSystem;
    public ParticleSystem FourLinesParticleSystem;

    public float spawnTime;
    public Transform spawnPoint;

    public float HorizontalMovSpeed = 0.1f;
    public float VerticalMovSpeed = 0.05f;
    public float RotateSpeed = 0.2f;
    public float MovTime = 1.0f;

    public static bool isPaused = false;
    public static float TimeTimer = 0f;
    public static bool isGameStarted = false;
    public static bool isGameOver = false;

    public List<Level> levels;

    private int[] _nextFigures;
    private Transform[] _nextFiguresObjects;

    public static Transform currentFigure;
    public PowerUpsMenu m_powerUpsMenu;

    static public int _score = 0;
    static public int _highScore = 0;
    static public bool _highScoreAchieved = false;

    static public int _level = 1;

    static public int _lines = 0;
    static public int _linesCounter = 0; // Store lines that player got in the last 3 seconds

    static public Game gameInstance;
    static public SoundSystem soundSystemInstance;

    public class Level
    {
        public float _timeToStart;
        public float _movTime;
        public int _level;

        public Level(float timeToStart, float movTime, int level)
        {
            _timeToStart = timeToStart;
            _movTime = movTime;
            _level = level;
        }
    }

    private void Awake()
    {
        gameInstance = this;

        if(soundSystemInstance == null)
        {
            soundSystemInstance = Instantiate(soundSystem).GetComponent<SoundSystem>();
            DontDestroyOnLoad(soundSystemInstance);
        }
            
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void Update()
    {
        if (isPaused || isGameOver)
            return;

        if(isGameStarted)
        {
            UpdateTimer();
        }
    }

    public void InitGame()
    {
        GameElements.SetActive(true);

        _initLevels();

        TimeTimer = 0f;
        isPaused = false;
        isGameStarted = true;
        isGameOver = false;
        _highScoreAchieved = false;

        _nextFigures = new int[3];
        _nextFiguresObjects = new Transform[3];

        _generateAllNextFigures();
        SpawnRandomFigure();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public IEnumerator GameOver()
    {
        GridGenerator.DeleteAllBoxes();
        isGameOver = true;

        yield return new WaitForSeconds(2.5f);

        if (LeaderboardController.UpdateHighScore(Game._score))
            inputScore.SetActive(true);
        else
            gameOverMenu.SetActive(true);            
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnLevelChange(float movTime, int level)
    {
        MovTime = movTime;
        GameObject.FindObjectOfType<UIController>().UpdateLevel(level);
    }

    void _generateAllNextFigures()
    {   
        for(int i = 0; i < 3; i++)
        {
            _nextFigures[i] = Random.Range(0, figures.Length);
            _nextFiguresObjects[i] = Instantiate(figuresPreview[_nextFigures[i]], nextFiguresSpawnPoints[i]);
        }
    }

    void _getNextFigure()
    {
        _nextFigures[0] = _nextFigures[1];
        _nextFigures[1] = _nextFigures[2];
        _nextFigures[2] = Random.Range(0, figures.Length);
        if (_nextFigures[2] == _nextFigures[1])
        {
            _nextFigures[2] = Random.Range(0, figures.Length);
        }

        for (int i = 0; i < 3; i++)
        {
            Destroy(_nextFiguresObjects[i].gameObject);
            
            _nextFiguresObjects[i] = Instantiate(figuresPreview[_nextFigures[i]], nextFiguresSpawnPoints[i]);
        }
    }

    public Transform SpawnRandomFigure()
    {
        Transform obj = Instantiate(figures[_nextFigures[0]], spawnPoint.transform.position, Quaternion.identity);
        _getNextFigure();

        currentFigure = obj;

        return obj;
    }

    public Transform SpawnFigure(Grup.FigureType type)
    {
        Transform obj = Instantiate(figures[(int)type], spawnPoint.transform.position, Quaternion.identity);

        currentFigure = obj;

        return obj;
    }

    private void _initLevels()
    {
        levels = new List<Level>();

        levels.Add(new Level(0f, 1f, 1)); // Level 1
        StartCoroutine("StartLevel", levels[0]);

        levels.Add(new Level(60f, .5f, 2)); // Level 2
        StartCoroutine("StartLevel", levels[1]);

        levels.Add(new Level(180f, .4f, 3)); // Level 3
        StartCoroutine("StartLevel", levels[2]);

        levels.Add(new Level(360f, .35f, 4)); // Level 4
        StartCoroutine("StartLevel", levels[3]);

        levels.Add(new Level(540f, .25f, 5)); // Level 5
        StartCoroutine("StartLevel", levels[4]);

        levels.Add(new Level(720f, .15f, 6)); // Level 5
        StartCoroutine("StartLevel", levels[5]);
    }

    IEnumerator StartLevel(Level level)
    {
        yield return new WaitForSeconds(level._timeToStart);
        GameObject.FindObjectOfType<Game>().OnLevelChange(level._movTime, level._level);
    }

    void UpdateTimer()
    {
        TimeTimer += Time.deltaTime;
        // Implements UI Controller
    }

    static public void On4LinesWin(int line)
    {
        Debug.Log("4 LINES WINED!!");
        Game._linesCounter = 0;

        var currentPos = gameInstance.FourLinesParticleSystem.transform.position;
        gameInstance.FourLinesParticleSystem.transform.position = new Vector3(currentPos.x, line, currentPos.z);
        gameInstance.FourLinesParticleSystem.Play();

        gameInstance.m_powerUpsMenu.InstantiatePowerUp(DragAndDropElement.PowerUpType.DeleteRow);
    }

    static public void On2LinesWin(int line)
    {
        Debug.Log("2 LINES WINED!!");
        Game._linesCounter = 0;

        var currentPos = gameInstance.TwoLinesParticleSystem.transform.position;
        gameInstance.TwoLinesParticleSystem.transform.position = new Vector3(currentPos.x, line, currentPos.z);
        gameInstance.TwoLinesParticleSystem.Play();

        gameInstance.m_powerUpsMenu.InstantiatePowerUp(DragAndDropElement.PowerUpType.CompleteRow);
    }

    static public void On3LinesWin(int line)
    {
        Debug.Log("3 LINES WINED!!");
        Game._linesCounter = 0;

        var currentPos = gameInstance.ThreeLinesParticleSystem.transform.position;
        gameInstance.ThreeLinesParticleSystem.transform.position = new Vector3(currentPos.x, line, currentPos.z);
        gameInstance.ThreeLinesParticleSystem.Play();

        gameInstance.m_powerUpsMenu.InstantiatePowerUp(DragAndDropElement.PowerUpType.DeleteColum);
    }

    public void OnNewHighScoreWrote()
    {
        if (_highScoreAchieved)
            return;

        Debug.Log("NEW HIGH SCORE!!");
        _highScoreAchieved = true;

        highScoreText.SetActive(true);

        // Backgound Animation
        Animator backgroundAnimator = GameObject.Find("LevelBack").GetComponentInChildren<Animator>();
        backgroundAnimator.SetTrigger("HighScore");
    }
}
