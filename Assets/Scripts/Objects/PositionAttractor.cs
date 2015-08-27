using UnityEngine;
using System.Collections;

/// <summary>
/// The attached gameobject will try to match the position of the given target transform, smoothly.
/// </summary>
public class PositionAttractor : MonoBehaviour {

    public Transform targetTr;  // The target to match
    public float strength;      // The strength for matching (elastic)
    public float damping;       // The higher this is, the slower 
    public bool heightIsNotAffected;      // If true, the height of this object is not affected
	
	void FixedUpdate () {
        Vector3 distanceVector = (targetTr.position - transform.position);
        Vector3 force = distanceVector * strength;
        if (heightIsNotAffected) force.y = 0;
        rigidbody.AddForce(force,ForceMode.Acceleration);

        Vector3 tmpVel = rigidbody.velocity;
        tmpVel *= (1 - damping);
        if (heightIsNotAffected) tmpVel.y = rigidbody.velocity.y;
        rigidbody.velocity = tmpVel;
	}
}
