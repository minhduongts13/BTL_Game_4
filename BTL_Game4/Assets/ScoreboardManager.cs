using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInfo
{
    public string playerName;
    public int cardCount;
}

public class ScoreboardManager : MonoBehaviour
{
    // Dữ liệu mẫu người chơi
    List<PlayerInfo> players = new List<PlayerInfo>();

    // Tham chiếu đến đối tượng cha mà bạn muốn tạo bảng xếp hạng (có sẵn Canvas hoặc Panel)
    public Transform parentTransform;

    // Biến chứa đối tượng Panel của bảng xếp hạng được tạo bằng code
    GameObject scoreboardPanel;

    void Start()
    {
        // Nếu bạn chưa gán parentTransform thì tìm Canvas trong scene
        if (parentTransform == null)
        {
            Canvas foundCanvas = FindAnyObjectByType<Canvas>();
            if (foundCanvas != null)
            {
                parentTransform = foundCanvas.transform;
            }
            else
            {
                Debug.LogError("Không tìm thấy Canvas trong scene!");
                return;
            }
        }

        // Tạo Panel cho bảng xếp hạng dưới Canvas đã có
        CreateScoreboardPanel();

        // Thêm dữ liệu mẫu
        players.Add(new PlayerInfo { playerName = "Player1", cardCount = 2 });
        players.Add(new PlayerInfo { playerName = "Player2", cardCount = 3 });
        players.Add(new PlayerInfo { playerName = "Player3", cardCount = 2 });
        players.Add(new PlayerInfo { playerName = "Player4", cardCount = 1 });

        // Cập nhật bảng xếp hạng
        UpdateScoreboard();
    }

    // Bước 1: Tạo Panel cho bảng xếp hạng trên Canvas đã có
    void CreateScoreboardPanel()
    {
        scoreboardPanel = new GameObject("ScoreboardPanel");
        // Đặt làm con của parentTransform (Canvas hoặc Panel bạn đã chọn)
        scoreboardPanel.transform.SetParent(parentTransform, false);

        // Thêm RectTransform và cấu hình kích thước, vị trí
        RectTransform rt = scoreboardPanel.AddComponent<RectTransform>();
        // Căn giữa panel với kích thước 400 x 600 (có thể thay đổi theo ý bạn)
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(400, 600);
        rt.anchoredPosition = Vector2.zero;

        // Thêm Image để xem rõ ràng nền của panel (màu xám nhạt)
        Image bg = scoreboardPanel.AddComponent<Image>();
        bg.color = new Color(0.9f, 0.9f, 0.9f, 0.9f);

        // Thêm Vertical Layout Group để sắp xếp các entry theo chiều dọc
        VerticalLayoutGroup layout = scoreboardPanel.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5;
        layout.padding = new RectOffset(10, 10, 10, 10);
    }

    // Bước 2: Cập nhật bảng xếp hạng, sắp xếp theo số lá (cardCount)
    // Nếu số lá của 2 người chơi giống nhau, họ sẽ có cùng thứ hạng
    void UpdateScoreboard()
    {
        // Sắp xếp theo số lá tăng dần (ít lá hơn đứng cao hơn)
        players.Sort((a, b) => a.cardCount.CompareTo(b.cardCount));

        // Xóa các entry cũ nếu có trong scoreboardPanel
        foreach (Transform child in scoreboardPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Tạo entry mới với thứ hạng tính tie: nếu cardCount bằng nhau thì cùng thứ hạng
        int displayedRank = 1;  // Thứ hạng hiển thị
        int prevCardCount = -1; // Số lá của entry trước đó, dùng để so sánh
        for (int i = 0; i < players.Count; i++)
        {
            if (i == 0)
            {
                displayedRank = 1;
            }
            else
            {
                if (players[i].cardCount != prevCardCount)
                {
                    displayedRank = i + 1;
                }
                // Nếu bằng nhau, giữ lại thứ hạng của entry trước đó
            }
            prevCardCount = players[i].cardCount;

            CreateScoreEntry(displayedRank, players[i].playerName, players[i].cardCount);
        }
    }

    // Bước 3: Hàm tạo 1 entry của bảng xếp hạng
    void CreateScoreEntry(int rank, string playerName, int cardCount)
    {
        // Tạo một GameObject cho entry
        GameObject entry = new GameObject("ScoreEntry");
        entry.transform.SetParent(scoreboardPanel.transform, false);

        // Sử dụng LayoutElement để quy định chiều cao (ví dụ 50)
        LayoutElement layoutElement = entry.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 50;

        // Thêm Horizontal Layout Group để sắp xếp các Text theo hàng ngang
        HorizontalLayoutGroup hLayout = entry.AddComponent<HorizontalLayoutGroup>();
        hLayout.childControlHeight = true;
        hLayout.childControlWidth = false;
        hLayout.childForceExpandWidth = false;
        hLayout.spacing = 10;

        // Tạo Text cho Rank
        GameObject rankTextObj = CreateTextObject("RankText", rank.ToString());
        rankTextObj.transform.SetParent(entry.transform, false);

        // Tạo Text cho Tên người chơi
        GameObject nameTextObj = CreateTextObject("NameText", playerName);
        nameTextObj.transform.SetParent(entry.transform, false);

        // Tạo Text cho số lá bài
        GameObject cardTextObj = CreateTextObject("CardText", cardCount.ToString() + " cards");
        cardTextObj.transform.SetParent(entry.transform, false);
    }

    // Bước 4: Hàm tạo đối tượng Text cơ bản với một số thiết lập
    GameObject CreateTextObject(string name, string textContent)
    {
        GameObject textObj = new GameObject(name);
        Text text = textObj.AddComponent<Text>();
        text.text = textContent;
        // Sử dụng font Arial tích hợp sẵn của Unity
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.color = Color.black;
        // Sử dụng ContentSizeFitter để Text tự điều chỉnh kích thước theo nội dung
        ContentSizeFitter csf = textObj.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return textObj;
    }
}
