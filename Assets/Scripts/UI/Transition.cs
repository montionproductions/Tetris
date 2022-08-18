using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Transition : MonoBehaviour
{
    public bool showElements = false;

    public bool isPlaying = false;

    private void Start()
    {
        ShowElements(true);
        Invoke("PlayExitAnimation", 1.0f);
    }

    private void Update()
    {
        //if (!isPlaying)
        //    ShowElements(showElements);
    }
    public void PlayEnterAnimation()
    {
        ShowElements(true);
        foreach (Transform child in transform)
        {
            child.GetComponent<MoveAnimation>().PlayEnterAnimation();
        }
    }
    public void PlayExitAnimation()
    {
        ShowElements(true);
        foreach (Transform child in transform)
        {
            child.GetComponent<MoveAnimation>().PlayExitAnimation();
        }
    }

    public void ShowElements(bool activate)
    {
        foreach (Transform child in transform)
        {
            child.GetChild(0).gameObject.SetActive(activate);
        }
    }
}
