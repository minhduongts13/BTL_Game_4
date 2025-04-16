using UnityEngine;
using UnityEngine.UI;

public class RankingEntry : MonoBehaviour
{
    public Text rankText;   // Hiển thị thứ hạng
    public Text nameText;   // Hiển thị tên người chơi
    public Text cardText;   // Hiển thị số lá bài còn lại

    // Phương thức thiết lập dữ liệu cho dòng BXH
    public void Setup(int rank, string playerName, int cardCount)
    {
        rankText.text = rank.ToString();
        nameText.text = playerName;
        cardText.text = cardCount.ToString();
    }
}
