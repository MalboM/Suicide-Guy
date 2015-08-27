//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// When triggered, activates the current hand.
/// </summary>
public class ActivateHandEffect : InteractiveEffect {

    override public void ActivateLogic()
    {
        var playerAction = Statics.instance.playerActionController;
        playerAction.CurrentHand.Activate();
    }

    public override void DeactivateLogic()
    {
        // Nothing
    }
}
