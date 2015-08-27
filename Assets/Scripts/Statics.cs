//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Class that holds references to common game parts as a singleton.
/// </summary>
public class Statics : MonoBehaviour {

    public static Statics instance;

    // References
    public RigidbodyFirstPersonController playerMovementController;
    public PlayerActionController playerActionController;

	void Awake () {
        instance = this;

        Initialise();
	}
	
    void Initialise()
    {
        playerMovementController = GameObject.FindObjectOfType<RigidbodyFirstPersonController>();
        playerActionController = GameObject.FindObjectOfType<PlayerActionController>();
	}
}
