//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// When triggered, destroys the current held object.
/// </summary>
public class DestroyObjectInHandEffect : InteractiveEffect {

    override public void ActivateLogic()
    {
        var playerAction = Statics.instance.playerActionController;
        var obj = playerAction.HeldObject;
        playerAction.ForceDrop();
        obj.gameObject.SetActive(false);    // Inactive (change with destroy if needed)
    }

    public override void DeactivateLogic()
    {
        // Nothing
    }
}
