using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Transform[] figures;
    public float spawnTime;
    public Transform spawnPoint;

    public float movSpeed = 1f;
    public float movTime = 2.0f;

    private float _timerMoveCount;

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
        _timerMoveCount = 0f;

        SpawnRandomFigure();
    }

    public void SpawnRandomFigure()
    {
        int randNumer = Random.Range(0, figures.Length - 1);
        Instantiate(figures[randNumer], spawnPoint.transform.position, Quaternion.identity);
    }
}
