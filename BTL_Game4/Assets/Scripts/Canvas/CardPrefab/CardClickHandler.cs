using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler
{
    public CardAnimationController cardAnimationController; // Tham chiếu tới script animation của bạn
    public PlayerHandManager playerHandManager;             // Tham chiếu tới PlayerHandManager
    public PlayedCardsManager playedCardsManager;  // Gán trong Inspector
    public ColorWheelController colorWheelController;

    public void OnPointerClick(PointerEventData eventData)
    {
        CardDisplay cardDisplay = GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.cardData == null) return;
        if (!GameSetupManager.Instance.isValidCard(cardDisplay.cardData)) return;
        // Sử dụng chính cardDisplay.gameObject chứ không phải so sánh cardData
        if (cardAnimationController != null)
        {
            cardAnimationController.AnimateCard(() =>
            {
                playerHandManager.RemoveCardObject(cardDisplay.gameObject);
                playedCardsManager.AddPlayedCard(cardDisplay.cardData);
                if (cardDisplay.cardData.cardNumber == -13 || cardDisplay.cardData.cardNumber == -14)
                {
                    colorWheelController.ShowWheel(cardDisplay.cardData);
                    Debug.Log("hahahahah");
                }
                GameSetupManager.Instance.SwitchTurn("discard", playedCardsManager.playedCards[playedCardsManager.playedCards.Count - 1]);
            });
        }
        else
        {
            playerHandManager.RemoveCardObject(cardDisplay.gameObject);
            playedCardsManager.AddPlayedCard(cardDisplay.cardData);
            if (cardDisplay.cardData.cardNumber == -13 || cardDisplay.cardData.cardNumber == -14)
            {
                colorWheelController.ShowWheel(cardDisplay.cardData);
            }
            GameSetupManager.Instance.SwitchTurn("discard", playedCardsManager.playedCards[playedCardsManager.playedCards.Count - 1]);
        }
    }



}
