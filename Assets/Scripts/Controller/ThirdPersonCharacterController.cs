using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Controller
{
    public class ThirdPersonCharacterController : PortalTraveller
    { 
        public enum PlayerState
        {
            Idle,
            Moving,
            Jumping,
            DoubleJumping,
            Falling,
            Dead
        }

        public InputManager inputManager;
        public bool diolougeWorking;

        public PlayerState playerState = PlayerState.Idle;

        public GameObject jumpEffect;

        public GameObject doubleJumpEffect;

        public GameObject landingEffect;

        public float walkSpeed = 3;
        public float runSpeed = 6;
        public float smoothMoveTime = 0.1f;
        public float jumpForce = 8;

        public bool lockCursor;
        public float mouseSensitivity = 10;
        public Vector2 pitchMinMax = new(-40, 85);
        public float rotationSmoothTime = 0.1f;

        [Header("Related Gameobjects / Scripts needed for determining control states")]
        [Tooltip("The player camera gameobject, used to manage the controller's rotations")]
        public GameObject playerCamera;

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
        private CharacterController characterController;
        private CharacterController controller;
        private bool disabled;
        private bool doubleJumpAvailable;
        private bool jumping;
        private bool landed;
        private float lastGroundedTime;
        private Vector3 moveDirection = Vector3.zero;
        private NpcInteractable npcInteractable;
        private List<Transform> npclist;
        private float pitchSmoothV;
        private Health playerHealth;
        private PlayerInteract playerInteract;
        private Vector3 rotationSmoothVelocity;
        private float smoothPitch;
        private Vector3 smoothV;
        private float smoothYaw;
        private float timeToStopBeingLenient;
        private Vector3 velocity;
        private float verticalVelocity;
        private float yawSmoothV;

        private void Start()
        {
            cam = Camera.main;
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            controller = GetComponent<CharacterController>();
            InitialSetup();
        }

        private void LateUpdate()
        {
            if (Time.timeScale == 0) return;
            CentralizedControl(inputManager.HorizontalMoveAxis, inputManager.VerticalMoveAxis, inputManager.JumpPressed,
                inputManager.EPressed);
        }

        private void InitialSetup()
        {
            inputManager = InputManager.instance;
            characterController = gameObject.GetComponent<CharacterController>();
            if (inputManager == null)
                Debug.LogError(
                    "There is no input manager in the scene, and the ThirdPersonCharacterController needs one to run");
            if (characterController == null)
                Debug.LogError(
                    "There is no character controller attached to the same gameobject as the ThirdPersonCharacterController. It needs one to run correctly");
            if (GetComponent<Health>() == null)
                Debug.LogError(
                    "There is no health script attached to the player!\n There needs to be a health script on the same object as the Third Person Character Controller");
            else
                playerHealth = GetComponent<Health>();
        }

        private void CentralizedControl(float leftRightMovementAxis, float forwardBackwardMovementAxis, bool jumpPressed,
            bool ePressed)
        {
            if (playerHealth.currentHealth <= 0)
                DeadControl();
            else
                NormalControl(leftRightMovementAxis, forwardBackwardMovementAxis, jumpPressed, ePressed);
        }

        private void NormalControl(float leftRightMovementAxis, float forwardBackwardMovementAxis, bool jumpPressed,
            bool ePressed)
        {
            var leftRightInput = leftRightMovementAxis;
            var forwardBackwardInput = forwardBackwardMovementAxis;

            if (characterController.isGrounded && !bounced)
            {
                timeToStopBeingLenient = Time.time + jumpTimeLeniency;
                doubleJumpAvailable = true;

                if (!landed && landingEffect != null && moveDirection.y <= -5)
                {
                    landed = true;
                    Instantiate(landingEffect, transform.position, transform.rotation, null);
                }

                moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= moveSpeed;

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
            else
            {
                moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y, forwardBackwardInput * moveSpeed);
                moveDirection = transform.TransformDirection(moveDirection);

                if (jumpPressed && Time.time < timeToStopBeingLenient)
                {
                    moveDirection.y = jumpStrength;
                    if (jumpEffect != null) Instantiate(jumpEffect, transform.position, transform.rotation, null);
                    playerState = PlayerState.Jumping;
                }
                else if (jumpPressed && doubleJumpAvailable)
                {
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

            moveDirection.y -= gravity * Time.deltaTime;

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

            characterController.Move(moveDirection * Time.deltaTime);
        }

        private void DeadControl()
        {
            playerState = PlayerState.Dead;
            moveDirection = new Vector3(0, moveDirection.y, 0);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime);
        }

        public void Bounce(float bounceForceMultiplier, float bounceJumpButtonHeldMultiplyer)
        {
            bounced = true;
            playerState = PlayerState.Jumping;
            if (inputManager.JumpHeld)
                moveDirection.y = jumpStrength * bounceJumpButtonHeldMultiplyer;
            else
                moveDirection.y = jumpStrength * bounceForceMultiplier;
        }

        public void ResetJumps()
        {
            doubleJumpAvailable = true;
        }

        public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
        {
            transform.position = pos;
            var eulerRot = rot.eulerAngles;
            var delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
            smoothYaw += delta;
            transform.eulerAngles = Vector3.up * smoothYaw;
            velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
            Physics.SyncTransforms();
        }
    }
}