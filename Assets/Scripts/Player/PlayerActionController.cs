//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnitySampleAssets.CrossPlatformInput;

/// <summary>
/// Class that controls how the player can interact with the game world.
/// </summary>
public class PlayerActionController : MonoBehaviour {

    // Parameters
    public float maxThrowBuildUpTime = 1f;
    public float throwStrength = 10;

    public float clickIsPressTime = 0.2f;                    // Time to treat a click as a click or as a press-release
    public float automatingDroppingDistanceThreshold = 5f;      // If a held object is farthest from this distance, it will be automatically dropped

    // References
    public Transform pickingUpPivotTr;
    public HandSwitcher handSwitcher;

    // Materials
    public Material pickedUpMaterial;

    // Internal state
    private bool heldObjectIsInHand;
    private Vector3 startPivotLocalPosition;

    // Object references
    List<InteractiveObject> currentInteractableObjects;
    InteractiveObject currentPointedObject;
    InteractiveObject heldObject;

    #region Game Logic
    public void Awake()
    {
        startPivotLocalPosition = this.pickingUpPivotTr.localPosition; 
        currentInteractableObjects = new List<InteractiveObject>();
        currentPointedObject = null;
        heldObject = null;
        heldObjectIsInHand = false;
    }

    void Update()
    {
        CheckCurrentInteractableObject();

        if (GetPrimaryActionButtonUp())
        {
            HandlePrimaryAction();
            HandlePrimaryLongAction();
            StopCoroutine("HandlePressCO");
        }

        if (GetPrimaryActionButtonDown())
            StartCoroutine("HandlePressCO");

        if (GetSecondaryActionButtonDown())
            HandleSecondaryAction();

        CheckLosingGrip();

        CheckHandSwitching();
	}

    /// <summary>
    /// Makes sure to remove from hand an object if it gets too far
    /// </summary>
    private void CheckLosingGrip()
    {
        if (!IsHoldingSomething()) return;
        if (heldObjectIsInHand) return;         // Not dropping if in hand
        float distanceSq = (this.heldObject.transform.position - transform.position).sqrMagnitude;
        float thresholdSq = automatingDroppingDistanceThreshold * automatingDroppingDistanceThreshold;
        if (distanceSq > thresholdSq)
        {
            //Debug.Log("DROPPING 'CAUSE TOO FAR!");
            this.heldObject.ActionDrop(this);
        }
    }

    private void CheckHandSwitching()
    {
        // Hand
        if (!IsHoldingSomething()){ // When holding, the object will tell what hands to use
            if (this.IsPointingSomething()) handSwitcher.ShowHand(HandType.HINT_PICKUP);
            else handSwitcher.ShowHand(HandType.NONE);
        }
    }
    #endregion

    #region Press Logic
    float pressTime = 0;
    float currentThrowBuildup = 0;
    IEnumerator HandlePressCO()
    {
        pressTime = 0;
        while (true)
        {
            pressTime += Time.deltaTime;
            currentThrowBuildup = Mathf.Clamp(pressTime, 0, maxThrowBuildUpTime) / maxThrowBuildUpTime;
            this.pickingUpPivotTr.localPosition = startPivotLocalPosition + Vector3.forward * (-currentThrowBuildup);
            yield return null;
        }
    }
    #endregion

    #region Action Handlers
    void HandlePrimaryAction()
    {
        if (!(pressTime < clickIsPressTime)) return;
        if (!IsHoldingSomething())
        {
            // You can pickup the currently pointed object
            if (IsPointingSomething())
                currentPointedObject.ActionPickUp(this);
        }
        else
        {
            this.heldObject.ActionDrop(this);
        }
    }

    void HandlePrimaryLongAction()
    {
        if (!(pressTime >= clickIsPressTime)) return;
        if (IsHoldingSomething())
            heldObject.ActionThrow(this, currentThrowBuildup * throwStrength, this.transform.forward);
    }

