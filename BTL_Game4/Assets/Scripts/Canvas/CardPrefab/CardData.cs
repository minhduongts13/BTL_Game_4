using UnityEngine;

public class CardData : ScriptableObject
{
    public string cardColor;     // Ví dụ: "Red", "Blue", "Green", "Yellow", "Wild"
    public int cardNumber;       // Ví dụ: 0-9 hoặc dùng giá trị đặc biệt cho các lá bài đặc biệt (như +2, +4)
    public Sprite cardSprite;    // Hình ảnh của lá bài
}
