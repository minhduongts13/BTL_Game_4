// using UnityEngine;
// using System.Collections.Generic;

// public class GameSetupManager : MonoBehaviour
// {
//     [Header("Setup")]
//     // public List<OpponentData> opponentsData; // ← Dữ liệu mỗi đối thủ riêng

//     [Header("References")]
//     public PlayerHandManager playerHandManager;
//     void Awake()
//     {
        
//     }
//     void Start()
//     {
//         DealInitialCards();
//     }

//     void DealInitialCards()
//     {
//         for (int i = 0; i < 7; i++)
//         {
//             var card = DeckManager.Instance.DrawCard();
//             if (card != null) 
//             {
//                 playerHandManager.SpawnCard(card);
//             }
//         }
//         // Sau khi chia bài xong, cập nhật số bài của local player
//         playerHandManager.UpdateLocalCardCount();
//     }

// }
