using System.Collections.Generic;
using Controller;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private CharacterLocomotion playerCharacterController;
    [SerializeField] private Animator representationAnimator;
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 offset;
    private Animator cloneAnimator;

    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int jumping = Animator.StringToHash("Jumping");
    private static readonly int falling = Animator.StringToHash("Falling");
    private static readonly int running = Animator.StringToHash("Running");
    private static readonly int dead = Animator.StringToHash("Dead");
    private PlayerState oldState=PlayerState.IDLE;

    private readonly Dictionary<PlayerState,int> playerState = new Dictionary<PlayerState, int>
    {
        {PlayerState.IDLE,idle},
        {PlayerState.JUMPING,jumping},
        {PlayerState.FALLING,falling},
        {PlayerState.DOUBLE_JUMPING,jumping},
        {PlayerState.MOVING,running},
        {PlayerState.DEAD,dead}
    };
    private bool isAnimatorNull;

    private void Start()
    {
        cloneAnimator = playerCharacterController.graphicsClone.GetComponentInChildren<Animator>();
        isAnimatorNull = representationAnimator == null;
    }
    private void FixedUpdate()
    {
      ApplyAnimation();
    }

    public void ApplyAnimation()
    {
        if (isAnimatorNull)
            return;
        PlayerState currentPlayerState = playerCharacterController.playerState;
        representationAnimator.SetBool(playerState[currentPlayerState], true);
        cloneAnimator.SetBool(playerState[currentPlayerState], true);
        if (oldState == currentPlayerState) return;
        representationAnimator.SetBool(playerState[oldState], false);
        cloneAnimator.SetBool(playerState[oldState], false);
        oldState = currentPlayerState;
    }
}
