using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public InputManager inputManager;

    [Header("Needed References")]
    // the rotation reference rotated by this script to get the camera functionality desired
    private Transform rotationReference;
    RadioInteractor radio;
    Portal portal;
    int waitForFrames = 3;
    int framesWaited = 0;
    public float cameraSpeed = 180f;
    public Transform playerBody;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void LateUpdate()
    {
        if (framesWaited <= waitForFrames)
        {
            framesWaited += 1;
            return;
        }

        // Do nothing if paused
        if (Time.timeScale == 0)
        {
            return;
        }

        FpCameraInput(inputManager.horizontalLookAxis, inputManager.verticalLookAxis);
    }

  

    [Header("Inversion Control Settings")]
    [Tooltip("Whether or not to invert the horizontal look")]
    public bool invertedHorizontal = false;
    [Tooltip("Whether or not to invert the vertical look")]
    public bool invertedVertical = false;

    [Header("Camera Vertical Rotation Caps (in degrees)")]
    [Tooltip("The maximum X rotation to rotate to in degrees")]
    [Range(70, 125)]
    public float maximumXRotation = 125f;
    [Tooltip("The minimum X rotation to rotate to in degrees")]
    [Range(-10, -85)]
    public float minimumXRotation = -85f;


    void FpCameraInput(float horizontalLook, float verticalLook)
    {
        // Camera input
        float horizontalInput = horizontalLook;
        float verticalInput = verticalLook;

        // Camera movement inversion
        if (invertedHorizontal)
        {
            horizontalInput = -horizontalInput;
        }
        if (invertedVertical)
        {
            verticalInput = -verticalInput;
        }

        // How much to adjust the rotation horizontally
        float horizontalRotationAdjustment = horizontalInput * cameraSpeed * Time.deltaTime;
        // How much to adjust the rotation vertically
        float verticalRotationAdjustment = verticalInput * cameraSpeed * Time.deltaTime;

        playerBody.Rotate(Vector3.up * horizontalRotationAdjustment);
        playerBody.Rotate(Vector3.right * verticalRotationAdjustment);
        // The eular angles of the rotation we are changing to
        Vector3 rotationToChangeToInEular = rotationReference.rotation.eulerAngles;
    }
}