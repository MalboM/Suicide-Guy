//Copyright 2015 Michele Pirovano
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

/// <summary>
/// Controller of player movement. Classic rigidbody version.
/// </summary>
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
public class ClassicFirstPersonController: MonoBehaviour
{
    [System.Serializable]
    public class MovementSettings
    {
        public float ForwardSpeed = 8.0f; // Speed when walking forward
        //public float BackwardSpeed = 4.0f; // Speed when walking backwards
        //public float StrafeSpeed = 4.0f; // Speed when walking sideways
        public float SprintSpeed = 10.0f; // Speed when sprinting
        public float JumpHeight = 2.0f;
        public float GravityMultiplier = 1f; // Increases gravity

        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
       
        [HideInInspector]
        public float JumpSpeed = 0.0f; // WILL BE COMPUTED
        [HideInInspector]
        public float CurrentTargetSpeed = 8f;
        private bool running;

        public void UpdateDesiredTargetSpeed(bool runningInput)
        {
            if (runningInput)
            {
                CurrentTargetSpeed = SprintSpeed;
                running = true;
                return;
            }
            CurrentTargetSpeed = ForwardSpeed;
            running = false;
        }

        public bool Running
        {
            get { return running; }
        }
    }

    [System.Serializable]
    public class AdvancedSettings
    {
        public float groundCheckDistance = 0.01f;           // Distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float stickToGroundHelperDistance = 0.5f;    // Stops the character when hitting the ground 
        public float slowDownRate = 20f;                    // Rate at which the controller comes to a stop when there is no input
        public bool airControl;                             // Can the user control the direction that is being moved in the air?
        public float airControlSteerMultiplier = 1;                        // Controls the steer power while in air
        public bool zeroInertiaOnStop;                      // If true, the player will stop as soon as we stop inputing anything
        public bool allowRunning;                           // Can the user make the avatar run by pressing a button?
    }

    [System.Serializable]
    public class ModifierSettings
    {
        public float speedModifier = 1; // Changes the speed
        public bool disableJump = false;
    }


    public Camera _camera;
    public MovementSettings movementSettings = new MovementSettings();
    public MouseLook mouseLook = new MouseLook();
    public AdvancedSettings advancedSettings = new AdvancedSettings();
    public ModifierSettings modifierSettings = new ModifierSettings();

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private float yRotation;
    private Vector3 groundContactNormal;
    private bool jumpAction, previouslyGrounded, jumping, isGrounded;

    public Vector3 Velocity
    {
        get { return rb.velocity; }
    }

    public bool Grounded
    {
        get { return isGrounded; }
    }

    public bool Jumping
    {
        get { return jumping; }
    }

    public bool Running
    {
        get { return movementSettings.Running; }
    }


    #region Game Logic
    private void Start()
    {
        movementSettings.JumpSpeed = CalculateJumpVerticalSpeed();

        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        RotateView();

        if (GetJumpButton() && !jumpAction && CanJump())
            jumpAction = true;
    }

    public void FixedUpdate()
    {
        GroundCheck();
        Vector2 input = GetInput();
        bool isGivingMovementInput = input.x != 0 || input.y != 0;
        
        if (isGivingMovementInput && (advancedSettings.airControl || isGrounded))
        {
            // Always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = _camera.transform.forward*input.y + _camera.transform.right*input.x;
            desiredMove = (desiredMove - Vector3.Project(desiredMove, groundContactNormal)).normalized;
            desiredMove *= movementSettings.CurrentTargetSpeed* modifierSettings.speedModifier;
            if (!isGrounded)
                desiredMove *= advancedSettings.airControlSteerMultiplier;
            desiredMove = desiredMove * SlopeMultiplier(desiredMove);

            // Clamp X-Z velocity (without affecting y)
            Vector3 velocityXZ = rb.velocity;
            velocityXZ.y = 0;
            if (velocityXZ.sqrMagnitude > (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                velocityXZ = velocityXZ.normalized * movementSettings.CurrentTargetSpeed;
            velocityXZ.y = rb.velocity.y;
            rb.velocity = velocityXZ;
            //velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            //velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            //velocityChange.y = 0;
            
            // Apply a force that attempts to reach our target velocity
            Vector3 currentVelocity = rigidbody.velocity;
            Vector3 velocityChange = (desiredMove - currentVelocity);
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            
            // Add an impulse force to move the player around
            //desiredMove = desiredMove * SlopeMultiplier(desiredMove);
            //rb.AddForce(desiredMove, ForceMode.Impulse);
        }

        if (nextForce != Vector3.zero)
        {
            rb.drag = 0f;
            //Debug.Log("NEXT FORCE!" + nextForce);
            rb.AddForce(nextForce, nextForceMode);
            nextForce = Vector3.zero;
            jumping = true;
        }

        if (isGrounded)
        {
            rb.drag = 5f;

            if (jumpAction)
            {
                rb.drag = 0f;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(new Vector3(0f, movementSettings.JumpSpeed, 0f), ForceMode.Impulse);
                jumping = true;
            }  
            
            if (!jumping && !isGivingMovementInput)
            {
                if (advancedSettings.zeroInertiaOnStop) rb.velocity = Vector3.zero;

                if (rb.velocity.magnitude < 1f)
                    rb.Sleep();
                else
                {
                    // Slow down if no input is given
                    rb.velocity *= (1 - Time.deltaTime * advancedSettings.slowDownRate);
                }
            }
        }
        else
        {
            rb.drag = 0;

            // Add custom gravity
            rb.AddForce(Physics.gravity * movementSettings.GravityMultiplier, ForceMode.Acceleration);
            //Debug.Log("ADDING FORCE " + (Vector3.up * baseGravity * movementSettings.GravityMultiplier));

            if (previouslyGrounded && !jumping)
                StickToGroundHelper();
        }
        jumpAction = false;


        //if (grounded)
        //    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        //else if (!grounded && !wallCollision)
        //{
        //    rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0)); // We apply gravity manually for more tuning control
        //    rigidbody.AddForce(velocityChange * airControl, ForceMode.VelocityChange); //Use this for Air Control
        //}
        //else if (!grounded && wallCollision)
         //   rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0)); // We apply gravity manually for more tuning control

            /*
        if (Input.GetButtonDown("Jump") && (grounded || delay > 0))
            rigidbody.velocity = new Vector3(currentVelocity.x,jumpSpeed, currentVelocity.z);

        if (!grounded) delay -= 0.1f;

        grounded = false;
        wallCollision = false;*/
    }
    #endregion

    #region Miscellaneous

    /// <summary>
    /// From the jump height and gravity we deduce the upwards speed the character will reach at its apex.
    /// </summary>
    float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * movementSettings.JumpHeight * Physics.gravity.magnitude * movementSettings.GravityMultiplier);
    }


