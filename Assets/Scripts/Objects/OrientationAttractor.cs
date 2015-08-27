using UnityEngine;
using System.Collections;

/// <summary>
/// The attached gameobject will try to match the orientation of the given target transform, smoothly.
/// </summary>
public class OrientationAttractor : MonoBehaviour {

    public Transform targetTr;              // The target transform to match
    public float dampingStrength = 100;     // The higher this is, the slower this orientates
    public bool yOnly = false;              // If true, match only around the vertical axis
    
    private bool ySteady = true;            // If true, tries to keep the XZ rotation around 0

    void FixedUpdate()
    {
        if (targetTr != null)
        {
            Quaternion targetRot = targetTr.rotation;

            if (yOnly)
            {
                var targetEulers = transform.localEulerAngles;
                targetEulers.y = targetTr.rotation.eulerAngles.y;
                if (ySteady)
                {
                    targetEulers.x = 0;
                    targetEulers.z = 0;
                }
                targetRot = Quaternion.Euler(targetEulers);
            }

            Quaternion slerpRot = Quaternion.Slerp(this.transform.rotation, targetRot, dampingStrength * Time.fixedDeltaTime);
            rigidbody.MoveRotation(slerpRot);
        }
    }
}
