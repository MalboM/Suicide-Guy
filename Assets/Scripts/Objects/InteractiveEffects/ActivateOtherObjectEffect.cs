using UnityEngine;
using System.Collections;

/// <summary>
/// When triggered, activates the target object.
/// </summary>
public class ActivateOtherObjectEffect : InteractiveEffect {

    public InteractiveObject other; // The object to activate

    override public void ActivateLogic()
    {
        if (other != null) other.Activate();
    }

    public override void DeactivateLogic()
    {
        if (other != null) other.Deactivate();
    }
}
