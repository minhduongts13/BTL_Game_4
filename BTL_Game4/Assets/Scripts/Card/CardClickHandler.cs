using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler
{
    public CardAnimationController cardAnimationController; // Tham chiếu tới script animation của bạn
    public PlayerHandManager playerHandManager;             // Tham chiếu tới PlayerHandManager
    public PlayedCardsManager playedCardsManager;  // Gán trong Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        CardDisplay cardDisplay = GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.cardData == null) return;

        // Sử dụng chính cardDisplay.gameObject chứ không phải so sánh cardData
        if (cardAnimationController != null)
        {
            cardAnimationController.AnimateCard(() =>
            {
                playerHandManager.RemoveCardObject(cardDisplay.gameObject);
                playedCardsManager.AddPlayedCard(cardDisplay.cardData);
            });
        }
        else
        {
            playerHandManager.RemoveCardObject(cardDisplay.gameObject);
            playedCardsManager.AddPlayedCard(cardDisplay.cardData);
        }
    }



}
