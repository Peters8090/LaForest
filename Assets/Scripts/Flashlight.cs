using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && UsefulReferences.playerWeapons.canAttack)
        {
            UsefulReferences.playerAnimator.Play("Attack");
        }
    }
}
