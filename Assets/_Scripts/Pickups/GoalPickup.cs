using Managers;
using UnityEngine;

public class GoalPickup : Pickup
{
    protected override void DoOnPickup(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventManager.RaiseEvent(new PlayerWinEventArgs());
            AudioManager.Instance.PlaySFX(SFXClips.Goal);
        }
        base.DoOnPickup(collision);
    }
}
