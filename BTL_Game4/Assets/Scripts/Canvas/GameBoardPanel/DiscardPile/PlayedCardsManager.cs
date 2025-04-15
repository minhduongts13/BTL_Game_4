using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayedCardsManager : MonoBehaviour
{
    public Transform playedCardsPanel; // Nơi hiện lá bài đã đánh
    public GameObject cardPrefab;      // Prefab của lá bài
    public int maxPlayedCards = 5;

    private List<CardData> playedCards = new List<CardData>();

    public void AddPlayedCard(CardData cardData)
    {
        // Clone prefab
        GameObject newCard = Instantiate(cardPrefab);
        newCard.transform.SetParent(playedCardsPanel, false); // giữ local pos/scale
        newCard.transform.localScale = Vector3.one;           // đảm bảo không bị sai scale

        // Set dữ liệu card
        CardDisplay display = newCard.GetComponent<CardDisplay>();
        display.SetCard(cardData);

        // Thêm vào list
        playedCards.Add(cardData);

        // Xóa lá bài cũ nếu quá max
        if (playedCards.Count > maxPlayedCards)
        {
            playedCards.RemoveAt(0);
            Destroy(playedCardsPanel.GetChild(0).gameObject);
        }

        // Đảm bảo lá mới nằm trên cùng
        newCard.transform.SetAsLastSibling();
        RectTransform rt = newCard.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        CardClickHandler clickHandler = newCard.GetComponent<CardClickHandler>();
        if (clickHandler != null)
            Destroy(clickHandler);

        // // Bắt layout rebuild nếu có layout group
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)playedCardsPanel);
    }

}
