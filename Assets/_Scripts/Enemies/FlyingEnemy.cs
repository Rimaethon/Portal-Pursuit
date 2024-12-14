using UnityEngine;

public class FlyingEnemy : Enemy
{
    public enum BehaviorAtStopDistance
    {
        STOP,
        CIRCLE_CLOCKWISE,
        CIRCLE_ANTICLOCKWISE
    }
    public float stopDistance = 5.0f;
    public BehaviorAtStopDistance stopBehavior = BehaviorAtStopDistance.CIRCLE_CLOCKWISE;

    protected override Vector3 CalculateDesiredMovement()
    {
        if ((target - transform.position).magnitude > stopDistance)
            return transform.position + (target - transform.position).normalized * (moveSpeed * Time.deltaTime);
        switch (stopBehavior)
        {
            case BehaviorAtStopDistance.STOP:
                break;
            case BehaviorAtStopDistance.CIRCLE_CLOCKWISE:
                return transform.position + Vector3.Cross(target - transform.position, transform.up).normalized * (moveSpeed * Time.deltaTime);
            case BehaviorAtStopDistance.CIRCLE_ANTICLOCKWISE:
                Transform transform1 = transform;

                return transform1.position - Vector3.Cross(target - transform1.position, transform1.up).normalized * (moveSpeed * Time.deltaTime);
        }

        return base.CalculateDesiredMovement();
    }

    protected override Quaternion CalculateDesiredRotation()
    {
        return Quaternion.LookRotation(target - transform.position, Vector3.up);
    }
}
