using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundSystem : MonoBehaviour
{
    AudioSource musicController; 

    private void Awake()
    {
        musicController = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
