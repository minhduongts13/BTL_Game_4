// using UnityEngine;
// using Photon.Pun;
// using Photon.Realtime;

// public class CardDraw : MonoBehaviourPunCallbacks
// {
//     // Tham chiếu đến DeckManager để rút bài
//     public DeckManager deckManager;
//     // Tham chiếu đến PlayerHandManager của người chơi (để thêm card vào tay người chơi sau khi rút)
//     // Lưu ý: nếu mỗi client có instance riêng cho PlayerHandManager, hãy đảm bảo rằng CardDraw ở client của Master sẽ gửi RPC riêng cho client yêu cầu và client đó tự gọi SpawnCard.
    
//     // ---------------------------------------------------
//     // RPC: Khi một client cần rút bài, họ gọi RPC này đến Master Client.
//     // ---------------------------------------------------
//     [PunRPC]
//     public void RPC_RequestDrawCard(int requestingActor)
//     {
//         // Chỉ Master Client mới thực hiện hành động này
//         if (!PhotonNetwork.IsMasterClient)
//             return;
            
//         // Rút bài từ DeckManager
//         CardData drawnCard = deckManager.DrawCard();
//         if (drawnCard == null)
//         {
//             Debug.LogWarning("Deck trống! Không thể rút bài.");
//             return;
//         }
        
//         // Gửi thông tin lá bài vừa rút qua RPC đến client yêu cầu
//         // Vì CardData thường là asset không tự đồng bộ, chúng ta sẽ gửi thông tin định danh (ví dụ: cardColor và cardNumber)
//         PhotonView pv = PhotonView.Get(this);
//         // Lấy đối tượng Player của người chơi yêu cầu dựa trên ActorNumber
//         Player requestingPlayer = PhotonNetwork.CurrentRoom.GetPlayer(requestingActor);
//         if(requestingPlayer != null)
//         {
//             pv.RPC("RPC_GiveDrawnCard", requestingPlayer, drawnCard.cardColor, drawnCard.cardNumber);
//             Debug.Log("Master Client gửi thông tin lá bài vừa rút cho Actor " + requestingActor);
//         }
//     }
    
//     // ---------------------------------------------------
//     // RPC: Được gọi trên client của người chơi rút bài. 
//     // Dựa vào các thông tin nhận được, client tái tạo CardData và thêm vào tay của mình.
//     // ---------------------------------------------------
//     [PunRPC]
//     public void RPC_GiveDrawnCard(string cardColor, int cardNumber, PhotonMessageInfo info)
//     {
//         // Dựa vào thông tin cardColor và cardNumber, client tự tìm kiếm CardData từ Resources hoặc từ một Dictionary đã cài đặt sẵn
//         CardData cardData = GetCardDataFromInfo(cardColor, cardNumber);
//         if(cardData != null)
//         {
//             // Giả sử PlayerHandManager là script quản lý tay bài của người chơi cục bộ.
//             // Bạn cần đảm bảo rằng PlayerHandManager có thể được truy cập (ví dụ bằng cách tạo singleton, hoặc qua Inspector)
//             PlayerHandManager.Instance.SpawnCard(cardData);
//             PlayerHandManager.Instance.UpdateLocalCardCount();
//             Debug.Log("Đã nhận thông tin lá bài và cập nhật vào tay của người chơi.");
//         }
//         else
//         {
//             Debug.LogWarning("Không tìm thấy CardData cho " + cardColor + " " + cardNumber);
//         }
//     }
    
//     // ---------------------------------------------------
//     // Hàm phụ: Tìm CardData tương ứng dựa trên thông tin (CardColor, CardNumber)
//     // Bạn có thể triển khai cách này theo logic quản lý tài nguyên của bạn
//     // ---------------------------------------------------
//     private CardData GetCardDataFromInfo(string cardColor, int cardNumber)
//     {
//         CardData[] allCards = Resources.LoadAll<CardData>("Cards");
//         foreach (CardData card in allCards)
//         {
//             if(card.cardColor.Equals(cardColor) && card.cardNumber == cardNumber)
//                 return card;
//         }
//         return null;
//     }
    
//     // ---------------------------------------------------
//     // Hàm public: Client gọi hàm này để yêu cầu rút bài. Nó sẽ gửi RPC đến Master Client.
//     // ---------------------------------------------------
//     public void RequestDrawCard()
//     {
//         PhotonView pv = PhotonView.Get(this);
//         pv.RPC("RPC_RequestDrawCard", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
//         Debug.Log("Đã gửi yêu cầu rút bài đến Master Client.");
//     }
// }
