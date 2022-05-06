using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartTimer : MonoBehaviour
{
    Game gameController;

    private void Awake()
    {
        gameController = GameObject.FindObjectOfType<Game>();
    }

    // Start is called before the first frame update
    public void StartGameCount()
    {
        StartCoroutine("StartCount");

        GetComponentInChildren<TMP_Text>().text = "3";
        StartCoroutine(ShowNumber(.75f, "2"));
        StartCoroutine(ShowNumber(.75f * 2, "1"));
        StartCoroutine(ShowNumber(.75f * 3, "Now!"));
    }

    IEnumerator StartCount()
    {
        yield return new WaitForSeconds(gameController.spawnTime);
        gameController.InitGame();
        this.gameObject.SetActive(false);
    }

    IEnumerator ShowNumber(float time, string number)
    {
        yield return new WaitForSeconds(time);
        GetComponentInChildren<TMP_Text>().text = number;
    }

}
