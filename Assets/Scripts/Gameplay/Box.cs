using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    Game game;

    public bool isStatic = false;

    private void Awake()
    {
        game = GameObject.FindObjectOfType<Game>();
        isStatic = false;
    }

}
