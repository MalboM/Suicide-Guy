//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Add this to an object to allow auto-activation on rigidbody mass
/// </summary>
public class RigidbodyActivationTrigger : ObjectTrigger<InteractiveObject> {

    // Parameters
    public float minimumMass = 1;   // Minimum mass for activation

    // References
    private List<GameObject> insideGos = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) rb = other.GetComponentInParent<Rigidbody>();
        if (rb == null) return;
        if (rb.mass < minimumMass) return;

        //Debug.Log("ENTER RIGIBODY FOR " + target.name);
        insideGos.Add(other.gameObject);
        if (insideGos.Count == 1) target.Activate();
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) rb = other.GetComponentInParent<Rigidbody>();
        if (rb == null) return;
        if (rb.mass < minimumMass) return;

        //Debug.Log("EXIT RIGIBODY FOR " + target.name);
        insideGos.Remove(other.gameObject);
        if (insideGos.Count == 0) target.Deactivate();
    }
}
