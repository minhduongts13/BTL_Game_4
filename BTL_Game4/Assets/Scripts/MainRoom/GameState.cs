using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState
{
    // Bộ bài rút
    public List<CardData> DrawPile { get; private set; }
    // Bộ bài đã chơi
    public List<CardData> DiscardPile { get; set; }
    // ActorNumber của người chơi có lượt hiện tại
    public int CurrentTurnActor { get; set; }

    // Có thể bổ sung các dữ liệu khác (ví dụ: tay bài của các người chơi) tùy vào logic của bạn
    // Nếu bạn đồng bộ tay bài qua PlayerHandManager thì có thể không cần lưu ở đây.

    public GameState()
    {
        // Khởi tạo các danh sách
        DrawPile = CreateDeck();
        ShuffleDeck();
        DiscardPile = new List<CardData>();

        // Thiết lập lượt ban đầu, có thể đặt mặc định là -1 hoặc ActorNumber của player đầu tiên
        CurrentTurnActor = -1;
    }

    // Tạo bộ bài Uno (giả sử sử dụng các CardData đã được tạo bằng CardDataGenerator)
    private List<CardData> CreateDeck()
    {
        // Ví dụ: Load tất cả CardData từ Resources/Cards
        CardData[] allCards = Resources.LoadAll<CardData>("Cards");
        return new List<CardData>(allCards);
    }

    // Xáo bài đơn giản
    public void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        DrawPile = DrawPile.OrderBy(a => rng.Next()).ToList();
    }

    // Ví dụ một phương thức để loại bỏ một card khỏi tay của người chơi
    // Bạn có thể điều chỉnh theo cách lưu trữ tay bài của bạn
    public void RemoveCardFromPlayer(int actorNumber, CardData card)
    {
        // Logic xóa card của player có ActorNumber tương ứng
        // Nếu bạn đồng bộ tay bài qua PlayerHandManager, có thể bạn không cần thiết lập tại đây
        Debug.Log($"Xóa {card} khỏi tay của Player {actorNumber}");
    }
}
