using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicSystem : MonoBehaviour
{
    static AudioSource musicController;
    void Start()
    {
        musicController = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 
}
