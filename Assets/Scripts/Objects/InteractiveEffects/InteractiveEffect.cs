//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract class that executes the logic of effects when interactive objects are activated and deactivated.
/// </summary>
public abstract class InteractiveEffect : MonoBehaviour
{
    /// <summary>
    /// What happens when the object is activated
    /// </summary>
    public void Activate()
    {
        ActivateLogic();
    }

    /// <summary>
    /// What happens when the object is deactivated
    /// </summary>
    public void Deactivate()
    {
        DeactivateLogic();
    }

    /// <summary>
    /// Inner activation logic to be implemented
    /// </summary>
    public abstract void ActivateLogic();

    /// <summary>
    /// Inner deactivation logic to be implemented
    /// </summary>
    public abstract void DeactivateLogic();

}