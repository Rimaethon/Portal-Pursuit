using UnityEngine;


public class Pickup : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        DoOnPickup(collision);
    }

    protected virtual void DoOnPickup(Collider collision)
    {
        if (!collision.CompareTag("Player")) return;
        Destroy(gameObject);
    }
}
