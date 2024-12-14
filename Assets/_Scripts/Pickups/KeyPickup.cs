using UnityEngine;

public class KeyPickup : Pickup
{
    [SerializeField] int keyID;

    protected override void DoOnPickup(Collider collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        KeyRing.AddKey(keyID);
        AudioManager.Instance.PlaySFX(SFXClips.KeyCollect);
        base.DoOnPickup(collision);
    }
}
