//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Uses this object in hand to activate another pointed object, if the tag is correct
/// </summary>
public class ActivatePointedObjectEffect : InteractiveEffect
{
    override public void ActivateLogic()
    {
        var playerAction = Statics.instance.playerActionController;
        if (playerAction.IsPointingSomething()
            && playerAction.PointedObject.objectTag == playerAction.HeldObject.objectTag)
        {
            playerAction.PointedObject.Activate();
        }
    }

    public override void DeactivateLogic()
    {
        // Nothing
    }
}

