#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawPileClickHandler : MonoBehaviour, IPointerClickHandler
{
    // Tham chiếu đến script CardDrawer (gán qua Inspector)
    public CardDrawer cardDrawer;
    public OpponentHandManager opponentHandManager;
    void Start()
    {
        //tesst opponent có thể xóa
        if (opponentHandManager == null)
        {
            GameSetupManager gm = FindAnyObjectByType<GameSetupManager>();
            if (gm != null && gm.opponentManagers.Count > 0)
            {
                opponentHandManager = gm.opponentManagers[0]; // ← Chọn đối thủ đầu tiên
            }
            if (opponentHandManager == null)
            {
                Debug.LogError("OpponentHandManager vẫn null sau khi khởi tạo.");
                Debug.Log(gm.opponentManagers.Count);
            }
        }
    }

    // Hàm xử lý sự kiện click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardDrawer != null)
        {

            cardDrawer.DrawCard();
            //Debug.Log("Đã click vào DrawPile, rút bài!");

            //Chỉ để test opponent
            // CardData drawnCard = DeckManager.Instance.DrawCard();
            // if (drawnCard == null)
            // {
            //     Debug.LogWarning("Deck trống, không có bài để rút.");
            //     return;
            // }
            // opponentHandManager.SpawnCard(drawnCard);
        }
        else
        {
            Debug.LogWarning("CardDrawer chưa được gán cho DrawPileClickHandler.");
        }
    }
}
