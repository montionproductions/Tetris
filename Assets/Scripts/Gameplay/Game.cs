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
    public GameObject musicSystem;

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
    public SoundSystem soundSystemInstance;
    static public MusicSystem musicSystemInstance;
    static public PowerUpsMenu powerUpsMenu;

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

        TutorialManager.EnsureExists();

        if (musicSystemInstance == null)
        {
            musicSystemInstance = Instantiate(musicSystem).GetComponent<MusicSystem>();
            DontDestroyOnLoad(musicSystemInstance);
        }

        if (powerUpsMenu == null)
            powerUpsMenu = FindFirstObjectByType<PowerUpsMenu>();
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
        ResetStaticState();

        GameElements.SetActive(true);

        _initLevels();

        isGameStarted = true;
        isGameOver = false;

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
        RoundFlowManager.I?.ResetForGameRestart();
        ResetStaticState();
        GridGenerator.ResetGrid();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator GameOver()
    {
        if (isGameOver)
            yield break;

        isGameOver = true;
        isGameStarted = false;

        if (GridGenerator.grid != null)
            GridGenerator.DeleteAllBoxes();

        yield return new WaitForSeconds(2.5f);

        if (inputScore != null)
            inputScore.SetActive(false);

        if (gameOverMenu != null)
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

    public void SetupGuidedFirstLine()
    {
        if (GridGenerator.grid == null || figures == null || figures.Length == 0)
            return;

        if (currentFigure != null)
            currentFigure.GetComponent<Grup>()?.RemoveFigureFromGrid();

        GridGenerator.DeleteAllBoxes();

        Transform blockTemplate = figures[0].childCount > 0 ? figures[0].GetChild(0) : null;
        if (blockTemplate == null)
            return;

        GameObject guidedBlocks = new GameObject("GuidedFirstLine");
        // Deja cuatro huecos centrales. La pieza I inicial encaja al girarla una vez.
        for (int x = 0; x < GridGenerator.colums; x++)
        {
            if (x >= 3 && x <= 6)
                continue;

            Transform block = Instantiate(blockTemplate, new Vector3(x, 0f, 0f), Quaternion.identity, guidedBlocks.transform);
            block.name = $"GuidedBlock_{x}";
            GridGenerator.grid[x, 0] = block;
        }

        SpawnFigure(Grup.FigureType.I);
    }

    public void SetupPowerUpTutorialBoard(PowerUpType powerUpType)
    {
        if (GridGenerator.grid == null || figures == null || figures.Length == 0)
            return;

        if (currentFigure != null)
            currentFigure.GetComponent<Grup>()?.RemoveFigureFromGrid();

        GridGenerator.DeleteAllBoxes();

        Transform blockTemplate = figures[0].childCount > 0 ? figures[0].GetChild(0) : null;
        if (blockTemplate == null) return;

        GameObject tutorialBlocks = new GameObject("PowerUpTutorialBoard");

        if (powerUpType == PowerUpType.ClearColumn)
        {
            // Columna visible para demostrar el borrado vertical.
            for (int y = 0; y < 7; y++)
                CreateTutorialBlock(blockTemplate, tutorialBlocks.transform, 5, y);
        }
        else
        {
            // Fila casi completa: el power-up de celda se suelta en x=5.
            for (int x = 0; x < GridGenerator.colums; x++)
            {
                if (x == 5)
                    continue;
                CreateTutorialBlock(blockTemplate, tutorialBlocks.transform, x, 0);
            }
        }

        SpawnRandomFigure();
    }

    private static void CreateTutorialBlock(Transform template, Transform parent, int x, int y)
    {
        Transform block = Instantiate(template, new Vector3(x, y, 0f), Quaternion.identity, parent);
        block.name = $"PowerUpGuide_{x}_{y}";
        GridGenerator.grid[x, y] = block;
    }

    private void _initLevels()
    {
        levels = new List<Level>();

        levels.Add(new Level(0f, 1f, 1)); // Level 1
        StartCoroutine("StartLevel", levels[0]);

        levels.Add(new Level(180f, .85f, 2)); // Level 2
        StartCoroutine("StartLevel", levels[1]);

        levels.Add(new Level(360f, .65f, 3)); // Level 3
        StartCoroutine("StartLevel", levels[2]);

        levels.Add(new Level(540f, .45f, 4)); // Level 4
        StartCoroutine("StartLevel", levels[3]);

        levels.Add(new Level(720f, .35f, 5)); // Level 5
        StartCoroutine("StartLevel", levels[4]);

        levels.Add(new Level(980f, .15f, 6)); // Level 6
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

        PowerUpsMenu menu = powerUpsMenu != null ? powerUpsMenu : FindFirstObjectByType<PowerUpsMenu>();

        if (menu != null)
            menu.AddPowerUp(DragAndDropElement.PowerUpType.DeleteRow);
        else
            Debug.LogWarning("[Game] PowerUpsMenu not found.");

        gameInstance.soundSystemInstance.PlayLines(SoundSystem.linesSounds.FourLines);
    }

    static public void On2LinesWin(int line)
    {
        Debug.Log("2 LINES WINED!!");
        Game._linesCounter = 0;

        var currentPos = gameInstance.TwoLinesParticleSystem.transform.position;
        gameInstance.TwoLinesParticleSystem.transform.position = new Vector3(currentPos.x, line, currentPos.z);
        gameInstance.TwoLinesParticleSystem.Play();

        PowerUpsMenu menu = powerUpsMenu != null ? powerUpsMenu : FindFirstObjectByType<PowerUpsMenu>();

        if (menu != null)
            menu.AddPowerUp(DragAndDropElement.PowerUpType.CompleteRow);
        else
            Debug.LogWarning("[Game] PowerUpsMenu not found.");

        gameInstance.soundSystemInstance.PlayLines(SoundSystem.linesSounds.TwoLines);
    }

    static public void On3LinesWin(int line)
    {
        Debug.Log("3 LINES WINED!!");
        Game._linesCounter = 0;

        var currentPos = gameInstance.ThreeLinesParticleSystem.transform.position;
        gameInstance.ThreeLinesParticleSystem.transform.position = new Vector3(currentPos.x, line, currentPos.z);
        gameInstance.ThreeLinesParticleSystem.Play();

        PowerUpsMenu menu = powerUpsMenu != null ? powerUpsMenu : FindFirstObjectByType<PowerUpsMenu>();

        if (menu != null)
            menu.AddPowerUp(DragAndDropElement.PowerUpType.DeleteColum);
        else
            Debug.LogWarning("[Game] PowerUpsMenu not found.");

        gameInstance.soundSystemInstance.PlayLines(SoundSystem.linesSounds.ThreeLines);
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

    public static void ResetStaticState()
    {
        isPaused = false;
        TimeTimer = 0f;
        isGameStarted = false;
        isGameOver = false;

        currentFigure = null;

        _score = 0;
        _highScoreAchieved = false;

        _level = 1;
        _lines = 0;
        _linesCounter = 0;

        Time.timeScale = 1f;
    }
}
