using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rock : MonoBehaviourPunCallbacks
{
    public static int ammo = 10000;
    float delay = 0.1f;
    float counting = 0f;
    public static float speed = 0.8f;
    AudioClip clip;

    Vector3 targetPos;
    Quaternion targetRot;

    void Start()
    {
        clip = (AudioClip)Resources.Load("RockSound");
    }
    
    void Update()
    {
        //if ammo is <= 0 player cannot shoot
        if (ammo <= 0)
        {
            ammo = 0;
            return;
        }

        //player can shoot 1/delay times per second
        if (counting > 0)
            counting -= Time.deltaTime;

        if(Input.GetButtonDown("Fire1") && ammo > 0 && counting <= 0)
        {
            counting = delay;
            GameObject rock = PhotonNetwork.Instantiate("RockBullet", Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.rotation);
            //instantiate the rock and let player throw it
            rock.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * speed, ForceMode.Impulse);
            //UsefulReferences.playerAudioSource.PlayOneShot(clip);
            UsefulReferences.playerSounds.PlaySound(clip);
            ammo--;
        }
    }
}
