//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The main behaviour to be attached to any object that the player can interact with in some way.
/// This will get all InteractiveEffects it will find in its children and Activate them when the InteractiveObject is Activated.
/// An InteractiveActivator found in the children can be used to change how this is activated.
/// </summary>
public class InteractiveObject : MonoBehaviour {

    public enum ActivationMethod
    {
        ALWAYS_ON,   // Once activates, stays activated
        TOGGLED,     // Each time the object is activated, it is actually switched ON or OFF
        MULTIPLE,    // This object can be activated multiple times.
        PICK_UP      // Activates when picked up
    }

    // Parameters
    public bool canBeTouched;           // If true, you can use Action2 to interact with this object when close
    public ActivationMethod activationMethod = ActivationMethod.ALWAYS_ON;  // How this can be activated
    public bool canBeThrown;            // Can this object be thrown, when picked up?
    public bool canBePickedUp;          // Can this object be picked up at all?
    public bool canBeDragged;           // Can this object be dragged around?

    public bool canBeHeld;              // If true, this will disappear and the hand should appear instead!
    public bool canBeUsedWhileHeld;     // If true, this object can be used while hand using Action2
    public HandType heldHandType;       // What type of hand to show when this is held in hand

    public string objectTag;            // What tag is assigned to this object (used by some activators and effects)

    // Generic configuration
    public static float DROP_IMPULSE = 1f;  // Velocity impulse when dropping stuff

    // References
    InteractiveEffect[] effects;            // Effects found in the children
    InteractiveActivator activator;         // Effect activator
    HintHandler hintHandler;                // Controls the hint handler
    Collider physicsCollider;               // The collider that is used by the rigidbody
    ObjectMaterialController matController; // Controller that changes materials when needed

    public ObjectMaterialController MaterialController
    {
        get { return this.matController; }
    }

    // State
    private bool _isActivated;          // Is this InteractiveObject activated now?

    #region Initialisation
    void Awake () {
        _isActivated = false;
        effects = GetComponentsInChildren<InteractiveEffect>();
        activator = GetComponentInChildren<InteractiveActivator>();
        hintHandler = GetComponent<HintHandler>();
        matController = GetComponentInChildren<ObjectMaterialController>();

        var physicsColliderTr = transform.Find("PhysicsCollider");
        if(physicsColliderTr != null) physicsCollider = physicsColliderTr.GetComponent<Collider>();
	}
    #endregion

    #region Player Actions

    /// <summary>
    /// The player touches (Action2) the object
    /// </summary>
    /// <returns>Returns true if it was touched</returns>
    public bool ActionTouch()
    {
        if (canBeTouched)
        {
            switch (activationMethod)
            {
                case ActivationMethod.ALWAYS_ON:
                case ActivationMethod.MULTIPLE:
                    Activate(); break;
                case ActivationMethod.TOGGLED:
                    ToggleActivation(); break;
            }
        }
        return canBeTouched;
    }

    /// <summary>
    /// The player uses (Action2) the held object
    /// </summary>
    /// <returns>Returns true if it was used</returns>
    public bool ActionUse()
    {
        if (canBeUsedWhileHeld) Activate();
        return canBeUsedWhileHeld;
    }

    /// <summary>
    /// The player picks up (Action1) the pointed object
    /// </summary>
    /// <returns>Returns true if it was picked up</returns>
    public bool ActionPickUp(PlayerActionController ac)
    {
        if (canBePickedUp) PickUp(ac);
        else if (canBeDragged) Drag(ac);
        else if (canBeHeld) HoldInHand(ac);

        bool ok = canBePickedUp || canBeDragged || canBeHeld;
        if (activationMethod == ActivationMethod.PICK_UP && ok) Activate();
        return ok;
    }

    /// <summary>
    /// The player throws (Action1) the picked up object
    /// </summary>
    /// <returns>Returns true if it was thrown</returns>
    public bool ActionThrow(PlayerActionController ac, float throwStrength, Vector3 direction)
    {
        if (canBeThrown) Throw(ac, throwStrength, direction);
        return canBeThrown;
    }

    /// <summary>
    /// The player drops (Action1) the picked up object
    /// </summary>
    /// <returns>Returns true if it was dropped</returns>
    public bool ActionDrop(PlayerActionController ac)
    {
        if (canBePickedUp || canBeHeld || canBeDragged) Drop(ac);
        return canBePickedUp || canBeHeld || canBeDragged;
    }
    #endregion

    #region Interaction Logic

    public void ToggleActivation()
    {
        if (IsActivated) Deactivate();
        else Activate();
    }
    public bool IsActivated
    {
        get { return this._isActivated; }
    }
    public void Activate()
    {
        // Check if we can activate this only once or not
        if (_isActivated && (activationMethod != ActivationMethod.MULTIPLE)) return;   

        // If there is an Activator, use that to determine if we need to activate. Otherwise, activate.
        if (activator == null || activator.Activate())
        {
            _isActivated = true;
            //Debug.Log(this + " ACTIVATES!");
            foreach (var e in effects)
                e.Activate();
        }
    }

    public void Deactivate()
    {
        // Check if we can activate this only once or not
        if (!_isActivated) return;

        // If there is an Activator, use that to determine if we need to activate. Otherwise, activate.
        if (activator == null || activator.Deactivate())
        {
            _isActivated = false;
            //Debug.Log(this + " DEACTIVATES!");
            foreach (var e in effects)
                e.Deactivate();
        }
    }

