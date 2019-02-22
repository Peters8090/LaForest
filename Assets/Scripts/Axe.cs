using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    void Start()
    {
        if (UsefulMethods.FindTopParent(gameObject).name != PlayerInfo.myPlayerInfo.nick)
        {
            enabled = false;
        }
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && UsefulReferences.playerWeapons.canAttack)
        {
            UsefulReferences.playerAnimator.Play("Attack");
        }
    }
}
