using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSounds : MonoBehaviourPunCallbacks
{
    void Update()
    {
        if (photonView.IsMine)
            AudioListener.volume = GameSettings.volume;
    }

    public void PlaySound(AudioClip audioClip)
    {
        photonView.RPC("PlaySoundRPC", RpcTarget.All, audioClip.name);
    }

    [PunRPC]
    void PlaySoundRPC(string audioClipName, PhotonMessageInfo pmi)
    {
        PlayerInfo.FindPlayerInfoByPP(pmi.Sender).gameObject.GetComponent<AudioSource>().PlayOneShot((AudioClip) Resources.Load(audioClipName));
    }
}
