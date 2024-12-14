using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    [SerializeField] Transform respawnLocation;
    Animator checkpointAnimator;
    private static readonly int isActive = Animator.StringToHash("isActive");

    private void Awake()
    {
        checkpointAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (CheckpointTracker.currentCheckpoint == this) return;

        if (!collision.CompareTag("Player") || collision.gameObject.GetComponent<Health>() == null) return;
        Health playerHealth = collision.gameObject.GetComponent<Health>();
        playerHealth.SetRespawnPoint(respawnLocation.position);

        if (CheckpointTracker.currentCheckpoint != null)
            CheckpointTracker.currentCheckpoint.checkpointAnimator.SetBool(isActive, false);

        AudioManager.Instance.PlaySFX(SFXClips.Checkpoint);
        CheckpointTracker.currentCheckpoint = this;
        checkpointAnimator.SetBool(isActive, true);

    }
}
