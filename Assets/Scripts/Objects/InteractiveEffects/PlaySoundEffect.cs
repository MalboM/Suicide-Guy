//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// When triggered, plays a sound effect
/// </summary>
public class PlaySoundEffect : InteractiveEffect
{
    public AudioSource targetAudioSource;   // The audio source to play

    public override void ActivateLogic()
    {
        targetAudioSource.Play();
    }

    public override void DeactivateLogic()
    {
        targetAudioSource.Stop();
    }
}
