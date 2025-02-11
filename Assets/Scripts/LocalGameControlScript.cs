﻿using System.Collections;
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

    void LateUpdate()
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
        UsefulReferences.playerWeapons.unarmed = false;
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
        if(Input.GetButton("Fire2")) //aiming hotkey
        {
            if (!UsefulReferences.playerWeapons.unarmed)
            {
                UsefulReferences.crosshairUI.SetActive(true);
            }
        }
    }

    void Moving()
    {
        // If weapon plays an animation, it can't be used while moving because it disturbs the animation
        if (UsefulReferences.playerMovement.moving && !UsefulReferences.playerWeapons.nonAnimWeapon) 
        {
            UsefulReferences.playerWeapons.unarmed = true;
            if(!UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Movement") && !UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                UsefulReferences.playerAnimator.Play("Movement");
        }
    }

    void Jumping()
    {
        if (!UsefulReferences.playerMovement.isGrounded)
        {
            UsefulReferences.playerMovement.slowDown = true;

            // If weapon plays an animation, it can't be used while jumping because it disturbs the animation
            if (!UsefulReferences.playerWeapons.nonAnimWeapon)
            {
                UsefulReferences.playerWeapons.unarmed = true;
                UsefulReferences.playerWeapons.canAttack = false;
            }
        }
    }

    void Attacking()
    {
        if (UsefulReferences.playerWeapons.isAttacking)
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
            UsefulReferences.playerWeapons.unarmed = true;
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
            UsefulReferences.playerWeapons.unarmed = true;
            UsefulReferences.playerWeapons.canChangeWeapons = false;
            UsefulReferences.healthUI.SetActive(false);
            UsefulReferences.playerMovement.SetMainCamerasPosRot(); //to make main camera look at the body ragdoll while the mouse look is locked
            global::PauseMenu.canOpenPauseMenu = false;
        }
    }
}
