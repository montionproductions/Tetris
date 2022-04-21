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

    public float movSpeed = 1f;
    public float movTime = 2.0f;

    public static bool isPaused = false;

    public GameObject gameObjectMenu;

    private int[] _nextFigures;
    private Transform[] _nextFiguresObjects;


    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void Update()
    {

    }

    void InitGame()
    {
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

    public void AddLevel()
    {
        // Implemen add level un UI
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
}
