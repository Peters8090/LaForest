using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerRagdoll : MonoBehaviourPunCallbacks
{
    public bool died = false;
    bool prevDied = false; //to detect the moment of player death and regenerating
    public Rigidbody[] ybotRagdollRigidbodies;
    Animator myAnimator;
    GameObject mainCameraPosRot;
    Transform mainCameraPosRotParent;
    GameObject my3dNick;

    void Start()
    {
        myAnimator = transform.Find("ybot").gameObject.GetComponent<Animator>();
        mainCameraPosRot = GetComponent<UsefulReferencesPlayer>().mainCameraPosRot;
        mainCameraPosRotParent = mainCameraPosRot.transform.parent;
        my3dNick = transform.Find("TextMeshPro Nick").gameObject;
        PlayerRegenerates();
    }
    
    void Update()
    {
        if (photonView.IsMine)
            died = UsefulReferences.playerDeath.died;
        
        //to detect the moment, in which the player dies
        if(died && !prevDied)
        {
            PlayerDies();
        }

        //to detect the moment, in which the player regenerates
        if (!died && prevDied)
        {
            PlayerRegenerates();
        }
        prevDied = died;
    }

    void PlayerRegenerates()
    {
        foreach (var rigidbody in ybotRagdollRigidbodies)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }
        myAnimator.enabled = true;

        if (photonView.IsMine)
        {
            mainCameraPosRot.transform.parent = mainCameraPosRotParent;
            mainCameraPosRot.transform.localPosition = new Vector3(0, 0.31f, 0.32f);
            mainCameraPosRot.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        //disable ragdoll by enable the animator
        GetComponent<CharacterController>().enabled = true;
        my3dNick.SetActive(true);
    }

    void PlayerDies()
    {
        foreach (var rigidbody in ybotRagdollRigidbodies)
        {
            //make ybot not to shake
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
        //enable ragdoll by disabling the animator (and setting rigidbody not to kinematic and setting useGravity to true)
        myAnimator.enabled = false;

        if (photonView.IsMine)
        {
            //spectator mode for the dead player
            mainCameraPosRot.transform.parent = transform;
            mainCameraPosRot.transform.localPosition = new Vector3(0, 5, 0);
            mainCameraPosRot.transform.localEulerAngles = new Vector3(90, 0, 0);
            //to make MainCamera not to see what's under the terrain (its position and rotation mustn't depend on ybot's
        }
        //make other players not to collide with the CharacterController while its position is not correct (ragdoll is enabled)
        GetComponent<CharacterController>().enabled = false;
        my3dNick.SetActive(false);
    }
}
