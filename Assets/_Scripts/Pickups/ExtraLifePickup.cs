using Managers;
using UnityEngine;

public class ExtraLifePickup : Pickup
{
    protected override void DoOnPickup(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
           EventManager.RaiseEvent(new LifePickupEventArgs());
           AudioManager.Instance.PlaySFX(SFXClips.Life);
        }
        base.DoOnPickup(collision);
    }
}
