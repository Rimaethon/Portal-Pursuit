using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private int teamId;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private bool destroyAfterDamage = true;
    [SerializeField] private bool dealDamageOnCollisionStay;
    [SerializeField] private bool dealDamageOnCollision;
    [SerializeField] private bool dealDamageOnTriggerStay;
    [SerializeField] private bool dealDamageOnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (dealDamageOnTrigger) DealDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (dealDamageOnCollision) DealDamage(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (dealDamageOnCollisionStay) DealDamage(collision);
    }

    private void DealDamage(Collision collision)
    {
        collision.gameObject.TryGetComponent(out IDamageable damageable);
        if (damageable == null) return;

        if (damageable.TeamId == teamId) return;

        damageable.TakeDamage(damageAmount);
        if (destroyAfterDamage)
            Destroy(gameObject);
    }

    private void DealDamage(Collider other)
    {
        other.gameObject.TryGetComponent(out IDamageable damageable);
        if (damageable == null) return;

        if (damageable.TeamId == teamId) return;

        damageable.TakeDamage(damageAmount);
        if (destroyAfterDamage)
            Destroy(gameObject);
    }
}
