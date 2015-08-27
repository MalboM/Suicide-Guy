//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that activates the target component
/// </summary>
public class ActivateComponentEffect : InteractiveEffect {

    public MonoBehaviour target;    // Component to activate

    public override void ActivateLogic()
    {
        target.enabled = true;
    }

    public override void DeactivateLogic()
    {
        target.enabled = false;
    }
}
