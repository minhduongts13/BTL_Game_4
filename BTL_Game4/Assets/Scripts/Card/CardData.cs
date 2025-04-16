using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardData : ScriptableObject
{
    public string cardColor;     // Ví dụ: "Red", "Blue", "Green", "Yellow", "Wild"
    public int   cardNumber;     // Ví dụ: 0-9 hoặc dùng giá trị đặc biệt cho các lá bài đặc biệt (như +2, +4)
    public Sprite cardSprite;    // Hình ảnh của lá bài

    Dictionary<string, int> cardDict = new Dictionary<string, int>{
        { "skip", -10 },
        { "rev" , -11 },
        { "chg" , -14 },
        { "+2"  , -12 },
        { "+4"  , -13 },
        { "0"   , 0 },
        { "1"   , 1 },
        { "2"   , 2 },
        { "3"   , 3 },
        { "4"   , 4 },
        { "5"   , 5 },
        { "6"   , 6 },
        { "7"   , 7 },
        { "8"   , 8 },
        { "9"   , 9 },
    };


    public CardData(string color, int number)
    {
        this.cardColor  = color;
        this.cardNumber = number;
    }

    public CardData(string color, string number)
    {
        this.cardColor  = color;
        this.cardNumber = int.Parse(number);
    }

    public override string ToString()
    {
        var myKey = cardDict.FirstOrDefault(x => x.Value == cardNumber).Key;
        return $"{this.cardColor}{myKey}";
    }

    // Clone method for deep copy
    public CardData Clone()
    {
        return new CardData(this.cardColor, this.cardNumber.ToString());
    }

    public bool Matches(CardData other)
    {
        // Lá bài khớp nếu cùng màu, cùng số hoặc nếu this là Wild ("W")
        return this.cardColor == "W" || this.cardColor == other.cardColor || this.cardNumber == other.cardNumber;
    }

    public override bool Equals(object obj)
    {
        if (obj is CardData other)
        {
            // The card matches if both attributes are equal or if this is Wild.
            return (this.cardColor == other.cardColor && this.cardNumber == other.cardNumber) /*|| this.cardColor == "W"*/;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (this.cardColor, this.cardNumber).GetHashCode();
    }
}
