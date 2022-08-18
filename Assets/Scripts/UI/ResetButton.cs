using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public Transition transition;
    public void RestartButton ()
    {
        Time.timeScale = 1.0f;
        transition.PlayEnterAnimation();
        Invoke("RestartGame", 1.5f);
    }

    private void RestartGame()
    {
        Game gameIns = GameObject.FindObjectOfType<Game>();
        gameIns.Restart();
    }
}
