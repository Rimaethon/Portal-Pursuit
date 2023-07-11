using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class handles moving the attached game object between waypoints
/// </summary>
public class WaypointMover : MonoBehaviour
{
    // Enum which describes the ways the mover can look at its target.
    public enum FacingStyle
    {
        LookAlongYOnly,
        LookDirectly,
        DontLook
    }

    [Header("Settings")] [Tooltip("A list of transforms to move between")]
    public List<Transform> waypoints;

    [Tooltip("How fast to move the platform")]
    public float moveSpeed = 1f;

    [Tooltip("How long to wait when arriving at a waypoint")]
    public float waitTime = 3f;

    [Tooltip("The style of facing the waypoint mover uses \n" +
             "LookAlongYOnly: Will rotate along the Y axis only \n" +
             "LookDirectly: Will look directely at the target being moved towards \n" +
             "DontLook: Will not change rotation at all")]
    public FacingStyle facingStyle = FacingStyle.DontLook;

    // Whether or not the waypoint mover is stopped
    [HideInInspector] public bool stopped;

    // The current direction being travelled in
    [HideInInspector] public Vector3 travelDirection;

    // The current waypoint being moved to
    private Vector3 currentTarget;

    // The index of the current Target ub tge waypoints list
    private int currentTargetIndex;

    // The previous waypoint or the starting position
    private Vector3 previousTarget;

    // The time at which movement is resumed
    private float timeToStartMovingAgain;

    // Start is called before the first frame update
    private void Start()
    {
        InitializeInformation();
    }

    /// <summary>
    ///     Description:
    ///     Standard Unity function called every fixed frame update
    ///     Input:
    ///     None
    ///     Return:
    ///     void
    /// </summary>
    private void FixedUpdate()
    {
        ProcessMovementState();
    }

    /// <summary>
    ///     Description:
    ///     Processes current state and does movement accordingly
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void ProcessMovementState()
    {
        if (stopped)
            StartCheck();
        else
            Travel();
    }


    /// <summary>
    ///     Description:
    ///     Checks to see if the waypoint mover can start movement again
    ///     Input:
    ///     none:
    ///     returnL
    ///     void (no return)
    /// </summary>
    private void StartCheck()
    {
        if (Time.time >= timeToStartMovingAgain)
        {
            stopped = false;
            previousTarget = currentTarget;
            currentTargetIndex += 1;
            if (currentTargetIndex >= waypoints.Count) currentTargetIndex = 0;
            currentTarget = waypoints[currentTargetIndex].position;
            CalculateTravelInformation();
        }
    }

    /// <summary>
    ///     Description:
    ///     Sets up the first previous target and current target
    ///     then calls CalculateTravelInformation to initilize travel direction
    ///     Inuputs:
    ///     none
    ///     Returns:
    ///     void (no return)
    /// </summary>
    private void InitializeInformation()
    {
        previousTarget = transform.position;
        currentTargetIndex = 0;
        if (waypoints.Count > 0)
        {
            currentTarget = waypoints[0].position;
        }
        else
        {
            waypoints.Add(transform);
            currentTarget = previousTarget;
        }

        CalculateTravelInformation();
    }

    /// <summary>
    ///     Description:
    ///     Calculates the current traveling direction using the previousTarget and the currentTarget
    ///     Inuputs:
    ///     none
    ///     Returns:
    ///     void (no return)
    /// </summary>
    private void CalculateTravelInformation()
    {
        travelDirection = (currentTarget - previousTarget).normalized;
    }

    /// <summary>
    ///     Description:
    ///     Translates the transform in the direction towards the next waypoint
    ///     Inputs:
    ///     none
    ///     Returns:
    ///     void
    /// </summary>
    private void Travel()
    {
        transform.Translate(travelDirection * moveSpeed * Time.deltaTime, Space.World);
        var overX = false;
        var overY = false;
        var overZ = false;

        var directionFromCurrentPositionToTarget = currentTarget - transform.position;

        if (directionFromCurrentPositionToTarget.x == 0 ||
            Mathf.Sign(directionFromCurrentPositionToTarget.x) != Mathf.Sign(travelDirection.x))
        {
            overX = true;
            transform.position = new Vector3(currentTarget.x, transform.position.y, transform.position.z);
        }

        if (directionFromCurrentPositionToTarget.y == 0 ||
            Mathf.Sign(directionFromCurrentPositionToTarget.y) != Mathf.Sign(travelDirection.y))
        {
            overY = true;
            transform.position = new Vector3(transform.position.x, currentTarget.y, transform.position.z);
        }

        if (directionFromCurrentPositionToTarget.z == 0 ||
            Mathf.Sign(directionFromCurrentPositionToTarget.z) != Mathf.Sign(travelDirection.z))
        {
            overZ = true;
            transform.position = new Vector3(transform.position.x, transform.position.y, currentTarget.z);
        }

        ChangeRotation();

        // If we are over the x, y, and z of our target we need to stop
        if (overX && overY && overZ) BeginWait();
    }

    private void ChangeRotation()
    {
        if (facingStyle == FacingStyle.DontLook)
        {
            // Do nothing
        }
        else if (facingStyle == FacingStyle.LookDirectly)
        {
            transform.LookAt(currentTarget);
        }
        else if (facingStyle == FacingStyle.LookAlongYOnly)
        {
            var targetPositionForRotation = new Vector3(currentTarget.x, transform.position.y, currentTarget.z);
            transform.LookAt(targetPositionForRotation);
        }
    }

    /// <summary>
    ///     Description:
    ///     Starts the waiting, setsup the needed variables for waiting
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void BeginWait()
    {
        stopped = true;
        timeToStartMovingAgain = Time.time + waitTime;
    }
}