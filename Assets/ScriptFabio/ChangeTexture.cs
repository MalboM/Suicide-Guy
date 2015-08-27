using UnityEngine;
using System.Collections;

public class ChangeTexture : MonoBehaviour {


	public Texture2D cavodrittoattivo;
	public Texture2D cavodrittodisattivo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		renderer.material.mainTexture = cavodrittoattivo;

	}

	void OnDisable(){
		renderer.material.mainTexture = cavodrittodisattivo;
	}
}
