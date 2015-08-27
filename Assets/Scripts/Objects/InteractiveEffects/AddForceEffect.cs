//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that adds a force to the object.
/// </summary>
public class AddForceEffect : InteractiveEffect {

    public Vector3 direction;       // Direction of the force
    public float strength;          // Strength of the force
    public ForceMode forceMode;     // Mode of the force

    public override void ActivateLogic()
    {
        this.GetComponentInParent<Rigidbody>().AddForce(direction * strength, forceMode);
    }

    public override void DeactivateLogic()
    {
        // Nothing
    }
}
