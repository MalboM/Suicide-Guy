//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the material switching for a mesh.
/// Supports interaction from multiple requests as a stack.
/// </summary>
public class ObjectMaterialController : MonoBehaviour
{
    [System.Serializable]
    public struct MaterialRequest {
        public Material mat;    // If single, use this
        public Material[] mats; // If multiple, use this

        public int priority;    // Higher -> more likely this will be used
    }

    // References
    MeshRenderer meshRenderer;
    List<MaterialRequest> materialRequests;

    /// <summary>
    /// Initialisation
    /// </summary>
	void Awake () {
        meshRenderer = this.GetComponent<MeshRenderer>();
        materialRequests = new List<MaterialRequest>();

        // Initial material
        Add(this.meshRenderer.materials, 0);   
	}

    /// <summary>
    /// Add a new Material with a priority (materials with higher priorities are shown first).
    /// </summary>
    /// <param name="mat">The material</param>
    /// <param name="priority">The priority</param>
    public void Add(Material mat, int priority)
    {
        var mr = new MaterialRequest();
        mr.mat = mat;
        mr.priority = priority;
        Add(mr);
    }

    /// <summary>
    /// Add a new array of Materials with a priority (materials with higher priorities are shown first).
    /// </summary>
    /// <param name="mats">The materials</param>
    /// <param name="priority">The priority</param>
    public void Add(Material[] mats, int priority)
    {
        var mr = new MaterialRequest();
        mr.mats = mats;
        mr.priority = priority;
        Add(mr);
    }

    /// <summary>
    /// Add a new MaterialRequest
    /// </summary>
    /// <param name="mr">The MaterialRequest to add</param>
    void Add(MaterialRequest mr)
    {
        for (int i = 0; i < materialRequests.Count; i++)
            if (mr.priority < materialRequests[i].priority)
            {
                materialRequests.Insert(i, mr);
                break;
            }
        if (!materialRequests.Contains(mr)) materialRequests.Add(mr); // At the end if not added already
        UpdateMaterial();
    }

    /// <summary>
    /// Remove a Material.
    /// </summary>
    /// <param name="m">The material</param>
    public void Remove(Material mat)
    {
        for (int i = 0; i < materialRequests.Count; i++)
            if (mat == materialRequests[i].mat)
            {
                materialRequests.RemoveAt(i);
                break;
            }
        UpdateMaterial();
    }
    /// <summary>
    /// Remove Materials.
    /// </summary>
    /// <param name="mats">The materials</param>
    public void Remove(Material[] mats)
    {
        for (int i = 0; i < materialRequests.Count; i++)
            if (mats == materialRequests[i].mats)
            {
                materialRequests.RemoveAt(i);
                break;
            }
        UpdateMaterial();
    }

    /// <summary>
    /// Inner update of the material
    /// </summary>
    void UpdateMaterial()
    {
        var currentRequest = this.materialRequests[this.materialRequests.Count - 1];
        if (currentRequest.mats != null)
            this.meshRenderer.materials = currentRequest.mats;
        else
        {
            var newMats = new Material[this.meshRenderer.materials.Length];
            for (int i = 0; i < newMats.Length; i++)
                newMats[i] = currentRequest.mat;
            this.meshRenderer.materials = newMats;
        }
    }

}
