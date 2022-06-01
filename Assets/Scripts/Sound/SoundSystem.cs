using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundSystem : MonoBehaviour
{
    static AudioSource musicController;

    private void Awake()
    {
        musicController = GetComponent<AudioSource>();
    }

    static void Init()
    {
        // Cargar volumen de archivo de configuracion .json
    }
}
