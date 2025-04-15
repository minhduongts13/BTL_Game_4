using UnityEngine;
using UnityEngine.UI;

public class CardDrawer : MonoBehaviour
{
    public PlayerHandManager playerHandManager;
    public Text deckCountText;
    // Hàm này sẽ gọi qua DeckManager để rút bài
    public void DrawCard()
    {
        // Gọi DrawCard từ DeckManager đã load và xáo bài từ Resources
        CardData drawnCard = DeckManager.Instance.DrawCard();
        if (drawnCard == null)
        {
            Debug.LogWarning("Deck trống, không có bài để rút.");
            return;
        }

        // Thêm bài vào tay người chơi và hiển thị qua SpawnCard của PlayerHandManager
        playerHandManager.playerCards.Add(drawnCard);
        playerHandManager.SpawnCard(drawnCard);

        deckCountText.text = DeckManager.Instance.RemainingCardCount().ToString();

        Debug.Log("Đã rút bài: " + drawnCard.name);
    }
}