    void HandleSecondaryAction()
    {
        if (IsHoldingSomething())
        {
            // Use the currently held object
            heldObject.ActionUse();
        }
        else
        {
            // Touch other objects when not holding another object
            foreach (var o in currentInteractableObjects)
                o.ActionTouch();
        }
    }
    #endregion

    #region Objects Interaction
    /// <summary>
    /// Adds a new interectable object to this controller's reach
    /// </summary>
    /// <param name="o">The object to add</param>
    public void AddObject(InteractiveObject o)
    {
        this.currentInteractableObjects.Add(o);
    }

    /// <summary>
    /// Remove an interectable object from this controller's reach
    /// </summary>
    /// <param name="o">The object to remove</param>
    public void RemoveObject(InteractiveObject o)
    {
        this.currentInteractableObjects.Remove(o);
    }

    /// <summary>
    /// Set a object as held in hand (or picked up)
    /// </summary>
    /// <param name="o">Object to hold</param>
    /// <param name="handType">Hands to show</param>
    /// <param name="inHand">Is it in hand instead of picked up?</param>
    public void SetHeldObject(InteractiveObject o, HandType handType = HandType.TEST_1, bool inHand = false)
    {
        //Debug.Log("Holding object with hands " + handType);
        this.heldObjectIsInHand = inHand;
        if (inHand)
        {
            this.currentInteractableObjects.Remove(o); // We make sure to remove it from the current interactiable objects, since the trigger won't be triggered now!
        }
        else
        {
            o.MaterialController.Add(this.pickedUpMaterial, 150);
        }
        this.handSwitcher.ShowHand(handType);
        this.heldObject = o;
    }

    /// <summary>
    /// Clear the currently held object
    /// </summary>
    public void ClearHeldObject()
    {
        //Debug.Log("CLEAR!");
        this.handSwitcher.ShowHand(HandType.NONE);
        this.heldObject.MaterialController.Remove(this.pickedUpMaterial);
        this.heldObject = null;
        this.heldObjectIsInHand = false;
    }

    /// <summary>
    /// Force a drop
    /// </summary>
    public void ForceDrop()
    {
        if (!this.IsHoldingSomething()) return;
        this.heldObject.ActionDrop(this);
    }

    /// <summary>
    /// Check what is the current object with which we can interact from the list of interactable objects
    /// </summary>
    private void CheckCurrentInteractableObject()
    {
        float minDistanceSqr = Mathf.Infinity;
        InteractiveObject closestObject = null;
        foreach (var o in currentInteractableObjects)
        {
            float distanceSqr = (o.transform.position - this.transform.position).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                closestObject = o;
            }
        }

        if (this.currentPointedObject != closestObject)
        {
            if (this.currentPointedObject != null) this.currentPointedObject.SetPointed(false);
            if (closestObject != null) closestObject.SetPointed(true);
        }

        this.currentPointedObject = closestObject;
    }


#endregion 

#region Checks
    public bool IsHoldingSomething()
    {
        return this.heldObject != null;
    }

    public bool IsPointingSomething()
    {
        return this.currentPointedObject != null;
    }
#endregion


#region Getters
    public InteractiveObject HeldObject
    {
        get { return this.heldObject; }
    }
    public InteractiveObject PointedObject
    {
        get { return this.currentPointedObject; }
    }
    public Hand CurrentHand
    {
        get { return this.handSwitcher.CurrentHand; }
    }
    #endregion

    #region Buttons & Axes
    bool GetPrimaryActionButtonDown()
    {
        return CrossPlatformInputManager.GetButtonDown("Action1");
    }
    bool GetPrimaryActionButtonUp()
    {
        return CrossPlatformInputManager.GetButtonUp("Action1");
    }
    bool GetSecondaryActionButtonDown()
    {
        return CrossPlatformInputManager.GetButtonDown("Action2");
    }
    #endregion
}
