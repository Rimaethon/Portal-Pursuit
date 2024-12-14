using Controller;
using Managers;
using UnityEngine;
using Utility;

public class CharacterLocomotion : PortalTraveller
{
    public PlayerState playerState = PlayerState.IDLE;
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float smoothMoveTime = 0.1f;
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float gravity = 18;
    [SerializeField] private float mouseSensitivity = 10;
    [SerializeField] private Vector2 pitchMinMax = new Vector2 (-40, 85);
    [SerializeField] private Camera cam;
    private CharacterController controller;
    private float yaw;
    private float pitch;
    private float verticalVelocity;
    private Vector3 velocity;
    private Vector3 smoothV;
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;
    private bool isJumping;
    private bool isDoubleJumping;
    private float lastGroundedTime;
    private bool disabled;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<CharacterController> ();
        yaw = 0;
        pitch = cam.transform.localEulerAngles.x;
    }

    private void OnEnable()
    {
        EventManager.Subscribe<JumpEventArgs>(HandleJump);
    }

    private void OnDisable()
    {
        EventManager.UnSubscribe<JumpEventArgs>(HandleJump);
    }

    private void HandleJump(JumpEventArgs data)
    {
        float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
        if (controller.isGrounded || (!isJumping && timeSinceLastTouchedGround < 0.15f))
        {
            isJumping = true;
            verticalVelocity = jumpForce;
            playerState = PlayerState.JUMPING;
        }else if (!isDoubleJumping && !controller.isGrounded&&isJumping) {
            isDoubleJumping = true;
            verticalVelocity = jumpForce*1.3f;
            playerState = PlayerState.DOUBLE_JUMPING;
        }
    }

    void Update ()
    {
        if (disabled) {
            return;
        }
        HandleMovement(InputManager.Instance.MoveInput);
        HandleLook(InputManager.Instance.LookInput);
    }

    private void HandleMovement(Vector2 movementInput)
    {
        Vector3 inputDir = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir) * walkSpeed;
        velocity = Vector3.SmoothDamp(velocity, worldInputDir, ref smoothV, smoothMoveTime);
        verticalVelocity -= gravity * Time.deltaTime;
        velocity.y = verticalVelocity;
        CollisionFlags flags = controller.Move(velocity * Time.deltaTime);

        if (flags == CollisionFlags.Below)
        {
            isJumping = false;
            isDoubleJumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
            playerState = inputDir == Vector3.zero ? PlayerState.IDLE : PlayerState.MOVING;
        }
        else if (verticalVelocity < -0.5f)
        {
            playerState = PlayerState.FALLING;
        }
    }

    private void HandleLook(Vector2 lookInput)
    {
        lookInput*=mouseSensitivity;
        yaw += lookInput.x;
        pitch -= lookInput.y;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        cam.transform.localEulerAngles = Vector3.right * pitch;
        transform.eulerAngles = Vector3.up * yaw;
    }

    public void HandleBounce(float bounceForce)
    {
        isJumping = false;
        verticalVelocity = bounceForce;
        playerState = PlayerState.JUMPING;

    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle (yaw, eulerRot.y);
        yaw += delta;
        transform.eulerAngles = Vector3.up * yaw;
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
        Physics.SyncTransforms ();
    }
}
