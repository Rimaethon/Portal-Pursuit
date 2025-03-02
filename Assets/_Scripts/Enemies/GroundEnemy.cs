using UnityEngine;
using UnityEngine.AI;

public class GroundEnemy : Enemy
{
    public NavMeshAgent agent;
    public float stopDistance = 2.0f;
    public bool lineOfSightToStop = true;
    public bool alwaysFacePlayer = true;

    protected override void Setup()
    {
        base.Setup();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.updateRotation = !alwaysFacePlayer;
    }

    protected override void HandleMovement()
    {
        if (enemyRigidbody != null)
        {
            enemyRigidbody.linearVelocity = Vector3.zero;
            enemyRigidbody.angularVelocity = Vector3.zero;
        }

        Quaternion desiredRotation = CalculateDesiredRotation();
        transform.rotation = desiredRotation;

        bool ShouldMove()
        {
            if (agent == null || target == null || !canMove) return false;

            if ((target - transform.position).magnitude > stopDistance)
                return true;
            return awareness != null && lineOfSightToStop && !awareness.CheckLineOfSight();

        }

        if (ShouldMove())
            agent.SetDestination(target);
        else if (agent != null) agent.SetDestination(transform.position);
    }

    protected override Vector3 CalculateDesiredMovement()
    {
        if (agent != null) return agent.desiredVelocity * Time.deltaTime;
        return base.CalculateDesiredMovement();
    }

    protected override Quaternion CalculateDesiredRotation()
    {
        if (!alwaysFacePlayer) return base.CalculateDesiredRotation();

        if (awareness == null || !(awareness.certaintyOfPlayer > awareness.followThreshold)) return transform.rotation;

        Vector3 toTarget = target - transform.position;
        toTarget.y = 0;
        Quaternion result = Quaternion.LookRotation(toTarget, transform.up);
        return result;

    }
}
