using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;  // Nếu cần dùng để hỗ trợ thuộc tính
using ExitGames.Client.Photon;      // Để sử dụng Hashtable
using UnityEngine.SceneManagement;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [Header("Player Slot UI")]
    public PlayerSlotUI slot1;  // Góc trái dưới
    public PlayerSlotUI slot2;  // Góc phải dưới
    public PlayerSlotUI slot3;  // Góc trái trên
    public PlayerSlotUI slot4;  // Góc phải trên

    [Header("Start Game")]
    public Button startGameButton1;  // Chỉ hiển thị cho MasterClient
    public Button startGameButton2;  // Chỉ hiển thị cho MasterClient
    public Button startGameButton3;  // Chỉ hiển thị cho MasterClient
    public Button startGameButton4;  // Chỉ hiển thị cho MasterClient


    public Text roomID;

    void Start()
    {
        UpdatePlayerSlots();
        
        // Hiển thị Room ID
        roomID.text = $"Room ID: {PhotonNetwork.CurrentRoom.Name}";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerSlots();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerSlots();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerSlots();
        CheckAllPlayersReady();
        RefreshAllPlayerReadyButtons();
    }

    void UpdatePlayerSlots()
    {
        List<Player> orderedPlayers = GetOrderedPlayers();

        slot1.Clear();
        slot2.Clear();
        slot3.Clear();
        slot4.Clear();

        for (int i = 0; i < orderedPlayers.Count && i < 4; i++)
        {
            PlayerSlotUI slot = GetSlotByIndex(i);
            Button button = GetReadyButtonByIndex(i);
            if (slot != null)
            {
                slot.SetPlayerInfo(orderedPlayers[i].NickName, button);
            }
        }
    }


    PlayerSlotUI GetSlotByIndex(int index)
    {
        switch(index)
        {
            case 0: return slot1;
            case 1: return slot2;
            case 2: return slot3;
            case 3: return slot4;
            default: return null;
        }
    }
    Button GetReadyButtonByIndex(int index)
    {
        switch(index)
        {
            case 0: return startGameButton1;
            case 1: return startGameButton2;
            case 2: return startGameButton3;
            case 3: return startGameButton4;
            default: return null;
        }
    }

    void CheckAllPlayersReady()
    {
        bool allReady = true;
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            object isReady;
            if (!p.CustomProperties.TryGetValue("Ready", out isReady) || !(bool)isReady)
            {
                allReady = false;
                break;
            }
        }

        if (PhotonNetwork.PlayerList.Length < 2){
            Debug.Log("Chưa đủ người chơi để bắt đầu.");
            allReady = false;
        }
        if(allReady && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Tất cả player đã sẵn sàng, game sẽ khởi động tự động.");
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }

    
    public void OnOurReadyButtonPressed()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        // Nếu thuộc tính "Ready" đã tồn tại, ta sẽ đảo trạng thái
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready"))
        {
            bool currentReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"];
            props["Ready"] = !currentReady; // đảo trạng thái
        }
        else
        {
            // Nếu chưa có, đặt trạng thái Ready là true
            props["Ready"] = true;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    void RefreshAllPlayerReadyButtons()
    {
        List<Player> orderedPlayers = GetOrderedPlayers();

        for (int i = 0; i < orderedPlayers.Count && i < 4; i++)
        {
            bool isReady = false;
            if (orderedPlayers[i].CustomProperties.ContainsKey("Ready"))
            {
                isReady = (bool)orderedPlayers[i].CustomProperties["Ready"];
            }
            PlayerSlotUI slot = GetSlotByIndex(i);
            if (slot != null)
            {
                slot.UpdateReadyUI(isReady);
            }
        }
    }


    List<Player> GetOrderedPlayers()
    {
        Player[] allPlayers = PhotonNetwork.PlayerList;
        List<Player> orderedPlayers = new List<Player>();

        // Thêm local player luôn đứng đầu
        Player localPlayer = PhotonNetwork.LocalPlayer;
        orderedPlayers.Add(localPlayer);

        // Thêm các player khác (tránh trùng lặp)
        foreach (Player p in allPlayers)
        {
            if (p != localPlayer)
            {
                orderedPlayers.Add(p);
            }
        }
        return orderedPlayers;
    }


}

