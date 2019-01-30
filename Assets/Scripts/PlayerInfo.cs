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
    public string nick = "";
    public static PlayerInfo myPlayerInfo;

    public GameObject gameObject()
    {
        return GameObject.Find(nick);
    }

    public static void DebugPlayersList()
    {
        string text2Debug = "Total players count: " + players.Count + "\nPlayers: \n";
        foreach (var player in players)
        {
            text2Debug += "Nick: " + player.nick + " \n";
        }
        Debug.Log(text2Debug);
    }

    public static PlayerInfo FindPlayer(Player pp)
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
}