    /// <summary>
    /// Rotate the current view
    /// </summary>
    private void RotateView()
    {
        // Get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;
        Vector2 mouseInput = mouseLook.Clamped(yRotation, transform.localEulerAngles.y);

        // Handle the rotation round the x axis on the camera
        _camera.transform.localEulerAngles = new Vector3(-mouseInput.y, _camera.transform.localEulerAngles.y, _camera.transform.localEulerAngles.z);
        yRotation = mouseInput.y;
        transform.localEulerAngles = new Vector3(0, mouseInput.x, 0);

        if (isGrounded || advancedSettings.airControl)
        {
            // Rotate the rigidbody velocity to match the new direction that the character is looking 
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            rb.velocity = velRotation * rb.velocity;
        }
    }

    /// <summary>
    /// Sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
    /// </summary>
    private void GroundCheck()
    {
        previouslyGrounded = isGrounded;
        RaycastHit hitInfo;
        LayerMask hitMask = 1 << LayerMask.NameToLayer("PickedupObject");
        hitMask = ~hitMask;

        if (Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo,
                                ((capsule.height / 2f) - capsule.radius) + advancedSettings.groundCheckDistance
                                , hitMask))
        {
            isGrounded = true;
            groundContactNormal = hitInfo.normal;
        }
        else
        {
            isGrounded = false;
            groundContactNormal = Vector3.up;
        }
        if (!previouslyGrounded && isGrounded && jumping)
        {
            jumping = false;
        }
    }

    /// <summary>
    /// Can the player jump here?
    /// </summary>
    /// <returns>True if he can</returns>
    private bool CanJump()
    {
        //Debug.Log(this.SlopeMultiplier(Vector3.zero));
        return this.SlopeMultiplier(Vector3.zero) > 0.01f && !this.modifierSettings.disableJump;
    }

    /// <summary>
    /// Compute and get a multiplier for the movement speed based on the current slope
    /// </summary>
    /// <param name="desiredMove">Direction of movement</param>
    /// <returns></returns>
    private float SlopeMultiplier(Vector3 desiredMove)
    {
        Vector3 from = transform.position + desiredMove.normalized;
        Vector3 dir = Vector3.down;
        RaycastHit hit;
        Vector3 targetNormal;
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(from, dir, out hit, 10, layerMask))
            targetNormal = hit.normal;
        else
            targetNormal = groundContactNormal;

        //Debug.DrawLine(from, from+dir * 1, Color.red);
        float angle = Vector3.Angle(targetNormal, Vector3.up);
        float modifier = movementSettings.SlopeCurveModifier.Evaluate(angle);
        //Debug.Log(angle + " : " + modifier + " desired " + desiredMove);
        return modifier;
    }

    /// <summary>
    /// Utility to stick the player to the ground
    /// </summary>
    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo,
                                ((capsule.height / 2f) - capsule.radius) +
                                advancedSettings.stickToGroundHelperDistance))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                rb.velocity = rb.velocity - Vector3.Project(rb.velocity, hitInfo.normal);
            }
        }
    }


#endregion

    #region Buttons & Axes

    /// <summary>
    /// Get the current movement input
    /// </summary>
    /// <returns>Input</returns>
    private Vector2 GetInput()
    {
        bool shouldRun = advancedSettings.allowRunning && GetRunButton();
        movementSettings.UpdateDesiredTargetSpeed(shouldRun);
        Vector2 input = new Vector2
        {
            x = GetHorizontalMovement(),
            y = GetVerticalMovement()
        };
        return input;
    }

    bool GetRunButton()
    {
        return CrossPlatformInputManager.GetButton("Run");
    }

    bool GetJumpButton()
    {
        return CrossPlatformInputManager.GetButtonDown("Jump");
    }

    float GetVerticalMovement()
    {
        return CrossPlatformInputManager.GetAxis("Vertical");
    }

    float GetHorizontalMovement()
    {
        return CrossPlatformInputManager.GetAxis("Horizontal");
    }
    #endregion


    #region Modifiers

    public void SetSpeedModifier(float v)
    {
        this.modifierSettings.speedModifier = v;
    }
    public void ResetSpeedModifier()
    {
        this.modifierSettings.speedModifier = 1;
    }

    public void DisableJump()
    {
        this.modifierSettings.disableJump = true;
    }
    public void ResetDisableJump()
    {
        this.modifierSettings.disableJump = false;
    }
    #endregion

    #region External forces
    Vector3 nextForce = Vector3.zero;
    ForceMode nextForceMode;
    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        this.nextForce = force;
        this.nextForceMode = forceMode;
    }
    #endregion

}
