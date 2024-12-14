using UnityEngine;


public class PlayerChilder : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        MakeAChildOfAttachedTransform(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) other.transform.parent = null;
    }

    private void OnTriggerStay(Collider collision)
    {
        MakeAChildOfAttachedTransform(collision);
    }

    private void MakeAChildOfAttachedTransform(Collider collision)
    {
        if (collision.transform.CompareTag("Player")) collision.transform.parent = transform;
    }
}
