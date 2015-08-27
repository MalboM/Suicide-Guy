//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Add this to an object to allow player interaction
/// </summary>
public class InteractionTrigger : ObjectTrigger<InteractiveObject> {

    void OnTriggerEnter(Collider other)
    {
        var action = other.GetComponent<PlayerActionController>();
        action.AddObject(target);
        //Debug.Log("ENTER INTERACTION FOR " + target.name);
    }

    void OnTriggerExit(Collider other)
    {
        var action = other.GetComponent<PlayerActionController>();
        action.RemoveObject(target);
        //Debug.Log("EXIT INTERACTION FOR " + target.name);
    }
}
