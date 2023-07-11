using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class handles the control of a third person character controller.
///     Motion is based on the direction the player(user) expects to move
///     i.e. left and right are based on the camera view and not the direction the player's character is facing
/// </summary>
public class ThirdPersonCharacterController : PortalTraveller
{
    // Enum to handle the player's state
    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        DoubleJumping,
        Falling,
        Dead
    }

    // The input manager to read input from
    public InputManager inputManager;
    public bool diolougeWorking;

    [Header("State Information")] [Tooltip("The state the player controller is currently in")]
    public PlayerState playerState = PlayerState.Idle;

    [Header("Effects settings")] [Tooltip("The effect to create when jumping")]
    public GameObject jumpEffect;

    [Tooltip("The effect to create when double jumping")]
    public GameObject doubleJumpEffect;

    [Tooltip("The effec to create when the player lands on the ground")]
    public GameObject landingEffect;

    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new(-40, 85);
    public float rotationSmoothTime = 0.1f;
    public float yaw;
    public float pitch;

    [Header("Related Gameobjects / Scripts needed for determining control states")]
    [Tooltip("The player camera gameobject, used to manage the controller's rotations")]
    public GameObject playerCamera;

    [Tooltip("The player character's representation (model)")]
    public PlayerRepresentation playerRepresentation;

    [Header("Speed Control")] [Tooltip("The speed at which to move the player")]
    public float moveSpeed = 5f;

    [Tooltip("The strength with which to jump")]
    public float jumpStrength = 8.0f;

    [Tooltip("The strength of gravity on this controller")]
    public float gravity = 20.0f;

    [Header("Jump Timing")] [Tooltip("How long to be lenient for when the player becomes ungrounded")]
    public float jumpTimeLeniency = 0.1f;


    private bool bounced;

    private Camera cam;

    // the character controller used for player motion
    private CharacterController characterController;

    private CharacterController controller;
    private Vector3 currentRotation;
    private bool disabled;

    // Whether or not the double jump is currently available
    private bool doubleJumpAvailable;

    private bool jumping;

    private bool landed;
    private float lastGroundedTime;

    // The direction the player is moving in
    private Vector3 moveDirection = Vector3.zero;
    private NpcInteractable npcInteractable;
    private List<Transform> npclist;

    private float pitchSmoothV;

    // the health used by the player
    private Health playerHealth;
    private PlayerInteract PlayerInteract;
    private Vector3 rotationSmoothVelocity;
    private float smoothPitch;
    private Vector3 smoothV;

    private float smoothYaw;

    // When to stop being lenient
    private float timeToStopBeingLenient;
    private Vector3 velocity;
    private float verticalVelocity;

    private float yawSmoothV;

    /// <summary>
    ///     Description:
    ///     Standard Unity function that is called before the first frame
    ///     Input:
    ///     none
    ///     Return:
    ///     void
    /// </summary>
    private void Start()
    {
        cam = Camera.main;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController>();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
        InitialSetup();
    }

    /// <summary>
    ///     Description:
    ///     Standard Unity function called once per frame
    ///     Inputs:
    ///     none
    ///     Retuns:
    ///     void
    /// </summary>
    private void LateUpdate()
    {
        // Don't do anything if the game is paused
        if (Time.timeScale == 0) return;
        MatchCameraYRotation();
        CentralizedControl(inputManager.horizontalMoveAxis, inputManager.verticalMoveAxis, inputManager.jumpPressed,
            inputManager.ePressed);
    }

    /// <summary>
    ///     Description:
    ///     Checks for and gets the needed refrences for this script to run correctly
    ///     Input:
    ///     none
    ///     Return:
    ///     void
    /// </summary>
    private void InitialSetup()
    {
        inputManager = InputManager.instance;
        characterController = gameObject.GetComponent<CharacterController>();
        if (inputManager == null)
            Debug.LogError(
                "There is no input manager in the scene and the ThirdPersonCharacterController needs one to run");
        if (characterController == null)
            Debug.LogError(
                "There is no character controller attached to the same gameobject as the ThirdPersonCharacterController. + " +
                "\n It needs one to run correctly");
        if (GetComponent<Health>() == null)
            Debug.LogError("There is no health script attached to the player!\n" +
                           "There needs to be a health script on the same" +
                           "object as the Third Person Character Controller");
        else
            playerHealth = GetComponent<Health>();
    }

    /// <summary>
    ///     Description:
    ///     Makes the character controller's rotation along the Y match the camera's
    ///     Input:
    ///     none
    ///     Return:
    ///     void
    /// </summary>
    private void MatchCameraYRotation()
    {
        if (playerCamera == null)
            Debug.LogError("The ThirdPersonCharacterController is missing a player camera gameobject reference. + " +
                           "\n It needs one to orient itself correctly");
        gameObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            playerCamera.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    /// <summary>
    ///     Description:
    ///     Handles swithing between control styles if more than one is coded in
    ///     Inputs:
    ///     float leftCharacterMovement | float rightCharacterMovement | bool jumpPressed
    ///     Returns:
    ///     void
    /// </summary>
    /// <param name="leftCharacterMovement">The movement input along the horizontal</param>
    /// <param name="rightCharacterMovement">The movement input along the vertical</param>
    /// <param name="jumpPressed">"Whether or not the jump input has been pressed"</param>
    private void CentralizedControl(float leftRightMovementAxis, float forwardBackwardMovementAxis, bool jumpPressed,
        bool ePressed)
    {
        if (playerHealth.currentHealth <= 0)
            DeadControl();
        else
            NormalControl(leftRightMovementAxis, forwardBackwardMovementAxis, jumpPressed, ePressed);
    }

    /// <summary>
    ///     Description:
    ///     Handles motion of the player representation under the average or normal use case
    ///     Inputs:
    ///     float leftCharacterMovement | float rightCharacterMovement | bool jumpPressed
    ///     Returns:
    ///     void
    /// </summary>
    /// <param name="leftCharacterMovement">The movement input along the horizontal</param>
    /// <param name="rightCharacterMovement">The movement input along the vertical</param>
    /// <param name="jumpPressed">"Wheter or not the jump input has been pressed"</param>
    private void NormalControl(float leftRightMovementAxis, float forwardBackwardMovementAxis, bool jumpPressed,
        bool ePressed)
    {
        // The input corresponding to the left and right movement
        var leftRightInput = leftRightMovementAxis;
        // The input corresponding to the forward and backward movement
        var forwardBackwardInput = forwardBackwardMovementAxis;

        // If the controller is grounded
        if (characterController.isGrounded && !bounced)
        {
            // Reset the time to stop being lenient
            timeToStopBeingLenient = Time.time + jumpTimeLeniency;

            // make the double jump available again
            doubleJumpAvailable = true;

            if (!landed && landingEffect != null && moveDirection.y <= -5)
            {
                landed = true;
                Instantiate(landingEffect, transform.position, transform.rotation, null);
            }

            // set the move direction based on the input
            moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput);

            // transform the movement direction to be in world space (because we want to move in relation to the world not ourselves)
            moveDirection = transform.TransformDirection(moveDirection);

            // Apply the movement speed to the movement direction
            moveDirection *= moveSpeed;

            // If the player has pressed the jump button, apply to the y movement the jump strength
            if (jumpPressed)
            {
                moveDirection.y = jumpStrength;
                if (jumpEffect != null) Instantiate(jumpEffect, transform.position, transform.rotation, null);
                playerState = PlayerState.Jumping;
            }
            else if (moveDirection == Vector3.zero)
            {
                playerState = PlayerState.Idle;
            }
            else
            {
                playerState = PlayerState.Moving;
            }
        }
        // When we are not grounded...
        else
        {
            // Apply move direction with the input and move speed to the x and z
            // Apply the previous move direction y to this current move direction y
            moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y, forwardBackwardInput * moveSpeed);
            // transform the movement direction to be in world space (because we want to move in relation to the world not ourselves)
            moveDirection = transform.TransformDirection(moveDirection);

            // If the jump is pressed and we are still being lenient apply the jump
            if (jumpPressed && Time.time < timeToStopBeingLenient)
            {
                moveDirection.y = jumpStrength;
                if (jumpEffect != null) Instantiate(jumpEffect, transform.position, transform.rotation, null);
                playerState = PlayerState.Jumping;
            }
            // otherwise, check for the double jump..
            else if (jumpPressed && doubleJumpAvailable)
            {
                // Apply the double jump and make it unavailable
                moveDirection.y = jumpStrength;
                doubleJumpAvailable = false;
                if (doubleJumpEffect != null)
                    Instantiate(doubleJumpEffect, transform.position, transform.rotation, null);
                playerState = PlayerState.DoubleJumping;
            }
            else if (moveDirection.y < -0.5f && playerState != PlayerState.Idle)
            {
                bounced = false;
                landed = false;
                playerState = PlayerState.Falling;
            }
        }

        // Apply gravity with Time.deltaTime (effectively applied again to make it an acceleration)
        moveDirection.y -= gravity * Time.deltaTime;

        // If we are grounded and the y movedirection is negative reset it to something small
        // This avoids building up a large negative motion along the y direction
        if (characterController.isGrounded && moveDirection.y < 0) moveDirection.y = -0.3f;


        var interactRange = 2f;
        var colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var collider in colliderArray)
            if (collider.TryGetComponent(out NpcInteractable npcInteractable))
            {
                if (!ePressed)
                {
                    StartCoroutine(npcInteractable.InteractInfo());
                }
                else if (ePressed)
                {
                    StopCoroutine(npcInteractable.InteractInfo());
                    StartCoroutine(npcInteractable.Diolouge());
                }
            }

        // Pass the calculated move direction multipplied by the time inbetween freames to the charater controller move function
        characterController.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    ///     Description:
    ///     Bounces the player upwards with some multiplier by the jump strength
    ///     Input:
    ///     float bounceForceMultiplier | float bounceJumpButtonHeldMultiplyer
    ///     Output:
    ///     void
    /// </summary>
    /// <param name="bounceForceMultiplier">The force to multiply jump strength by when bounce is called</param>
    /// <param name="bounceJumpButtonHeldMultiplyer">
    ///     The force to multiply jump strength by when bounce is called and the jump
    ///     button is held down
    /// </param>
    public void Bounce(float bounceForceMultiplier, float bounceJumpButtonHeldMultiplyer)
    {
        bounced = true;
        playerState = PlayerState.Jumping;
        if (inputManager.jumpHeld)
            moveDirection.y = jumpStrength * bounceJumpButtonHeldMultiplyer;
        else
            moveDirection.y = jumpStrength * bounceForceMultiplier;
    }

    /// <summary>
    ///     Description:
    ///     Control when the player is dead
    ///     Input:
    ///     none
    ///     Return:
    ///     void
    /// </summary>
    private void DeadControl()
    {
        playerState = PlayerState.Dead;
        moveDirection = new Vector3(0, moveDirection.y, 0);
        moveDirection = transform.TransformDirection(moveDirection);

        moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    ///     Description:
    ///     Resets the double jump of the player
    ///     Input:
    ///     None
    ///     Return:
    ///     void (no return)
    /// </summary>
    public void ResetJumps()
    {
        doubleJumpAvailable = true;
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        var eulerRot = rot.eulerAngles;
        var delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
        Physics.SyncTransforms();
    }
}