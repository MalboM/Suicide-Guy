//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Uses this object in hand to activate another pointed object, if the tag is correct
/// </summary>
public class ChangeColorEffect : InteractiveEffect {

    // Parameters
    public MeshRenderer targetRenderer;     // The renderer to change the color to
    public Color color = Color.white;       // The color to change to

    // State
    private Color startColor;

    void Awake(){
        startColor = targetRenderer.material.color;
    }

    public override void ActivateLogic()
    {
        targetRenderer.material.color = color;
    }

    public override void DeactivateLogic()
    {
        targetRenderer.material.color = startColor;
    }
}
