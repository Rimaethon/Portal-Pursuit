using UnityEngine;


public class EnemyAwareness : MonoBehaviour
{
    public bool seesPlayer => certaintyOfPlayer > detectionThreshold && CheckLineOfSight();
    public Transform target;
    public Vector3 expectedPosition = Vector3.zero;
    public float sightAngleRadius = 90.0f;
    public float sightDistance = 20.0f;
    public float hearingDistance = 10.0f;
    public float hearingSpeedThreshold = 5.0f;
    [Range(0.0f, 1.0f)]
    public float certaintyOfPlayer;
    public float followThreshold = 0.2f;
    public float detectionThreshold = 0.5f;
    public float awarenessDecayRate = 2.0f;
    public Transform returnPositionTransform;
    private Vector3 lastHeardPosition = Vector3.zero;
    private float lastHeardTime = Mathf.NegativeInfinity;

    public Vector3 followPosition
    {
        get
        {
            if (certaintyOfPlayer > followThreshold) return expectedPosition;

            return returnPositionTransform != null ? returnPositionTransform.position : transform.position;
        }
    }

    private void Start()
    {
       target = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        float visionCertainty = GetVisionCertainty();
        float hearingCertainty = GetHearingCertainty();
        float totalCertainty = visionCertainty + hearingCertainty;
        if (totalCertainty == 0) totalCertainty = -awarenessDecayRate * Time.deltaTime;

        AddCertainty(totalCertainty);
        if (certaintyOfPlayer > followThreshold) expectedPosition = target.position;
    }

    private void AddCertainty(float amount)
    {
        certaintyOfPlayer = Mathf.Clamp(certaintyOfPlayer + amount, 0, 1);
    }

    private float GetVisionCertainty()
    {
        if (CheckLineOfSight() && CheckVisionAngle())
            return GetDistanceToTarget() / sightDistance * Time.deltaTime;
        return 0;
    }

    public bool CheckLineOfSight()
    {
        Ray ray = new Ray(transform.position, target.position - transform.position);
        RaycastHit hit = new RaycastHit();
        if (!Physics.Raycast(ray.origin, ray.direction, out hit, sightDistance)) return true;
        return hit.transform.IsChildOf(target);

    }

    private float GetHearingCertainty()
    {
        if (!(GetDistanceToTarget() < hearingDistance)) return 0;

        float speed = Mathf.Abs((target.position - lastHeardPosition).magnitude *
                                Mathf.Pow(lastHeardTime - Time.timeSinceLevelLoad, -1.0f));
        float obstructionModifier = 1.0f;
        if (!CheckLineOfSight()) obstructionModifier = 0.25f;
        lastHeardTime = Time.timeSinceLevelLoad;
        lastHeardPosition = target.position;
        if (speed > hearingSpeedThreshold)
            return speed * obstructionModifier * Mathf.Pow(hearingDistance / GetDistanceToTarget(), 1) *
                   Time.deltaTime;
        return 0;

    }

    private bool CheckVisionAngle()
    {
        float angle = Vector3.Angle(transform.forward, target.position - transform.position);
        return angle < sightAngleRadius;
    }

    private float GetDistanceToTarget()
    {
        return (target.position - transform.position).magnitude;
    }
}
