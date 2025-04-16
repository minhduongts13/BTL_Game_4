using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameplaySceneManager : MonoBehaviourPunCallbacks
{
    public PlayerHandManager localHandManager;  // Được gán trong Inspector (Panel Player 1)
    public OpponentHandManager opponent2Manager;  // Gán cho Player 2 Panel
    public OpponentHandManager opponent3Manager;  // Gán cho Player 3 Panel
    public OpponentHandManager opponent4Manager;  // Gán cho Player 4 Panel

    void Start()
    {
        List<Player> orderedPlayers = GetOrderedPlayers();
        // Sắp xếp local player
        if(orderedPlayers.Count > 0)
        {
            // LocalPlayer luôn được hiển thị đầy đủ (face up)
            localHandManager.SetPlayer();
        }
        // Với đối thủ chỉ hiển thị số lượng bài và card back
        if(orderedPlayers.Count > 1)
        {
            opponent2Manager.SetPlayer(orderedPlayers[1]);
        }
        if(orderedPlayers.Count > 2)
        {
            opponent3Manager.SetPlayer(orderedPlayers[2]);
        }
        if(orderedPlayers.Count > 3)
        {
            opponent4Manager.SetPlayer(orderedPlayers[3]);
        }
    }

    List<Player> GetOrderedPlayers()
    {
        Player[] allPlayers = PhotonNetwork.PlayerList;
        List<Player> orderedPlayers = new List<Player>();
        orderedPlayers.Add(PhotonNetwork.LocalPlayer);
        foreach (Player p in allPlayers)
        {
            if (p != PhotonNetwork.LocalPlayer)
                orderedPlayers.Add(p);
        }
        return orderedPlayers;
    }
}
