using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimationUI : MonoBehaviour
{
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Enter()
    {
        gameObject.SetActive(true);
        anim.SetTrigger("Enter");
    }
    public void Exit()
    {
        Invoke("HideMenu", 1.30f);
        anim.SetTrigger("Exit");
    }

    private void HideMenu()
    {
        gameObject.SetActive(false);
    }
    public void Popup()
    {
        anim = GetComponent<Animator>();
        gameObject.SetActive(true);

        anim.SetTrigger("Popup");
    }
}
