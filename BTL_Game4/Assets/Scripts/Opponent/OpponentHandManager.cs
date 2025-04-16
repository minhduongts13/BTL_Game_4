using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpponentHandManager : HandManagerBase  // ← thêm MonoBehaviour
{
    //public List<CardData> playerCards;    // Gán trong Inspector
    public GameObject cardBackPrefab;         // Prefab của lá bài
    public Transform opponentHandPanel;           // Vị trí/parent chứa các lá bài
    public DynamicHandLayout dynamicHandLayout; // Gán script DynamicHandLayout qua Inspector
    public int cardCount = 0; // Số lượng lá bài hiện có
    public OpponentUI opponentUI;  // Tham chiếu đến component OpponentUI trong prefab
    public OpponentPosition position;

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
    public override void SpawnCard(CardData cardData)
    {
        GameObject cardGO = Instantiate(cardBackPrefab, opponentHandPanel);
        CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
        //cardData = cardDisplay.cardData;
        cardDisplay.SetCard(cardData);
        playerCards.Add(cardData);

        cardCount++;
        if (opponentUI != null)
        {
            opponentUI.SetCardCount(cardCount);
        }
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
        if (cardCount > 0) cardCount--;
        if (opponentUI != null)
        {
            opponentUI.SetCardCount(cardCount);
        }
    }

    public void RemoveCardObject(CardData cardData)
    {
        // Giả sử các CardDisplay được lưu dưới dạng các con (child) của hand container
        // Duyệt qua tất cả các đối tượng con để tìm CardDisplay có cardData phù hợp
        CardDisplay targetCardDisplay = null;
        foreach (Transform child in transform)  // Hoặc dùng transform chứa các card display của tay
        {
            CardDisplay cd = child.GetComponent<CardDisplay>();
            if (cd != null && cd.cardData == cardData)
            {
                targetCardDisplay = cd;
                break;
            }
        }

        // Nếu tìm được, gọi hàm gốc để xử lý việc xoá
        if (targetCardDisplay != null)
        {
            RemoveCardObject(targetCardDisplay.gameObject);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy CardDisplay phù hợp với CardData cần xoá!");
        }
    }




}
