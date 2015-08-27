using UnityEngine;
using System.Collections;

/// <summary>
/// Add this to an object to allow auto-activation on proximity
/// </summary>
public class ActivationTrigger : ObjectTrigger<InteractiveObject> {

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ENTER ACTIVATION FOR " + target.name);
        target.Activate();
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("EXIT ACTIVATION FOR " + target.name);
        target.Deactivate();
    }

}
