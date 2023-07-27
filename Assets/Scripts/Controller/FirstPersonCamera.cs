using UnityEngine;
using Utility;

public class FirstPersonCamera : MonoBehaviour
{
    public InputManager inputManager;
    public float cameraSpeed = 180f;
    public Transform playerBody;


    [Header("Inversion Control Settings")] [Tooltip("Whether or not to invert the horizontal look")]
    public bool invertedHorizontal;

    [Tooltip("Whether or not to invert the vertical look")]
    public bool invertedVertical;

    [Header("Camera Vertical Rotation Caps (in degrees)")]
    [Tooltip("The maximum X rotation to rotate to in degrees")]
    [Range(70, 125)]
    public float maximumXRotation = 125f;

    [Tooltip("The minimum X rotation to rotate to in degrees")] [Range(-10, -85)]
    public float minimumXRotation = -85f;

    private readonly int waitForFrames = 3;

    private int framesWaited;
    private Portal portal;
    private RadioInteractor radio;

    [Header("Needed References")]
    // the rotation reference rotated by this script to get the camera functionality desired
    private Transform rotationReference;
    // Start is called before the first frame update


    // Update is called once per frame
    private void LateUpdate()
    {
        if (framesWaited <= waitForFrames)
        {
            framesWaited += 1;
            return;
        }

        // Do nothing if paused
        if (Time.timeScale == 0) return;

        FpCameraInput(inputManager.HorizontalLookAxis, inputManager.VerticalLookAxis);
    }


    private void FpCameraInput(float horizontalLook, float verticalLook)
    {
        // Camera input
        var horizontalInput = horizontalLook;
        var verticalInput = verticalLook;

        // Camera movement inversion
        if (invertedHorizontal) horizontalInput = -horizontalInput;
        if (invertedVertical) verticalInput = -verticalInput;

        // How much to adjust the rotation horizontally
        var horizontalRotationAdjustment = horizontalInput * cameraSpeed * Time.deltaTime;
        // How much to adjust the rotation vertically
        var verticalRotationAdjustment = verticalInput * cameraSpeed * Time.deltaTime;

        playerBody.Rotate(Vector3.up * horizontalRotationAdjustment);
        playerBody.Rotate(Vector3.right * verticalRotationAdjustment);
        // The eular angles of the rotation we are changing to
        var rotationToChangeToInEular = rotationReference.rotation.eulerAngles;
    }
}