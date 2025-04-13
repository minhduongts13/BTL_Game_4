using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpponentHandManager : MonoBehaviour  // ← thêm MonoBehaviour
{
    public List<CardData> playerCards;    // Gán trong Inspector
    public GameObject cardBackPrefab;         // Prefab của lá bài
    public Transform opponentHandPanel;           // Vị trí/parent chứa các lá bài
    public DynamicHandLayout dynamicHandLayout; // Gán script DynamicHandLayout qua Inspector

    void Start()
    {
        // for (int i = 0; i < playerCards.Count; i++)
        // {
        //     GameObject cardGO = Instantiate(cardBackPrefab, opponentHandPanel);
        //     CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
        //     cardDisplay.SetCard(playerCards[i]);
        //     StartCoroutine(CallAdjustSpacing());
        // }
    }
    public void SpawnCard(CardData cardData)
    {
        GameObject cardGO = Instantiate(cardBackPrefab, opponentHandPanel);
        CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
        cardDisplay.SetCard(cardData);

        // Có thể gọi DynamicHandLayout để điều chỉnh layout nếu cần
        StartCoroutine(CallAdjustSpacing());
    }

    IEnumerator CallAdjustSpacing()
    {
        yield return null; // Đợi 1 frame
        dynamicHandLayout.AdjustSpacing(opponentHandPanel.childCount);
    }
    public void RemoveCardObject(GameObject cardObject)
    {
        CardDisplay cd = cardObject.GetComponent<CardDisplay>();
        if (cd != null)
        {
            // Xoá khỏi dữ liệu tay người chơi nếu cần (bạn cần đảm bảo xoá đúng instance)
            if (playerCards.Contains(cd.cardData))
                playerCards.Remove(cd.cardData);

            Destroy(cardObject);
        }
    }


}
