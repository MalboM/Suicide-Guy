//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// When triggered, this takes the object held by the player and this object and creates a new one.
/// </summary>
public class MergeObjectsEffect : InteractiveEffect
{
    public GameObject mergedObject; // The object that will result by the merging

    void Awake()
    {
        mergedObject.SetActive(false);
    }   

    public override void ActivateLogic()
    {
        var playerAction = GameObject.FindObjectOfType<PlayerActionController>();
        if (playerAction.IsHoldingSomething()){
            this.transform.parent.gameObject.SetActive(false);

            var obj = playerAction.HeldObject;
            playerAction.ForceDrop();
            obj.gameObject.SetActive(false);

            mergedObject.transform.position = this.transform.position;
            mergedObject.SetActive(true);
        }
    }
    public override void DeactivateLogic()
    {
        // Cannot be undone
    }

}
