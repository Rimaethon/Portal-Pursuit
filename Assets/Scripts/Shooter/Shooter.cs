﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

/// <summary>
///     This class stores information on obtained guns and the currently equipped gun.
///     It also reads input for firing the guns
/// </summary>
public class Shooter : MonoBehaviour
{
    [Header("Settings")] [Tooltip("The list of guns this shooter can potentially have access to")]
    public List<Gun> guns = new();

    [Tooltip("The index in the available gun list of the gun currently equipped")]
    public int equippedGunIndex;

    [Tooltip("The input manager that this reads input from")]
    public InputManager inputManager;

    [Tooltip("Whether or not this shooter is controlled by the player")]
    public bool isPlayerControlled;

    /// <summary>
    ///     Description:
    ///     Standard Unity function called once before the first update
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void Start()
    {
        SetUpInput();
        SetUpGuns();
    }

    /// <summary>
    ///     Description:
    ///     Standard Unity function called once per frame
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void Update()
    {
        CheckInput();
    }

    /// <summary>
    ///     Description:
    ///     Checks input and responds accordingly
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void CheckInput()
    {
        if (!isPlayerControlled) return;

        // Do nothing when paused
        if (Time.timeScale == 0) return;

        if (guns.Count > 0)
        {
            if (guns[equippedGunIndex].fireType == Gun.FireType.semiAutomatic)
            {
                if (inputManager.FirePressed) FireEquippedGun();
            }
            else if (guns[equippedGunIndex].fireType == Gun.FireType.automatic)
            {
                if (inputManager.FirePressed || inputManager.FireHeld) FireEquippedGun();
            }

            if (inputManager.CycleWeaponInput != 0) CycleEquippedGun();

            if (inputManager.NextWeaponPressed) GoToNextWeapon();

            if (inputManager.PreviousWeaponPressed) GoToPreviousWeapon();
        }
    }

    /// <summary>
    ///     Description:
    ///     Goes to the next weapon of the available weapons
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    public void GoToNextWeapon()
    {
        var availableGuns = guns.Where(item => item.available).ToList();
        var maximumAvailableGunIndex = availableGuns.Count - 1;
        var equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);

        equippedAvailableGunIndex += 1;
        if (equippedAvailableGunIndex > maximumAvailableGunIndex) equippedAvailableGunIndex = 0;

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    /// <summary>
    ///     Description:
    ///     Goes to the previous weapon of the available weapons
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    public void GoToPreviousWeapon()
    {
        var availableGuns = guns.Where(item => item.available).ToList();
        var maximumAvailableGunIndex = availableGuns.Count - 1;
        var equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);

        equippedAvailableGunIndex -= 1;
        if (equippedAvailableGunIndex < 0) equippedAvailableGunIndex = maximumAvailableGunIndex;

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    /// <summary>
    ///     Description:
    ///     Cycles through the available guns starting from the currently equipped gun
    ///     and moving in the direction of the mouse scroll input.
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void CycleEquippedGun()
    {
        var cycleInput = inputManager.CycleWeaponInput;
        var availableGuns = guns.Where(item => item.available).ToList();
        var maximumAvailableGunIndex = availableGuns.Count - 1;
        var equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);
        if (cycleInput < 0)
        {
            equippedAvailableGunIndex += 1;
            if (equippedAvailableGunIndex > maximumAvailableGunIndex) equippedAvailableGunIndex = 0;
        }
        else if (cycleInput > 0)
        {
            equippedAvailableGunIndex -= 1;
            if (equippedAvailableGunIndex < 0) equippedAvailableGunIndex = maximumAvailableGunIndex;
        }

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    /// <summary>
    ///     Description:
    ///     Equips the gun from the list of guns at the given index
    ///     Input:
    ///     int gunIndex
    ///     Return:
    ///     void (no return)
    /// </summary>
    /// <param name="gunIndex">The index of the gun to make the equipped gun</param>
    public void EquipGun(int gunIndex)
    {
        equippedGunIndex = gunIndex;
        guns[equippedGunIndex].gameObject.SetActive(true);
        for (var i = 0; i < guns.Count; i++)
            if (equippedGunIndex != i)
                guns[i].gameObject.SetActive(false);
    }

    /// <summary>
    ///     Description:
    ///     Sets up the available guns for use
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void SetUpGuns()
    {
        foreach (var gun in guns)
            if (gun != null)
            {
                if (gun.available && guns[equippedGunIndex] == gun)
                    gun.gameObject.SetActive(true);
                else
                    gun.gameObject.SetActive(false);
            }
    }

    /// <summary>
    ///     Description:
    ///     Sets up the input manager
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    private void SetUpInput()
    {
        if (inputManager == null) inputManager = FindObjectOfType<InputManager>();
        if (inputManager == null)
            Debug.LogError(
                "There is no input manager in the scene, the shooter script requires an input manager in order to work for the player");
    }

    /// <summary>
    ///     Description:
    ///     Attempts to fire the equipped gun. Fails if the gun is not available
    ///     Input:
    ///     none
    ///     Return:
    ///     void (no return)
    /// </summary>
    public void FireEquippedGun()
    {
        if (guns[equippedGunIndex].available && guns[equippedGunIndex] != null) guns[equippedGunIndex].Fire();
    }

    /// <summary>
    ///     Description:
    ///     Makes a gun available to the player
    ///     Input:
    ///     int gunIndex
    ///     Return:
    ///     void (no return)
    /// </summary>
    /// <param name="gunIndex">The index of the gun to make available</param>
    public void MakeGunAvailable(int gunIndex)
    {
        if (gunIndex < guns.Count && guns[gunIndex] != null && guns[gunIndex].available == false)
        {
            guns[gunIndex].available = true;
            EquipGun(gunIndex);
        }
    }
}