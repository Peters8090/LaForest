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

    Dictionary<string, Vector3> playerModeldefaultPos = new Dictionary<string, Vector3>();
    Dictionary<string, Quaternion> playerModelDefaultRot = new Dictionary<string, Quaternion>();

    UsefulReferencesPlayer urp;

    //the attacker transform.forward * power of hit (for example the direction from which we got shot)
    //I set it nullable because it is an easy way to detect whether we have an attacker or not, f.e. player dies of hunger
    public Vector3? dir2Fall = null;

    void Start()
    {
        urp = GetComponent<UsefulReferencesPlayer>();
        myAnimator = urp.ybot.gameObject.GetComponent<Animator>();
        mainCameraPosRot = urp.mainCameraPosRot;
        mainCameraPosRotParent = mainCameraPosRot.transform.parent;
        my3dNick = transform.Find("TextMeshPro Nick").gameObject;

        //first we save all player model's bones pos and rot
        foreach (var bone in ((GameObject)Resources.Load("Player")).transform.Find("ybot").transform.GetComponentsInChildren<Transform>())
        {
            playerModeldefaultPos.Add(bone.name, bone.localPosition);
            playerModelDefaultRot.Add(bone.name, bone.localRotation);
        }
        PlayerRegenerates();
        RestorePlayerModelPosAndRot();
        InvokeRepeating("RestorePlayerModelPosAndRot", 0, 0.1f);
    }

    void Update()
    {
        if (photonView.IsMine)
            died = UsefulReferences.playerDeath.died;

        //to detect the moment, in which the player dies
        if (died && !prevDied)
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
    
    /// <summary>
    /// Use to restore start pos and rot of bones to them unless the bone doesn't exist for example it wasn't a bone, but a weapon
    /// </summary>
    [PunRPC]
    public void RestorePlayerModelPosAndRot()
    {
        if (died)
            return;
        foreach (var bone in urp.ybot.transform.GetComponentsInChildren<Transform>())
        {
            //because: 1) the bone always is on position 2) it always has Rigidbody component, if it hadn't that component, we wouldn't have to restore its position
            if (!playerModeldefaultPos.ContainsKey(bone.name) || !playerModelDefaultRot.ContainsKey(bone.name) || !bone.GetComponent<Rigidbody>())
            {
                continue;
            }
            if (playerModeldefaultPos.ContainsKey(bone.name) && playerModelDefaultRot.ContainsKey(bone.name))
            {
                bone.localPosition = playerModeldefaultPos[bone.name];
                bone.localRotation = playerModelDefaultRot[bone.name];
            }
        }
    }

    //health: 100
    void PlayerRegenerates()
    {
        foreach (var rigidbody in ybotRagdollRigidbodies)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        myAnimator.enabled = true;

        RestorePlayerModelPosAndRot();

        if (photonView.IsMine)
        {
            //to make MainCamera not to see what's under the terrain (its position and rotation mustn't depend on ybot's
            mainCameraPosRot.transform.parent = mainCameraPosRotParent;
            mainCameraPosRot.transform.localPosition = new Vector3(0, 0.31f, 0.32f);
            mainCameraPosRot.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        //disable ragdoll by enable the animator
        GetComponent<CharacterController>().enabled = true;
        my3dNick.SetActive(true);
        InvokeRepeating("RestorePlayerModelPosAndRot", 0, 0.1f);
    }

    //health: 0
    void PlayerDies()
    {
        CancelInvoke();
        foreach (var rigidbody in ybotRagdollRigidbodies)
        {
            //make ybot not to shake
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }

        //enable ragdoll by disabling the animator (and setting rigidbody not to kinematic and setting useGravity to true)
        myAnimator.enabled = false;

        foreach (var bone in urp.ybot.transform.GetComponentsInChildren<Transform>())
        {
            //because: 1) the bone always is on position 2) it always has Rigidbody component, if it hadn't that component, we wouldn't have to restore its position
            if (!playerModeldefaultPos.ContainsKey(bone.name) || !playerModelDefaultRot.ContainsKey(bone.name) || !bone.GetComponent<Rigidbody>())
            {
                continue;
            }

            //we check whether do we have any attacker
            if (dir2Fall != null)
            {
                //we push player to dir2Fall
                bone.GetComponent<Rigidbody>().AddForce((Vector3)dir2Fall, ForceMode.Impulse);
            }
        }

        //reset the direction to fall
        dir2Fall = null;

        if (photonView.IsMine)
        {
            //spectator mode for the dead player
            mainCameraPosRot.transform.parent = transform;
            mainCameraPosRot.transform.localPosition = new Vector3(0, 5, 0);
            mainCameraPosRot.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        //make other players not to collide with the CharacterController while its position is not correct (ragdoll is enabled)
        GetComponent<CharacterController>().enabled = false;
        my3dNick.SetActive(false);
    }
}
