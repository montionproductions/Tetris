using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Transform[] figures;

    public Transform[] figuresPreview;
    public Transform[] nextFiguresSpawnPoints;

    public float spawnTime;
    public Transform spawnPoint;

    public float MovSpeed = 1.0f;
    public float MovTime = 1.0f;

    public static bool isPaused = false;

    public GameObject gameObjectMenu;

    public List<Level> levels;

    private int[] _nextFigures;
    private Transform[] _nextFiguresObjects;

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

    }

    public void InitGame()
    {
        _initLevels();

        isPaused = false;     

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

    public void GameOver()
    {
        gameObjectMenu.SetActive(true);
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

        return obj;
    }

    private void _initLevels()
    {
        levels = new List<Level>();

        levels.Add(new Level(0f, 1f, 1)); // Level 1
        StartCoroutine("StartLevel", levels[0]);

        levels.Add(new Level(60f, .5f, 2)); // Level 2
        StartCoroutine("StartLevel", levels[1]);

        levels.Add(new Level(180f, .25f, 3)); // Level 3
        StartCoroutine("StartLevel", levels[2]);

        levels.Add(new Level(300f, .10f, 4)); // Level 4
        StartCoroutine("StartLevel", levels[3]);
    }

    IEnumerator StartLevel(Level level)
    {
        yield return new WaitForSeconds(level._timeToStart);
        GameObject.FindObjectOfType<Game>().OnLevelChange(level._movTime, level._level);
    }
}
