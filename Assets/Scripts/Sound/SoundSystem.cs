using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundSystem : MonoBehaviour
{
    static AudioSource soundController;

    public AudioClip hardDrope;
    public AudioClip move;
    public AudioClip rotate;
    public List<AudioClip> lines = new List<AudioClip>();
    public enum linesSounds
    {
        TwoLines = 0,
        ThreeLines = 1,
        FourLines = 2
    }

    private void Awake()
    {
        soundController = GetComponent<AudioSource>();
    }

    public void PlayHardDrope()
    {
        soundController.PlayOneShot(hardDrope);
    }
    public void PlayMove()
    {
        soundController.PlayOneShot(move);
    }
    public void PlayRotation()
    {
        soundController.PlayOneShot(rotate);
    }
    public void PlayLines(linesSounds index)
    {
        soundController.PlayOneShot(lines[(int)index]);
    }
}
