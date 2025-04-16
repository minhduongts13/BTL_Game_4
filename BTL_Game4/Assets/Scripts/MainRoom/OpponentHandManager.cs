using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class OpponentHandManager : MonoBehaviourPunCallbacks
{
    public Text opponentNameText;
    public Image cardBackImage;     
    public Text cardCountText;      

    private Player opponentPlayer;
    private int cardCount = 0;      

    public void SetPlayer(Player player)
    {
        opponentPlayer = player;
        opponentNameText.text = player.NickName;
        UpdateCardCount(); // Lần đầu khởi tạo khi vào phòng
    }

    public void UpdateCardCount()
    {
        object countObj;
        if(opponentPlayer.CustomProperties.TryGetValue("CardCount", out countObj))
        {
            cardCount = (int)countObj;
        }
        cardCountText.text = cardCount.ToString();
    }

    // Override Photon callback để lắng nghe thay đổi properties từ người chơi đối thủ
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Kiểm tra nếu targetPlayer chính là người đại diện cho panel này
        if(opponentPlayer != null && targetPlayer.ActorNumber == opponentPlayer.ActorNumber)
        {
            if(changedProps.ContainsKey("CardCount"))
            {
                UpdateCardCount();
            }
        }
    }
}
