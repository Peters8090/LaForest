using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sword : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!photonView.IsMine)
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
