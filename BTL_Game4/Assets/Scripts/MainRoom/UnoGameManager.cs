using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class UnoGameManager : MonoBehaviourPunCallbacks
{
    public static UnoGameManager Instance { get; private set; }

    // Thành phần quản lý game (logic cốt lõi)
    public GameState gameState;

    // Các reference UI – bạn cần gán qua Inspector trong Scene Gameplay
    [Header("UI References")]
    public PlayerHandManager localHandManager;          // Tay bài của local player (Player1)
    public OpponentHandManager[] opponentHandManagers;    // Tay bài của đối thủ (Player2,3,...)
    public PlayedCardsManager playedCardsManager;         // Khu vực hiển thị discard pile
    public TurnHighlighter turnHighlighter;               // Một script để hiển thị lượt chơi (Text)

    // Chuyển scene không được xử lý ở đây; script này chỉ quản lý logic trong Gameplay

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ qua các scene nếu cần
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // MasterClient làm authority về game state
        if (PhotonNetwork.IsMasterClient)
        {
            // Khởi tạo game state mới (logic dựa trên Uno game console)
            gameState = new GameState();
            // Deal bài ban đầu cho tất cả các player (ví dụ: 7 lá mỗi người)
            DealInitialCards();
        }
    }

    // Lấy danh sách player theo thứ tự mà local player luôn đứng đầu
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

    /// <summary>
    /// MasterClient deal bài ban đầu cho tất cả các player.
    /// Giả sử gameState đã chia bài ban đầu cho từng player (PlayerHand của local, OpponentHand của others).
    /// Trong ví dụ này, chúng ta deal cho local player bằng cách gửi RPC riêng.
    /// </summary>
    void DealInitialCards()
    {
        // Lấy danh sách người chơi theo thứ tự (chọn theo ActorNumber sắp tăng)
        List<Player> orderedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToList();
        int initialCardCount = 7;

        // Với mỗi player, deal số lá bài ban đầu
        for (int i = 0; i < orderedPlayers.Count; i++)
        {
            for (int j = 0; j < initialCardCount; j++)
            {
                CardData card = DeckManager.Instance.DrawCard();
                if (card != null)
                {
                    // Nếu người chơi đang xử lý là chính client hiện tại, spawn bài trực tiếp
                    if (orderedPlayers[i].Equals(PhotonNetwork.LocalPlayer))
                    {
                        // Ở mỗi client, PlayerHandManager ở script cục bộ sẽ được dùng để hiển thị tay bài
                        PlayerHandManager.Instance.SpawnCard(card);
                    }
                    else
                    {
                        // Gửi RPC để đối phương tự nhận bài
                        PhotonView.Get(this).RPC("RPC_GiveInitialCard", RpcTarget.All, card.cardColor, card.cardNumber, orderedPlayers[i].ActorNumber);
                    }
                }
            }
        }

        // Sau khi chia bài, thiết lập lượt chơi ban đầu. 
        // Giả sử bạn muốn người chơi có ActorNumber nhỏ nhất bắt đầu.
        int firstActor = orderedPlayers.First().ActorNumber;
        gameState.CurrentTurnActor = firstActor;
        PhotonView.Get(this).RPC("RPC_UpdateTurn", RpcTarget.All, firstActor);
    }


    /// <summary>
    /// RPC gửi lá bài ban đầu cho đối thủ
    /// </summary>
    /// <param name="cardColor"></param>
    /// <param name="cardNumber"></param>

    [PunRPC]
    
    void RPC_GiveInitialCard(string cardColor, int cardNumber, int receiverActorNumber)
    {
        Debug.Log($"RPC_GiveInitialCard: Local Actor {PhotonNetwork.LocalPlayer.ActorNumber} - Receiver {receiverActorNumber}");
        // Kiểm tra nếu ActorNumber của client hiện tại khớp với người nhận thì spawn bài
        if (PhotonNetwork.LocalPlayer.ActorNumber == receiverActorNumber)
        {
            CardData cardData = FindCardData(cardColor, cardNumber);
            if (cardData != null)
            {
                // Giả sử mỗi client đều có một instance của PlayerHandManager để hiển thị tay bài của chính nó
                PlayerHandManager.Instance.SpawnCard(cardData);
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy CardData cho {cardColor} {cardNumber}");
            }
        }
    }



    /// <summary>
    /// Khi người chơi muốn đánh bài, họ sẽ gọi RPC_PlayCard từ CardClickHandler.
    /// MasterClient sẽ xử lý logic (xác nhận nước đi, chuyển lượt, cập nhật game state) và gửi thông tin về cho tất cả client.
    /// </summary>
    /// <param name="cardColor"></param>
    /// <param name="cardNumber"></param>
    /// <param name="actorNumber"></param>
    [PunRPC]
    public void RPC_PlayCard(string cardColor, int cardNumber, int actorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CardData playedCard = FindCardData(cardColor, cardNumber);
        if (playedCard == null)
        {
            Debug.LogWarning("MasterClient: Không tìm thấy CardData cho nước đi đánh.");
            return;
        }

        // Kiểm tra tính hợp lệ của nước đi (ví dụ, so sánh với lá cuối của discard pile)
        if (!IsValidPlay(playedCard))
        {
            Debug.Log("MasterClient: Nước đi không hợp lệ.");
            return;
        }

        // Nếu hợp lệ, cập nhật GameState:
        // Giả sử chúng ta có các phương thức để xoá card khỏi tay của player có ActorNumber tương ứng.
        gameState.RemoveCardFromPlayer(actorNumber, playedCard);
        gameState.DiscardPile.Add(playedCard);

        // Gửi RPC xác nhận cho tất cả client: thông tin lá bài vừa chơi
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("RPC_ConfirmPlayCard", RpcTarget.All, cardColor, cardNumber, actorNumber);

        // Chuyển lượt chơi (ví dụ, gọi SwitchTurn trong gameState)
        int newTurnActor = SwitchTurn(playedCard);
        photonView.RPC("RPC_UpdateTurn", RpcTarget.All, newTurnActor);
    }

    /// <summary>
    /// RPC xác nhận lá bài vừa đánh cho tất cả các client để cập nhật UI.
    /// </summary>
    [PunRPC]
    void RPC_ConfirmPlayCard(string cardColor, int cardNumber, int actorNumber, PhotonMessageInfo info)
    {
        CardData playedCard = FindCardData(cardColor, cardNumber);
        if (playedCard == null) return;

        // Nếu client là người chơi đánh bài, loại bỏ card khỏi tay (ví dụ, PlayerHandManager)
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            PlayerHandManager.Instance.RemoveCardByData(playedCard);
        }
        // Tất cả client cập nhật discard pile UI
        playedCardsManager.AddPlayedCard(playedCard);
    }

    /// <summary>
    /// RPC cập nhật lượt chơi trên giao diện của tất cả các client.
    /// </summary>
    [PunRPC]
    void RPC_UpdateTurn(int newTurnActor, PhotonMessageInfo info)
    {
        // Cập nhật UI turn indicator (nếu có) hoặc lưu trạng thái lượt trong GameState.
        // Ví dụ:
        gameState.CurrentTurnActor = newTurnActor;
        if (turnHighlighter != null)
        {
            turnHighlighter.SetTurn(newTurnActor);
        }
    }

    // -------------------------------
    // Helper Functions
    // -------------------------------

    /// <summary>
    /// Tìm CardData từ thông tin cardColor và cardNumber bằng Resources.LoadAll
    /// </summary>
    CardData FindCardData(string cardColor, int cardNumber)
    {
        CardData[] allCards = Resources.LoadAll<CardData>("Cards");
        foreach (CardData card in allCards)
        {
            if (card.cardColor == cardColor && card.cardNumber == cardNumber)
                return card;
        }
        return null;
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của nước đi.
    /// Giả sử rằng lá cuối của discard pile được lưu trong gameState.DiscardPile.Last()
    /// </summary>
    bool IsValidPlay(CardData card)
    {
        if (gameState.DiscardPile == null || gameState.DiscardPile.Count == 0)
            return true;
        CardData lastCard = gameState.DiscardPile.Last();
        // Luật chơi: Nếu card là Wild, hoặc màu hoặc số trùng với lá trên discard pile.
        return (card.cardColor == "W") || (card.cardColor == lastCard.cardColor) || (card.cardNumber == lastCard.cardNumber);
    }

    /// <summary>
    /// Thực hiện chuyển lượt chơi dựa trên lá bài vừa đánh.
    /// Bạn có thể xử lý các lá bài đặc biệt (skip, reverse, +2, +4) tại đây.
    /// Trả về ActorNumber của người chơi có lượt mới.
    /// </summary>
    int SwitchTurn(CardData playedCard)
    {
        // Ví dụ đơn giản: chuyển lượt cho player kế tiếp theo thứ tự trong PhotonNetwork.PlayerList
        List<Player> orderedPlayers = new List<Player>(PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber));
        int currentIndex = orderedPlayers.FindIndex(p => p.ActorNumber == gameState.CurrentTurnActor);
        // Nếu chưa có thông tin lượt, mặc định người chơi đầu tiên
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }
        int nextIndex = (currentIndex + 1) % orderedPlayers.Count;
        gameState.CurrentTurnActor = orderedPlayers[nextIndex].ActorNumber;
        return gameState.CurrentTurnActor;
    }

    /// <summary>
    /// Ví dụ hàm để lấy ActorNumber của người chơi có lượt hiện tại.
    /// </summary>
    public int GetCurrentTurnActor()
    {
        // Nếu GameState chưa có, mặc định local player
        if (gameState == null)
            return PhotonNetwork.LocalPlayer.ActorNumber;
        return gameState.CurrentTurnActor;
    }
}
