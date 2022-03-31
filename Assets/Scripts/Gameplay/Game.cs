using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Transform[] figures;
    public Color[] colors = new Color[6];

    public Transform[] figuresPreview;
    public Transform[] nextFiguresSpawnPoints;
    public Color[] figuresPreviewColors = new Color[3];

    public float spawnTime;
    public Transform spawnPoint;

    public float movSpeed = 1f;
    public float movTime = 2.0f;

    private int[] _nextFigures;
    private Transform[] _nextFiguresObjects;

    public static bool isPaused = false;

    public GameObject gameObjectMenu; 

    public enum COLLISION_TYPE
    {
        LIMIT = 0,
        DOWN = 1
    };

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
        /*Grup[] allGrups = GameObject.FindObjectsOfType<Grup>();
        for(int i = 0; i < allGrups.Length; i++)
        {
            Destroy(allGrups[i].transform.gameObject);
        }

        for(int i = 0; i < nextFiguresSpawnPoints.Length; i++)
        {
            Destroy(nextFiguresSpawnPoints[i].GetChild(0).gameObject);
        }

        Resume();
        InitGame();

        GameObject.Find("GameOverMenu").gameObject.SetActive(false);*/

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

    void _generateAllNextFigures()
    {   
        for(int i = 0; i < 3; i++)
        {
            _nextFigures[i] = Random.Range(0, figures.Length - 1);
            _nextFiguresObjects[i] = Instantiate(figuresPreview[_nextFigures[i]], nextFiguresSpawnPoints[i]);
            figuresPreviewColors[i] = colors[Random.Range(0, colors.Length - 1)];
        }
    }

    void _getNextFigure()
    {
        _nextFigures[0] = _nextFigures[1];
        _nextFigures[1] = _nextFigures[2];
        _nextFigures[2] = Random.Range(0, figures.Length - 1);

        figuresPreviewColors[0] = figuresPreviewColors[1];
        figuresPreviewColors[1] = figuresPreviewColors[2];
        figuresPreviewColors[2] = colors[Random.Range(0, colors.Length -1)];

        for (int i = 0; i < 3; i++)
        {
            Destroy(_nextFiguresObjects[i].gameObject);
            
            _nextFiguresObjects[i] = Instantiate(figuresPreview[_nextFigures[i]], nextFiguresSpawnPoints[i]);

            foreach (Transform child in _nextFiguresObjects[i])
            {
                child.GetComponent<SpriteRenderer>().color = figuresPreviewColors[i];
            }
        }
    }

    public Transform SpawnRandomFigure()
    {
        _getNextFigure();
        Transform obj = Instantiate(figures[_nextFigures[0]], spawnPoint.transform.position, Quaternion.identity);

        foreach (Transform child in obj)
        {
            child.GetComponent<SpriteRenderer>().color = figuresPreviewColors[0];
        }


        return obj;
    }
}
