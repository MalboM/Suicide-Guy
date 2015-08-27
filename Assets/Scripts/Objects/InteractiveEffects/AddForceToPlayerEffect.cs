//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that adds a force to the player when activated
/// </summary>
public class AddForceToPlayerEffect : InteractiveEffect {

    public Vector3 direction;
    public float strength;
    public ForceMode forceMode;

    public override void ActivateLogic()
    {
        var playerMovement = Statics.instance.playerMovementController;
        playerMovement.AddForce(direction * strength, forceMode);
    }

    public override void DeactivateLogic()
    {
        // Nothing
    }
}
