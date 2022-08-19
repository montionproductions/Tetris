using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    public Transition transition;

    private bool wasActivated = false;

    private void Start()
    {
        wasActivated = false;
    }
    public void RestartButton ()
    {
        if (wasActivated)
            return;

        Time.timeScale = 1.0f;
        transition.PlayEnterAnimation();
        Invoke("RestartGame", 1.5f);

        wasActivated = true;

        GetComponent<Button>().interactable = false;
    }

    private void RestartGame()
    {
        Game gameIns = GameObject.FindObjectOfType<Game>();
        gameIns.Restart();
    }
}
