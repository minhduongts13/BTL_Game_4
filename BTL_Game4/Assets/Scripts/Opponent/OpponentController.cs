using UnityEngine;
using UnityEngine.UI;

public class OpponentUI : MonoBehaviour
{
    public Text nameText;
    public Image avatarImage;
    public Text cardCountText;     // Text hiển thị số lượng lá bài
    public RectTransform infoPanel; // Panel chứa thông tin của opponent (avatar & số bài)
    public void Setup(OpponentData data)
    {
        if (nameText != null)
            nameText.text = data.opponentName;

        if (avatarImage != null)
            avatarImage.sprite = data.avatar;
        SetCardCount(7);
    }
    // Hàm cập nhật số lượng lá bài hiển thị
    public void SetCardCount(int count)
    {
        if (cardCountText != null)
        {
            cardCountText.text = count.ToString();
        }
    }
}
