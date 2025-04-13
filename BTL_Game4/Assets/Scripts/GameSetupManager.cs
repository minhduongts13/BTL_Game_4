using UnityEngine;
using System.Collections.Generic;

public class GameSetupManager : MonoBehaviour
{
    [Header("Setup")]
    public List<OpponentData> opponentsData; // ← Dữ liệu mỗi đối thủ riêng

    [Header("References")]
    public PlayerHandManager playerHandManager;
    public GameObject opponentHandPrefab;
    public Transform opponentsParent;

    public List<OpponentHandManager> opponentManagers = new();

    void Start()
    {
        CreateOpponents();
        DealInitialCards();
    }

    void CreateOpponents()
    {
        foreach (var data in opponentsData)
        {
            GameObject newOpponent = Instantiate(opponentHandPrefab, opponentsParent);
            OpponentHandManager manager = newOpponent.GetComponent<OpponentHandManager>();
            OpponentUI ui = newOpponent.GetComponent<OpponentUI>(); // script xử lý tên/avatar nếu bạn có

            if (ui != null)
                ui.Setup(data); // truyền dữ liệu riêng vào UI

            opponentManagers.Add(manager);
        }
    }

    void DealInitialCards()
    {
        for (int i = 0; i < 7; i++)
        {
            var card = DeckManager.Instance.DrawCard();
            if (card != null) playerHandManager.SpawnCard(card);
        }

        foreach (var opponent in opponentManagers)
        {
            for (int i = 0; i < 7; i++)
            {
                var card = DeckManager.Instance.DrawCard();
                if (card != null) opponent.SpawnCard(card);
            }
        }
    }
}
