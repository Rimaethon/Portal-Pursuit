﻿using Controller;
using UnityEngine;

/// <summary>
///     This class handles behavior for jumping on top of something with a head
/// </summary>
public class Head : MonoBehaviour
{
    [Header("Settings")] [Tooltip("The health component associated with this head")]
    public Health associatedHealth;

    [Tooltip("The amount of damage to deal when jumped on")]
    public int damage = 1;

    /// <summary>
    ///     Description:
    ///     Standard Unity function that is called when a collider enters a trigger
    ///     Input:
    ///     Collision collision
    ///     Return:
    ///     void (no return)
    /// </summary>
    /// <param name="collision">The collision information of what has collided with the attached collider</param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Feet")
        {
            associatedHealth.TakeDamage(damage);
            BouncePlayer();
        }
    }

    /// <summary>
    ///     Description:
    ///     Tells the player controller attatched to the player object collided with to bounce
    ///     Inputs:
    ///     none
    ///     Returns:
    ///     void
    /// </summary>
    private void BouncePlayer()
    {
        var playerController = GameManager.instance.player.GetComponentInChildren<ThirdPersonCharacterController>();
        if (playerController != null)
        {
            playerController.ResetJumps();
            playerController.Bounce(1, 1.5f);
        }
    }
}