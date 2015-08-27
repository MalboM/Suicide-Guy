using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that changes the material of a target object.
/// Uses the ObjectMaterialController
/// </summary>
public class ChangeMaterialEffect : InteractiveEffect {

    // Parameters
    public ObjectMaterialController materialController;     // The material controller that will be changed
    public Material material;       // The material to add

    public override void ActivateLogic()
    {
        materialController.Add(material, 1);
    }

    public override void DeactivateLogic()
    {
        materialController.Remove(material);
    }

}
