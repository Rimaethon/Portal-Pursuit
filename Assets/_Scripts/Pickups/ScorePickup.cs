using Managers;
using UnityEngine;

public class ScorePickup : Pickup
{
    [SerializeField] private int scoreAmount = 1;
    private readonly ScorePickupEventArgs scorePickupEventArgs = new ScorePickupEventArgs();

    protected override void DoOnPickup(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            scorePickupEventArgs.score = scoreAmount;
            EventManager.RaiseEvent(scorePickupEventArgs);
            AudioManager.Instance.PlaySFX(SFXClips.Score);
        }
        base.DoOnPickup(collision);
    }
}
