using System;
using UnityEngine;

public sealed class Door : MonoBehaviour
{
    [SerializeField] private int doorID;
    private bool isOpen;
    private Animator animator;
    private static readonly int doorOpenTrigger = Animator.StringToHash("Open");
    private static readonly int doorCloseTrigger = Animator.StringToHash("Close");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
            AttemptToOpen();
    }

    private void AttemptToOpen()
    {
        if(isOpen)
            return;

        if(CheckPlayerHasKey())
            Open();
        else
            AudioManager.Instance.PlaySFX(SFXClips.LockedDoor);
    }

    private bool CheckPlayerHasKey()
    {
        return KeyRing.HasKey(doorID);
    }

    private void Open()
    {
        isOpen = true;
        animator.SetTrigger(doorOpenTrigger);
        AudioManager.Instance.PlaySFX(SFXClips.DoorOpenClose);
    }

    private void Close()
    {
        isOpen = false;
        animator.SetTrigger(doorCloseTrigger);
        AudioManager.Instance.PlaySFX(SFXClips.DoorOpenClose);
    }
}
