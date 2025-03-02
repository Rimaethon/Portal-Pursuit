using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum ActionStates
    {
        IDLE,
        PREPARING,
        ATTACKING
    }

    public enum MovementStates
    {
        IDLE,
        MOVING
    }

    public float moveSpeed = 2.0f;
    public bool canMove = true;
    public EnemyAwareness awareness;
    public EnemyAttacker attacker;
    public float maximumAttackRange = 5.0f;
    public bool doesAttack;
    public bool lineOfSightToAttack = true;
    public MovementStates movementState = MovementStates.IDLE;
    public ActionStates actionState = ActionStates.IDLE;
    protected Rigidbody enemyRigidbody;
    protected bool isActing = false;
    protected bool isPreparing = false;

    public Vector3 target
    {
        get
        {
            if (awareness != null)
                return awareness.followPosition;
            return transform.position;
        }
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        HandleMovement();
        HandleActions();
    }

    protected virtual void Setup()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        if (attacker == null) attacker = GetComponent<EnemyAttacker>();
    }

    protected virtual void HandleMovement()
    {
        if (canMove && enemyRigidbody != null)
        {
            var desiredMovement = CalculateDesiredMovement();
            var desiredRotation = CalculateDesiredRotation();

            enemyRigidbody.linearVelocity = Vector3.zero;
            enemyRigidbody.angularVelocity = Vector3.zero;

            enemyRigidbody.MovePosition(desiredMovement);
            enemyRigidbody.MoveRotation(desiredRotation);
        }
    }

    protected virtual void HandleActions()
    {
        TryToAttack();
    }

    protected virtual void TryToAttack()
    {
        if (doesAttack && attacker != null && target != null &&
            (target - transform.position).magnitude < maximumAttackRange)
            if (!lineOfSightToAttack || (awareness != null && lineOfSightToAttack && awareness.seesPlayer))
                attacker.Attack(target);
    }

    protected virtual Vector3 CalculateDesiredMovement()
    {
        return transform.position;
    }

    protected virtual Quaternion CalculateDesiredRotation()
    {
        return transform.rotation;
    }

    protected void SetState()
    {
        if (isActing)
            actionState = ActionStates.ATTACKING;
        else if (isPreparing)
            actionState = ActionStates.PREPARING;
        else
            actionState = ActionStates.IDLE;
        if (canMove && enemyRigidbody != null && enemyRigidbody.linearVelocity.magnitude > 0.1f)
            movementState = MovementStates.MOVING;
        else
            movementState = MovementStates.IDLE;
    }
}
