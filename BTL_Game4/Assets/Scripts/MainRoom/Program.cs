using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace UnoGame
{
    // Lớp Card đại diện cho một lá bài trong trò chơi Uno
    public class Card
    {
        public string Color  { get; set; } // Màu: "R" (Red), "Y" (Yellow), "B" (Blue), "G" (Green), "W" (Wild)
        public string Number { get; set; } // Số hoặc hành động: "0"-"9", "skip", "rev", "+2", "chg", "+4"

        public Card(string color, string number)
        {
            Color  = color;
            Number = number;
        }

        // Kiểm tra xem lá bài này có thể chơi được trên lá bài khác không
        public bool Matches(Card other)
        {
            return Color == "W" || Color == other.Color || Number == other.Number;
        }

        public override string ToString()
        {
            return $"{Color}{Number}";
        }

        // So sánh hai lá bài có giống nhau hoàn toàn không (dùng khi xóa lá bài khỏi tay)
        public override bool Equals(object? obj)
        {
            if (obj is Card other)
            {
                return Color == other.Color && Number == other.Number;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Number);
        }
    }

    // Lớp GameState quản lý trạng thái trò chơi
    public class GameState
    {
        public List<Card> DrawPile { get; private set; }      // Bộ bài rút
        public List<Card> DiscardPile { get; private set; }   // Bộ bài đã chơi
        public List<Card> AIHand { get; private set; }        // Tay bài của AI
        public List<Card> PlayerHand { get; private set; }    // Tay bài của người chơi
        public string Turn { get; private set; }              // Lượt hiện tại: "Player" hoặc "AI"
        public int Direction { get; private set; }            // Chiều chơi: 1 (thuận), -1 (ngược)
        public string[] Players { get; private set; }         // Danh sách người chơi
        public int NumPlayers { get; private set; }           // Số người chơi
        public string ForcedWinner { get; private set; }      // Người thắng khi hết bài rút
        public Random rng;                                   // Đối tượng Random dùng chung

        public GameState()
        {
            rng = new Random();
            List<Card> deck = CreateDeck();
            deck = deck.OrderBy(x => rng.Next()).ToList(); // Xáo bài

            // Chia 7 lá bài cho mỗi người chơi
            AIHand = new List<Card>();
            PlayerHand = new List<Card>();
            for (int i = 0; i < 7; i++)
            {
                AIHand.Add(deck[0]);
                deck.RemoveAt(0);
                PlayerHand.Add(deck[0]);
                deck.RemoveAt(0);
            }

            // Chọn lá đầu tiên cho discard pile (phải là lá số)
            Card firstCard;
            do
            {
                firstCard = deck[0];
                deck.RemoveAt(0);
                if (int.TryParse(firstCard.Number, out _))
                {
                    break;
                }
                else
                {
                    deck.Add(firstCard);
                }
            } while (true);
            DiscardPile = new List<Card> { firstCard };

            DrawPile = deck; // Bộ bài còn lại là draw pile

            Turn = "Player";
            Direction = 1;
            Players = new string[] { "Player", "AI" };
            NumPlayers = Players.Length;
            ForcedWinner = null;
        }

        // Tạo bộ bài Uno gồm 108 lá
        private List<Card> CreateDeck()
        {
            string[] colors = { "R", "Y", "B", "G" };
            List<string> numbers = new List<string> { "0" };
            for (int i = 1; i < 10; i++)
            {
                numbers.Add(i.ToString());
                numbers.Add(i.ToString()); // 1-9 xuất hiện 2 lần
            }
            string[] actions = { "skip", "rev", "+2" };
            List<string> actionCards = new List<string>();
            foreach (var action in actions)
            {
                actionCards.Add(action);
                actionCards.Add(action); // Mỗi hành động xuất hiện 2 lần
            }
            List<Card> deck = new List<Card>();
            foreach (var color in colors)
            {
                foreach (var num in numbers)
                {
                    deck.Add(new Card(color, num));
                }
                foreach (var act in actionCards)
                {
                    deck.Add(new Card(color, act));
                }
            }
            string[] wilds = { "chg", "+4" };
            for (int i = 0; i < 4; i++)
            {
                foreach (var wild in wilds)
                {
                    deck.Add(new Card("W", wild)); // Wild cards xuất hiện 4 lần mỗi loại
                }
            }
            return deck;
        }

        // Rút một lá bài cho người chơi
        public void DrawCard(string player)
        {
            if (DrawPile.Count > 0)
            {
                Card card = DrawPile[0];
                DrawPile.RemoveAt(0);
                if (player == "AI")
                {
                    AIHand.Add(card);
                }
                else
                {
                    PlayerHand.Add(card);
                }
            }
            else
            {
                int aiCount = AIHand.Count;
                int playerCount = PlayerHand.Count;
                ForcedWinner = aiCount > playerCount ? "Player" : "AI";
            }
        }

        // Chuyển lượt chơi, xử lý các lá bài đặc biệt
        public void SwitchTurn(object lastMove = null)
		{
			int currentIndex= Array.IndexOf(Players, Turn);
			int newIndex    = (currentIndex + Direction + NumPlayers) % NumPlayers;

			if (lastMove is Card lastCard)
			{
				switch (lastCard.Number)
				{
					case "skip":
						Console.WriteLine($"{Players[newIndex]} bị cấm!");
						newIndex = (newIndex + Direction + NumPlayers) % NumPlayers;
						break;
					case "rev":
						Direction *= -1;
						newIndex = (currentIndex + Direction + NumPlayers) % NumPlayers;
						if (NumPlayers == 2)
						{
							newIndex = currentIndex; // Với 2 người, "rev" giống "skip"
						}
						break;
					case "+2":
					case "+4":
						int numCards = int.Parse(lastCard.Number[1].ToString());
						for (int i = 0; i < numCards; i++)
						{
							DrawCard(Players[newIndex]);
						}
						Console.WriteLine($"{Players[newIndex]} rút {numCards} lá bài!");
						newIndex = (newIndex + Direction + NumPlayers) % NumPlayers;
						break;
				}
			}

			Turn = Players[newIndex];
			Console.WriteLine($"Đến lượt của {Turn}!");
		}


        // Kiểm tra xem trò chơi đã kết thúc chưa
        public bool IsGameOver()
        {
            return ForcedWinner != null || AIHand.Count == 0 || PlayerHand.Count == 0;
        }

        // Xác định người thắng
        public string Winner()
        {
            if (ForcedWinner != null)
                return ForcedWinner;
            return AIHand.Count == 0 ? "AI" : "Player";
        }

        // Quyết định của AI (chọn ngẫu nhiên)
        public Card? AIDecision()
        {
            List<Card> legalMoves = AIHand.Where(card => card.Matches(DiscardPile.Last())).ToList();
            if (legalMoves.Count == 0)
            {
                return null; // Nghĩa là "DRAW"
            }
            else
            {
                return legalMoves[rng.Next(legalMoves.Count)];
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameState game = new GameState();
            while (!game.IsGameOver())
            {
                Console.WriteLine($"\nDiscard Pile  : {game.DiscardPile.Last()}\n");
                Console.WriteLine("Your Hand     : " + string.Join(", ", game.PlayerHand));
                Console.WriteLine("Your Hand Size: " + game.PlayerHand.Count);
                Console.WriteLine("AI Hand Size  : " + game.AIHand.Count);

                if (game.Turn == "Player")
                {
                    List<Card> validCards = game.PlayerHand.Where(card => card.Matches(game.DiscardPile.Last())).ToList();
                    List<string> validMovesStr = validCards.Select(card => card.ToString()).ToList();
                    validMovesStr.Add("DRAW");
                    string moveStr = GetPlayerMove(validMovesStr);
                    Card chosenCard = null;

                    if (moveStr == "DRAW")
                    {
                        game.DrawCard("Player");
                        Card extraCard = game.PlayerHand.Last();
                        Console.WriteLine("Your ExtraCard: " + extraCard);
                        if (extraCard.Matches(game.DiscardPile.Last()))
                        {
                            Console.Write("Bạn có muốn chơi lá này không? (Y/N): ");
                            string res = Console.ReadLine().Trim().ToUpper();
                            if (res == "Y")
                            {
                                chosenCard = extraCard;
                            }
                        }
                    }
                    else
                    {
                        chosenCard = game.PlayerHand.First(card => card.ToString() == moveStr);
                    }

                    if (chosenCard != null)
                    {
                        if (chosenCard.Color == "W")
                        {
                            while (true)
                            {
                                Console.Write("Chọn màu (R, Y, B, G): ");
                                string color = Console.ReadLine().Trim().ToUpper();
                                if (color == "R" || color == "Y" || color == "B" || color == "G")
                                {
                                    chosenCard.Color = color;
                                    break;
                                }
                                Console.WriteLine("Màu không hợp lệ! Thử lại.");
                            }
                        }
                        game.PlayerHand.Remove(chosenCard);
                        game.DiscardPile.Add(chosenCard);
                        if (game.PlayerHand.Count == 1)
                        {
                            Console.WriteLine("\nBạn còn 1 lá! Gõ 'UNO' trong 2 giây!");
                            var stopwatch = Stopwatch.StartNew();
                            string unoInput = Console.ReadLine().Trim().ToUpper();
                            stopwatch.Stop();
                            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                            if (elapsedSeconds > 2 || unoInput != "UNO")
                            {
                                Console.WriteLine("Bạn không gõ UNO kịp! Rút 2 lá.");
                                game.DrawCard("Player");
                                game.DrawCard("Player");
                            }
                        }
                        game.SwitchTurn(chosenCard);
                    }
                    else
                    {
                        game.SwitchTurn();
                    }
                }
                else // Lượt của AI
                {
                    Card? aiMove = game.AIDecision();
                    if (aiMove == null)
                    {
                        game.DrawCard("AI");
                        Card extraCard = game.AIHand.Last();
                        if (extraCard.Matches(game.DiscardPile.Last()))
                        {
                            bool aiChoice = game.rng.Next(2) == 0; // 50% cơ hội chơi lá vừa rút
                            if (aiChoice)
                            {
                                Console.WriteLine($"AI rút lá phụ: {extraCard}");
                                aiMove = extraCard;
                            }
                        }
                    }
                    if (aiMove != null)
                    {
                        if (aiMove.Color == "W")
                        {
                            aiMove.Color = new string[] { "R", "Y", "B", "G" }[game.rng.Next(4)];
                            Console.WriteLine($"AI chọn màu: {aiMove.Color}");
                        }
                        Console.WriteLine($"AI chơi: {aiMove}");
                        game.DiscardPile.Add(aiMove);
                        game.AIHand.Remove(aiMove);
                        game.SwitchTurn(aiMove);
                    }
                    else
                    {
                        game.SwitchTurn();
                    }
                }
            }
            Console.WriteLine("Trò chơi kết thúc! Người thắng: " + game.Winner());
        }

        // Lấy nước đi của người chơi
        private static string GetPlayerMove(List<string> validMoves)
        {
            while (true)
            {
                Console.WriteLine("Các nước đi khả thi: " + string.Join(", ", validMoves));
                Console.Write("Chọn lá bài để chơi hoặc gõ 'DRAW': ");
                string input = Console.ReadLine().Trim();
                if (input == "DRAW")
                {
                    return "DRAW";
                }
                if (int.TryParse(input, out int index) && index >= 0 && index < validMoves.Count)
                {
                    return validMoves[index];
                }
                Console.WriteLine("Nước đi không hợp lệ! Thử lại.");
            }
        }
    }
}