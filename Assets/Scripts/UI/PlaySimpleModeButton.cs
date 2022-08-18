using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySimpleModeButton : MonoBehaviour
{
    public GameObject HUD;
    public GameObject mainMenu;
    public StartTimer timer;
    public GameObject gameElements;
    public Transition transition;

    public void PlayButton()
    {
        Invoke("PlaySimpleMode", 1.5f);
        transition.PlayEnterAnimation();
        transition.Invoke("PlayExitAnimation", 1.5f);
    }
    private void PlaySimpleMode()
    {
        HUD.SetActive(true);
        mainMenu.SetActive(false);
        timer.gameObject.SetActive(true);
        timer.StartGameCount();
        gameElements.SetActive(true);
    }

}