    /// <summary>
    /// The target object is picked up.
    /// </summary>
    /// <param name="ac">The player's action controller</param>
    void PickUp(PlayerActionController ac)
    {
        ac.SetHeldObject(this, this.heldHandType);


        // Parenting: uncomment this to achieve non-physical motion
        //this.transform.parent = ac.pickingUpPivotTr;

        this.physicsCollider.gameObject.layer = LayerMask.NameToLayer("PickedupObject");

        // Follow the position
        var posAttractor = GetComponent<PositionAttractor>();
        posAttractor.heightIsNotAffected = false;
        posAttractor.targetTr = ac.pickingUpPivotTr;
        posAttractor.enabled = true;

        // Follow the rotation
        var rotAttractor = GetComponent<OrientationAttractor>();
        rotAttractor.targetTr = ac.pickingUpPivotTr;
        rotAttractor.enabled = true;

        // No gravity
        this.rigidbody.useGravity = false;
    }

    /// <summary>
    /// The target object is dragged around.
    /// </summary>
    /// <param name="ac">The player's action controller</param>
    void Drag(PlayerActionController ac)
    {
        ac.SetHeldObject(this, this.heldHandType);

        this.physicsCollider.gameObject.layer = LayerMask.NameToLayer("PickedupObject");

        // Follow the position
        var posAttractor = GetComponent<PositionAttractor>();
        posAttractor.heightIsNotAffected = true;
        posAttractor.targetTr = ac.pickingUpPivotTr;
        posAttractor.enabled = true;

        // Follow the rotation
        var rotAttractor = GetComponent<OrientationAttractor>();
        rotAttractor.targetTr = ac.pickingUpPivotTr;
        rotAttractor.enabled = true;

        // Slow down player
        Statics.instance.playerMovementController.SetSpeedModifier(0.5f);
        Statics.instance.playerMovementController.DisableJump();

        // No gravity
        this.rigidbody.useGravity = true;
    }

    /// <summary>
    /// The target object is held in hand.
    /// </summary>
    /// <param name="ac">The player's action controller</param>
    void HoldInHand(PlayerActionController ac)
    {
        ac.SetHeldObject(this, this.heldHandType, inHand:true);

        // This object disappears, as it will appear in hand
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// The current held object is dropped.
    /// </summary>
    /// <param name="ac">The player's action controller</param>
    void Drop(PlayerActionController ac)
    {
        // If it was held in hand
        if (canBeHeld) 
        {
            this.gameObject.SetActive(true);
            float dropBackOffset = 1f;
            this.transform.position = ac.transform.position + ac.transform.forward * (-dropBackOffset);
            float castDistance = 1f;

            // AC pos
            //Debug.Log("Check dropping: AC position " + ac.transform.position);
            //Debug.DrawRay(ac.transform.position, Vector3.up, Color.red, 2.0f);

            // FROM pos
            //Debug.Log("Check dropping: FROM position " + this.transform.position);
            //Debug.DrawRay(this.transform.position, Vector3.up, Color.blue, 2.0f);

            // RAYCAST
            //Debug.Log("Check dropping: FROM position " + this.transform.position);
            //Debug.DrawRay(this.transform.position, ac.transform.forward * castDistance, Color.magenta, 2.0f);

            // Check if we can drop it here
            RaycastHit hitInfo;
            LayerMask mask = 1 << LayerMask.NameToLayer("PlayerTrigger");
            mask = ~mask;
            if (!Physics.Raycast(this.transform.position, ac.transform.forward, out hitInfo, castDistance, mask))
            {
                //Debug.Log("We can drop it!");
                // Can drop it here, drop it with a velocity impulse
                this.transform.position += ac.transform.forward * dropBackOffset / 2; // We move it a bit farther
                this.rigidbody.AddForce(ac.transform.forward * DROP_IMPULSE, ForceMode.VelocityChange);
            }
            else
            {
                //Debug.Log("We cannot drop it! Leave it just in front of us! We hit a " + hitInfo.collider.gameObject.name);
                // Cannot drop it here, just drop it without an impulse
                float feetDistance = 0.5f;
                float forwardOffset = 0.45f;

                this.transform.position = Statics.instance.playerMovementController.transform.position;
                this.transform.position += Vector3.down * feetDistance; // A bit downfloat forwardOffset = 0.1f;
                this.transform.position += Statics.instance.playerMovementController.transform.forward * forwardOffset; // A bit forward
                //Debug.DrawRay(this.transform.position, Vector3.up, Color.black, 2.0f);
            }
        }

        // If it was dragged or picked up
        else if (canBeDragged || canBePickedUp)
        {
            this.physicsCollider.gameObject.layer = LayerMask.NameToLayer("Default");

            // Just drop it
            var posAttractor = GetComponent<PositionAttractor>();
            posAttractor.enabled = false;
            var rotAttractor = GetComponent<OrientationAttractor>();
            rotAttractor.enabled = false;
        }

        // Reset player movement modifier settings
        Statics.instance.playerMovementController.ResetSpeedModifier();
        Statics.instance.playerMovementController.ResetDisableJump();

        this.transform.parent = null;
        this.rigidbody.useGravity = true;

        ac.ClearHeldObject();
    }

    /// <summary>
    /// The current held object is thrown.
    /// </summary>
    /// <param name="ac">The player's action controller</param>
    /// <param name="throwStrength">Strength with which to throw the action</param>
    /// <param name="direction">Direction of the throw</param>
    void Throw(PlayerActionController ac, float throwStrength, Vector3 direction)
    {
        Drop(ac);
        this.rigidbody.AddForce(direction*throwStrength,ForceMode.Impulse);
    }
    #endregion


    #region Mesh Hint Handling

    /// <summary>
    /// Set this object as pointed or not pointed by the player.
    /// </summary>
    /// <param name="choice">Pointed?</param>
    public void SetPointed(bool choice)
    {
        this.hintHandler.SetHintEnabled(choice);
    }

    #endregion
}
