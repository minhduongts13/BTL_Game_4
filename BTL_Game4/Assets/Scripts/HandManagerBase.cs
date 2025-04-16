using System.Collections.Generic;
using UnityEngine;

public abstract class HandManagerBase : MonoBehaviour
{
    // Ví dụ chung: mọi hand đều có list card
    //public List<CardData> Cards = new List<CardData>();
    public List<CardData> playerCards;    // Gán trong Inspector

    // Ví dụ method chung
    public abstract void SpawnCard(CardData card);
    //public abstract void RemoveCard(CardData card);
}
