using Managers;
using UnityEngine;


public class HealthPickup : Pickup
{
    [SerializeField] int healingAmount = 1;

    protected override void DoOnPickup(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
			HealthPickupEventArgs healthPickupEventArgs = new HealthPickupEventArgs
            {
                healingAmount = healingAmount
            };

            EventManager.RaiseEvent(healthPickupEventArgs);
            AudioManager.Instance.PlaySFX(SFXClips.Health);
        }
        base.DoOnPickup(collision);
    }
}
