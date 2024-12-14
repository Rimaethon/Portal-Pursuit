using UnityEngine;


public class JumpPad : MonoBehaviour
{
    private Animator jumpPadAnimator;
    private static readonly int bounce = Animator.StringToHash("Bounce");

    private void Awake()
    {
        jumpPadAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")) BouncePlayer();
    }
    private void BouncePlayer()
    {
        AudioManager.Instance.PlaySFX(SFXClips.JumpPad);
        if (jumpPadAnimator != null) jumpPadAnimator.SetTrigger(bounce);
    }
}
