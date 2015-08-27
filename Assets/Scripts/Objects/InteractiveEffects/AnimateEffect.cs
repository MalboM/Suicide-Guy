//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that plays an animation when triggered
/// </summary>
public class AnimateEffect : InteractiveEffect
{
    public Animation targetAnimation;   // The animation that will be played
    public bool onDeactivationPlayBackwards = false;    // If true, the animation will play backwards

    private float lastTime = 0;

    public override void ActivateLogic()
    {
        if (onDeactivationPlayBackwards)
        {
            if (targetAnimation[targetAnimation.clip.name].normalizedTime < 0)
                targetAnimation[targetAnimation.clip.name].normalizedTime = 0;
            targetAnimation[targetAnimation.clip.name].speed = 1;
        }
        else
        {
            targetAnimation[targetAnimation.clip.name].normalizedTime = lastTime;
        }

        targetAnimation.Play();
    }

    public override void DeactivateLogic()
    {
        if (onDeactivationPlayBackwards)
        {
            if (targetAnimation[targetAnimation.clip.name].normalizedTime > 1)
                targetAnimation[targetAnimation.clip.name].normalizedTime = 1;
            targetAnimation[targetAnimation.clip.name].speed = -1;
            targetAnimation.Play();
        }
        else
        {
            lastTime = targetAnimation[targetAnimation.clip.name].normalizedTime;
            targetAnimation.Stop();
        }
    }
}
