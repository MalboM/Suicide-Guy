using UnityEngine;
using System.Collections;

public class ParentTrigger : MonoBehaviour {



	void OnCollisionEnter(Collision other){

		other.transform.parent = this.transform;

	}

	void OnCollisionExit(Collision other){

		other.transform.parent = null;

	}
}
