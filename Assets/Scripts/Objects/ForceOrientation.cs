using UnityEngine;
using System.Collections;

/// <summary>
/// Forces the orientation of the attached object to that of the target.
/// </summary>
public class ForceOrientation : MonoBehaviour {

    public Transform targetTr;

	void LateUpdate () {
        transform.rotation = targetTr.rotation;
	}
}
