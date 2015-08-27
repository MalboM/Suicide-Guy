//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Activates the object only if the given tag is used on this object.
/// </summary>
public class UsedTagActivator : InteractiveActivator
{
    // Parameter
    public string requiredTag;

    protected override bool ActivateLogic()
    {
        var playerAction = Statics.instance.playerActionController;
        return (playerAction.IsHoldingSomething() && playerAction.HeldObject.objectTag == requiredTag);
    }

    protected override bool DeactivateLogic()
    {
        return true;
    }

}