using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;    // Tham chiếu tới component Image của lá bài
    public Text cardText;      // (Nếu hiển thị số hoặc thông tin khác)
    public CardData cardData;  // Dữ liệu của lá bài

    // Hàm để cập nhật giao diện khi có dữ liệu
    public void SetCard(CardData data)
    {
        cardData = data;
        cardImage.sprite = data.cardSprite;  // Gán hình ảnh từ CardData
        cardText.text = data.cardColor + " " + data.cardNumber.ToString();
        Debug.Log("Card Display Set: " + cardText.text);
    }
}
