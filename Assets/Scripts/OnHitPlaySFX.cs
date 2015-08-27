using UnityEngine;
using System.Collections;

public class OnHitPlaySFX : MonoBehaviour {

    public AudioSource audioSource;

	void OnCollisionEnter (Collision other) {
        if (this.rigidbody.velocity.sqrMagnitude > 1f)
            audioSource.Play();
	}
}
