using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class DrawPileClickHandler : MonoBehaviour, IPointerClickHandler
{
    // Tham chiếu đến CardDrawer hoặc có thể tích hợp trực tiếp vào GameManager nếu muốn
    public CardDrawer cardDrawer;  // Giả sử CardDrawer chứa logic rút bài tại Master Client

    public void OnPointerClick(PointerEventData eventData)
    {
        // Khi click, gửi yêu cầu rút bài cho Master Client qua RPC.
        // Gửi ActorNumber của người chơi đã click để Master biết gửi lá bài về đâu.
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("RPC_RequestDrawCard", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        Debug.Log("Đã gửi yêu cầu rút bài đến Master Client");
    }
}
