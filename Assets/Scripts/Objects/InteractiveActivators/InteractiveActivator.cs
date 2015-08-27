//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Receives activation input from other objects and relay is to its taregt interactive object
/// </summary>
public abstract class InteractiveActivator : MonoBehaviour
{
    /// <summary>
    /// Should return FALSE when the object can activate
    /// </summary>
    /// <returns></returns>
    public bool Activate()
    {
        return ActivateLogic();
    }

    /// <summary>
    /// Should return TRUE when the object can deactivate
    /// </summary>
    public bool Deactivate()
    {
        return DeactivateLogic();
    }
    /// <summary>
    /// Inner Activation logic
    /// </summary>
    /// <returns></returns>
    protected abstract bool ActivateLogic();

    /// <summary>
    /// Inner Deactivation logic
    /// </summary>
    /// <returns></returns>
    protected abstract bool DeactivateLogic();

}