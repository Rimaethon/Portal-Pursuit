using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField]
    private Vector3 _forceDirection = Vector3.zero;
    public float forceMagnitude = 1.0f;
    public ForceMode forceMode = ForceMode.Force;

    public Vector3 forceDirection
    {
        get => _forceDirection.normalized;
        set => _forceDirection = value;
    }

    private void OnCollisionStay(Collision collision)
    {
        ApplyForceToGameObject(collision.gameObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ApplyForceToGameObject(hit.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + GetTotalForce());
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyForceToGameObject(other.gameObject);
    }

    private void ApplyForceToGameObject(GameObject target)
    {
        Vector3 totalForce = GetTotalForce();
        Rigidbody rigidbody = target.GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.AddForce(totalForce, forceMode);
    }

    private Vector3 GetTotalForce()
    {
        return forceDirection * forceMagnitude;
    }
}
