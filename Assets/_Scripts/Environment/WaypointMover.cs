using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public enum FacingStyle
    {
        LOOK_ALONG_Y_ONLY,
        LOOK_DIRECTLY,
        DONT_LOOK
    }

    public List<Transform> waypoints;

    public float moveSpeed = 1f;

    public float waitTime = 3f;

    public FacingStyle facingStyle = FacingStyle.DONT_LOOK;

    [HideInInspector] public bool stopped;

    [HideInInspector] public Vector3 travelDirection;

    private Vector3 currentTarget;

    private int currentTargetIndex;

    private Vector3 previousTarget;

    private float timeToStartMovingAgain;

    private void Start()
    {
        InitializeInformation();
    }

    private void FixedUpdate()
    {
        ProcessMovementState();
    }

    private void ProcessMovementState()
    {
        if (stopped)
            StartCheck();
        else
            Travel();
    }

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

    private void CalculateTravelInformation()
    {
        travelDirection = (currentTarget - previousTarget).normalized;
    }

    private void Travel()
    {
        transform.Translate(travelDirection * (moveSpeed * Time.deltaTime), Space.World);
        bool overX = false;
        bool overY = false;
        bool overZ = false;

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
        if (overX && overY && overZ) BeginWait();
    }

    private void ChangeRotation()
    {
        if (facingStyle == FacingStyle.DONT_LOOK)
        {
            // Do nothing
        }
        else if (facingStyle == FacingStyle.LOOK_DIRECTLY)
        {
            transform.LookAt(currentTarget);
        }
        else if (facingStyle == FacingStyle.LOOK_ALONG_Y_ONLY)
        {
            var targetPositionForRotation = new Vector3(currentTarget.x, transform.position.y, currentTarget.z);
            transform.LookAt(targetPositionForRotation);
        }
    }

    private void BeginWait()
    {
        stopped = true;
        timeToStartMovingAgain = Time.time + waitTime;
    }
}
