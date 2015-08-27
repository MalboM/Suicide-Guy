//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// A test script for a hand-activated component
/// </summary>
public class TestHandChangeColorComponent : MonoBehaviour {

    public MeshRenderer mr;

    void OnEnable()
    {
        mr.material.color = Color.cyan;
    }

    void OnDisable()
    {
        mr.material.color = Color.gray;
    }

}
