//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Activates the object only if an object with the given tag is held by the player.
/// </summary>
public class HeldTagActivator : InteractiveActivator
{
    // Parameters
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