//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Activates the object only if an input is given to it X times.
/// </summary>
public class CounterActivator : InteractiveActivator
{
    // Parameters
    public int numberOfRequiredInputs = 2;

    // State
    private int currentInputsActive = 0;

    protected override bool ActivateLogic()
    { 
        currentInputsActive++;
        return currentInputsActive >= numberOfRequiredInputs;
    }
    protected override bool DeactivateLogic()
    {
        currentInputsActive--;
        return currentInputsActive < numberOfRequiredInputs;
    }

}