using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInfo
{
    public static List<PlayerInfo> players = new List<PlayerInfo>();
    public Player pp;
    public GameObject gameObject;
    public string nick = "";
    public static PlayerInfo myPlayerInfo;

    public static void DebugPlayersList()
    {
        string text2Debug = "Total players count: " + players.Count + "\nPlayers: \n";
        foreach (var player in players)
        {
            text2Debug += "Nick: " + player.nick + " \n";
        }
        Debug.Log(text2Debug);
    }

    public static PlayerInfo FindPlayerInfoByPP(Player pp)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].pp == pp)
            {
                return players[i];
            }
        }
        return null;
    }

    public static Player FindPPByGO(GameObject go)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].gameObject == go)
            {
                return players[i].pp;
            }
        }
        return null;
    }

    public string SerializePlayer()
    {
        string weaponSerialized;
        if(gameObject.GetComponent<UsefulReferencesPlayer>().eq.transform.childCount > 0)
        {
            weaponSerialized = gameObject.GetComponent<UsefulReferencesPlayer>().eq.transform.GetChild(0).GetComponent<WeaponMB>().weapon.Serialize();
        } else
        {
            weaponSerialized = "disarmed";
        }

        return weaponSerialized + "." + gameObject.GetPhotonView().ViewID;
    }

    public static Dictionary<string, string> DeserializePlayer(string serializedPlayer)
    {
        string[] serializedPlayerArray = serializedPlayer.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string> deserializedPlayer = new Dictionary<string, string>();
        deserializedPlayer.Add("weapon", serializedPlayerArray[0]);
        deserializedPlayer.Add("playerPvID", serializedPlayerArray[1]);
        return deserializedPlayer;
    }
}
