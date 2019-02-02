using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls game settings like locking mouse, player movement
/// </summary>
public class LocalGameControlScript : MonoBehaviour
{
    void Update()
    {
        //the position is higher, the priority is lower
        if(UsefulReferences.initialized && !global::MainMenu.menu)
        {
            Normal();
            Moving();
            Jumping();
            Attacking();
            PauseMenu();
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
        UsefulReferences.playerWeapons.canAttack = true;
        UsefulReferences.playerMovement.slowDown = false;
    }

    void Moving()
    {
        if(UsefulReferences.playerMovement.moving)
            UsefulReferences.playerWeapons.disarmed = true;
    }

    void Jumping()
    {
        if(!UsefulReferences.playerCharacterController.isGrounded)
        {
            UsefulReferences.playerMovement.slowDown = true;
            UsefulReferences.playerWeapons.disarmed = true;
            UsefulReferences.playerWeapons.canAttack = false;
        }
    }

    void Attacking()
    {
        if(UsefulReferences.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            UsefulReferences.playerWeapons.canAttack = false;
            UsefulReferences.playerMovement.slowDown = true;
        }
    }

    void MainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void PauseMenu()
    {
        if(global::PauseMenu.menu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UsefulReferences.playerMovement.movementLocked = true;
            UsefulReferences.playerMovement.mouseLookLocked = true;
            UsefulReferences.playerWeapons.canAttack = false;
        }
    }
}
