using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticleOnEnabled : MonoBehaviour
{
    public ParticleSystem particleSystem;

    private void OnEnable()
    {
        particleSystem.Play();
    }
}
