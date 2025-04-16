using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
	
	// public PhotonView playerPrefab;
	public InputField playerNickname;
	public InputField RoomID;
	public Text feedbackText; 
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

	public override void OnConnectedToMaster(){
		Debug.Log("Connected to Master");
		// PhotonNetwork.JoinRandomOrCreateRoom();
	}
	
	public override void OnJoinedRoom(){
		Debug.Log($"Joined a room. Room ID: {PhotonNetwork.CurrentRoom.Name}");
        SceneManager.LoadSceneAsync("Lobby");

		// PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
	}

	public void StartTheGame(){
        if (playerNickname.text.Length < 1){
		    PhotonNetwork.NickName = "Nameless";
        }
		else PhotonNetwork.NickName = playerNickname.text;
        if (RoomID.text.Length > 1){
		    string enteredRoomCode = RoomID.text; // Nhập từ UI
            PhotonNetwork.JoinRoom(enteredRoomCode);
        }
		else {
            string randomRoomCode = UnityEngine.Random.Range(1000, 9999).ToString(); 
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(randomRoomCode, roomOptions);
        }
	}
	
	public override void OnJoinRoomFailed(short returnCode, string message)
    {
        feedbackText.text = "Room not found. Please check the code.";
        Debug.LogWarning($"Join failed: {message}");
    }
}