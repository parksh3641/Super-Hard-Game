using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (GameStateManager.instance.Sfx)
        {
            audioSource.volume = GameStateManager.instance.SfxValue;
            audioSource.Play();
        }
    }
}
