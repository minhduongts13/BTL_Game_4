using UnityEngine;

public class CardData : ScriptableObject
{
    public string cardColor;     // Ví dụ: "Red", "Blue", "Green", "Yellow", "Wild"
    public int cardNumber;       // Ví dụ: 0-9 hoặc dùng giá trị đặc biệt cho các lá bài đặc biệt (như +2, +4)
    public Sprite cardSprite;    // Hình ảnh của lá bài
    public string cardString;

    public CardData()
    {
        this.cardString = this.cardColor + this.cardNumber.ToString();
    }
    public CardData(string cardColor)
    {
        this.cardColor = cardColor;
        this.cardNumber = -14;
        this.cardString = this.cardColor + this.cardNumber.ToString();
    }
    public bool Matches(CardData other)
    {
        // Lá bài khớp nếu cùng màu, cùng số hoặc nếu this là Wild ("W")
        return this.cardColor == "wild" || this.cardColor == other.cardColor || this.cardNumber == other.cardNumber;
    }
}
