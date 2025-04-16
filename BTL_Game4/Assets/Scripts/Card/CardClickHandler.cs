using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler
{
    public CardAnimationController cardAnimationController;
    public PlayerHandManager playerHandManager;
    public PlayedCardsManager playedCardsManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        CardDisplay cardDisplay = GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.cardData == null)
        {
            Debug.LogWarning("Không tìm thấy CardData!");
            return;
        }

        // Chỉ cho phép người chơi đánh bài nếu đang đến lượt của họ
        if (UnoGameManager.Instance == null)
        {
            Debug.LogError("UnoGameManager chưa được gán!");
            return;
        }

        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        if (UnoGameManager.Instance.GetCurrentTurnActor() != myActorNumber)
        {
            Debug.Log("Chưa đến lượt của bạn!");
            return;
        }

        // Gửi RPC đến MasterClient
        PhotonView.Get(UnoGameManager.Instance).RPC("RPC_PlayCard", RpcTarget.MasterClient,
            cardDisplay.cardData.cardColor,
            cardDisplay.cardData.cardNumber,
            myActorNumber);
    }
}
