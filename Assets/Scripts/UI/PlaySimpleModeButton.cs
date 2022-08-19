using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySimpleModeButton : MonoBehaviour
{
    public GameObject HUD;
    public GameObject mainMenu;
    public StartTimer timer;
    public GameObject gameElements;
    public Transition transition;

    private bool wasActivated = false;
    private void Start()
    {
        wasActivated = false;
    }

    public void PlayButton()
    {
        if (wasActivated)
            return;

        wasActivated = true;

        Invoke("PlaySimpleMode", 1.5f);
        transition.PlayEnterAnimation();
        transition.Invoke("PlayExitAnimation", 1.5f);

        GetComponent<Button>().interactable = false;

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
