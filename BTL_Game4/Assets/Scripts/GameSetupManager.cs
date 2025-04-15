using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameSetupManager : MonoBehaviour
{
    [Header("UI")]
    public ColorWheelController colorWheel;
    public GameBoardPanel gameBoardPanel;

    [Header("Setup")]
    public List<OpponentData> opponentsData; // Dữ liệu mỗi đối thủ

    [Header("References")]
    public PlayerHandManager playerHandManager;
    public GameObject opponentHandPrefab;
    public Transform opponentsParent;
    public List<OpponentHandManager> opponentManagers = new();
    [Header("Deck UI")]
    public Text deckCountText;

    // Hàm chuyển đổi enum sang vị trí cố định (có thể điều chỉnh theo bố cục của bạn)
    Vector3 GetPositionForOpponent(OpponentPosition pos)
    {
        switch (pos)
        {
            case OpponentPosition.Top:
                return new Vector3(0, 300, 0);
            case OpponentPosition.Right:
                return new Vector3(500, 0, 0);
            case OpponentPosition.Bottom:
                return new Vector3(0, -300, 0);
            case OpponentPosition.Left:
                return new Vector3(-500, 0, 0);
            default:
                return Vector3.zero;
        }
    }

    // Tính góc xoay để đối thủ "hướng về" trung tâm (Center = (0,0,0))
    float GetRotationForPosition(OpponentPosition pos)
    {
        Vector3 posVec = GetPositionForOpponent(pos);
        Vector2 dir = (Vector2.zero - new Vector2(posVec.x, posVec.y)).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle + 90;  // Điều chỉnh nếu cần
    }

    void Awake()
    {
        CreateOpponents();
    }

    void Start()
    {
        DealInitialCards();

        if (colorWheel != null && gameBoardPanel != null)
        {
            colorWheel.OnColorSelected += (Color selectedColor) =>
            {
                gameBoardPanel.SetBackgroundColor(selectedColor);
            };
        }
    }

    void CreateOpponents()
    {
        foreach (var data in opponentsData)
        {
            Debug.Log($"Tạo đối thủ: {data.opponentName} ở vị trí: {data.fixedPosition}");

            GameObject newOpponent = Instantiate(opponentHandPrefab, opponentsParent);

            // Đặt vị trí và xoay dựa trên giá trị của enum trong OpponentData
            Vector3 targetPos = GetPositionForOpponent(data.fixedPosition);
            newOpponent.transform.localPosition = targetPos;
            newOpponent.transform.rotation = Quaternion.Euler(0, 0, GetRotationForPosition(data.fixedPosition));

            // Sử dụng GetComponentInChildren để lấy Component từ con của prefab
            OpponentHandManager manager = newOpponent.GetComponentInChildren<OpponentHandManager>();
            OpponentUI ui = newOpponent.GetComponentInChildren<OpponentUI>();

            if (ui != null)
            {
                // Gọi hàm Setup để truyền thông tin trong OpponentData
                ui.Setup(data);
                Debug.Log($"Đã setup UI cho đối thủ: {data.opponentName}");

                // Điều chỉnh vị trí infoPanel nếu cần (ví dụ: đặt sang bên trái)
                if (ui.infoPanel != null)
                {
                    RectTransform handRect = manager.opponentHandPanel as RectTransform;
                    RectTransform infoRect = ui.infoPanel; // Cái này đã là RectTransform rồi

                    if (handRect != null && infoRect != null)
                    {
                        Vector2 handPos = handRect.anchoredPosition;
                        infoRect.anchoredPosition = new Vector2(handPos.x + 250f, handPos.y + 50f);
                    }
                    else
                    {
                        Debug.LogWarning("Không thể ép kiểu sang RectTransform!");
                    }

                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy OpponentUI trên prefab!");
            }

            if (manager != null)
            {
                opponentManagers.Add(manager);
                Debug.Log($"OpponentHandManager đã được thêm cho: {data.opponentName}");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy OpponentHandManager trên prefab!");
            }
        }

        Debug.Log($"Tổng số đối thủ được tạo: {opponentManagers.Count}");
    }

    void DealInitialCards()
    {
        Debug.Log("Chia bài cho người chơi...");
        for (int i = 0; i < 7; i++)
        {
            var card = DeckManager.Instance.DrawCard();
            if (card != null)
            {
                playerHandManager.SpawnCard(card);
                // Có thể thêm Debug.Log(card.cardName) nếu cần
            }
        }

        Debug.Log("Chia bài cho đối thủ...");
        foreach (var opponent in opponentManagers)
        {
            for (int i = 0; i < 7; i++)
            {
                var card = DeckManager.Instance.DrawCard();
                if (card != null)
                {
                    opponent.SpawnCard(card);  // Đây sẽ tạo lá bài úp
                    Debug.Log("Đối thủ nhận lá bài (úp).");
                }
            }
        }
        Debug.Log("Chia bài hoàn tất.");
        UpdateDeckCount(); // ← Cập nhật số bài sau khi chia
    }

    void UpdateDeckCount()
    {
        if (deckCountText != null)
        {
            deckCountText.text = DeckManager.Instance.RemainingCardCount().ToString();
        }
    }

}
