//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the hinting of interactive objects, changing materials where needed.
/// </summary>
public class HintHandler : MonoBehaviour {

    /// <summary>
    /// The material to use as the hinting one.
    /// </summary>
    public Material hintingMaterial;

    // References
    private ObjectMaterialController matController;

    void Awake()
    {
        matController = GetComponentInChildren<ObjectMaterialController>();
        if (matController == null) Debug.LogWarning("Cannot find an ObjectMaterialController for the mesh of " + this.name);
    }

    #region Enabling & Disabling
    public void SetHintEnabled(bool choice)
    {
        if (choice) EnableHint();
        else DisableHint();
    }

    void EnableHint()
    {
        this.matController.Add(hintingMaterial, 100);
	}

    void DisableHint()
    {
        this.matController.Remove(hintingMaterial);
    }
    #endregion
}
