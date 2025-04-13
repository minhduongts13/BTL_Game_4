using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }
    private List<CardData> deck;

    void Awake()
    {
        Instance = this;
        LoadDeckFromResources();
        ShuffleDeck();
    }

    // Load tất cả CardData từ folder Resources/Cards
    void LoadDeckFromResources()
    {
        // Sử dụng Resources.LoadAll để load các asset CardData từ folder "Cards"
        CardData[] cards = Resources.LoadAll<CardData>("Cards");
        deck = new List<CardData>(cards);
        Debug.Log("Loaded " + deck.Count + " cards from Resources/Cards");
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
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }
        CardData card = deck[0];
        deck.RemoveAt(0);
        return card;
    }
}
