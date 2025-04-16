using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }
    private List<CardData> deck;
    //public Text cardcount;

    void Awake()
    {
        Instance = this;
        LoadDeckFromResources();
        ShuffleDeck();
    }

    // Load tất cả CardData từ folder Resources/Cards
    void LoadDeckFromResources()
    {

        CardData[] cards = Resources.LoadAll<CardData>("Cards");
        deck = new List<CardData>();

        foreach (CardData card in cards)
        {
            if (card.cardNumber != (-99)) // <- Tên phải đúng với asset
            {
                deck.Add(card);
            }
            else
            {
                Debug.Log("Skipped BackCard.asset from deck.");
            }
        }

        Debug.Log("Loaded " + deck.Count + " cards (excluding BackCard)");

    }

    // Xáo bài đơn giản
    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    // Rút bài từ bộ bài
    public CardData DrawCard()
    {
        CardData card = deck[0];
        deck.RemoveAt(0);
        if (deck.Count == 0)
        {
            Debug.LogWarning("Game kết thúc");

            return null;
        }
        //cardcount = RemainingCardCount().ToString();
        return card;
    }
    public int RemainingCardCount()
    {
        return deck.Count; // hoặc danh sách bạn đang dùng để chứa bài
    }

    public void AddDeck(CardData card)
    {
        deck.Add(card);
    }

    public List<CardData> GetDeckCopy()
    {
        // Trả về một bản copy của danh sách bài hiện có
        return new List<CardData>(deck);
    }
}
