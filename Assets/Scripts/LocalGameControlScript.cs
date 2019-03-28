using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// This script controls game settings like locking mouse, player movement
/// </summary>
public class LocalGameControlScript : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        UsefulReferences.Initialize(null);
    }

    void Update()
    {
        //the position is higher, the priority is lower
        if (UsefulReferences.initialized && !global::MainMenu.menu)
        {
            Normal();
            Moving();
            Jumping();
            Attacking();
            PauseMenu();
            Died();
            Aiming();
        }
        else
            MainMenu();
    }

    /// <summary>
    /// Normal settings, its priority is the lowest
    /// </summary>
    void Normal()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UsefulReferences.playerMovement.movementLocked = false;
        UsefulReferences.playerMovement.mouseLookLocked = false;
        UsefulReferences.playerWeapons.disarmed = false;
        UsefulReferences.playerWeapons.canChangeWeapons = true;
        UsefulReferences.playerWeapons.canAttack = true;
        UsefulReferences.playerMovement.slowDown = false;
        UsefulReferences.healthUI.SetActive(true);
        UsefulReferences.crosshairUI.SetActive(false);
        UsefulReferences.deathUI.SetActive(false);
        global::PauseMenu.canOpenPauseMenu = true;
    }

    void Aiming()
    {
        if (!UsefulReferences.playerWeapons.disarmed)
        {
            UsefulReferences.crosshairUI.SetActive(true);
        }
    }

    void Moving()
    {
        if (UsefulReferences.playerMovement.moving && !UsefulReferences.playerWeapons.nonAnimWeapon)
        {
            UsefulReferences.playerWeapons.disarmed = true;
        }
    }

    void Jumping()
    {
        if (!UsefulReferences.playerMovement.isGrounded)
        {
            UsefulReferences.playerMovement.slowDown = true;
            UsefulReferences.playerWeapons.disarmed = true;
            UsefulReferences.playerWeapons.canAttack = false;
        }
    }

    void Attacking()
    {
        if (UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            UsefulReferences.playerWeapons.canAttack = false;
            UsefulReferences.playerWeapons.canChangeWeapons = false;
            UsefulReferences.playerMovement.slowDown = true;
            UsefulReferences.crosshairUI.SetActive(false);
        }
    }

    void MainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UsefulReferences.healthUI.SetActive(false);
        UsefulReferences.crosshairUI.SetActive(false);
        global::PauseMenu.canOpenPauseMenu = false;
    }

    void PauseMenu()
    {
        if (global::PauseMenu.menu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UsefulReferences.playerMovement.movementLocked = true;
            UsefulReferences.playerMovement.mouseLookLocked = true;
            UsefulReferences.playerWeapons.canAttack = false;
            UsefulReferences.playerWeapons.disarmed = true;
            UsefulReferences.playerWeapons.canChangeWeapons = false;
            UsefulReferences.healthUI.SetActive(false);
        }
    }
    
    void Died()
    {
        if (UsefulReferences.playerDeath.died)
        {
            UsefulReferences.deathUI.SetActive(true);
            UsefulReferences.playerMovement.movementLocked = true;
            UsefulReferences.playerMovement.mouseLookLocked = true;
            UsefulReferences.playerWeapons.canAttack = false;
            UsefulReferences.playerWeapons.disarmed = true;
            UsefulReferences.playerWeapons.canChangeWeapons = false;
            UsefulReferences.healthUI.SetActive(false);
            global::PauseMenu.canOpenPauseMenu = false;
        }
    }
}
