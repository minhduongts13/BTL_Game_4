using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerHandManager : MonoBehaviour
{
    public static PlayerHandManager Instance { get; private set; }
    public List<CardData> playerCards;    // Gán trong Inspector
    public GameObject cardPrefab;         // Prefab của lá bài
    public Transform handPanel;           // Vị trí/parent chứa các lá bài
    public DynamicHandLayout dynamicHandLayout; // Gán script DynamicHandLayout qua Inspector

    // Biến lưu tên của local user
    public Text localUserName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Nếu bạn muốn duy trì đối tượng qua các scene:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateLocalCardCount();

    }

    // Phương thức để cài đặt local player (và lưu tên người chơi)
    public void SetPlayer()
    {
        localUserName.text = PhotonNetwork.NickName;
        Debug.Log("Local player set: " + localUserName.text);
        // Bạn có thể cập nhật UI hoặc lưu thêm thông tin nếu cần
    }

    // Phương thức để lấy tên local user
    

    public void SpawnCard(CardData cardData)
    {
        playerCards.Add(cardData);
        GameObject cardGO = Instantiate(cardPrefab, handPanel);
        CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
        cardDisplay.SetCard(cardData);

        // Gọi DynamicHandLayout để điều chỉnh layout nếu cần
        StartCoroutine(CallAdjustSpacing());
        UpdateLocalCardCount();
    }

    IEnumerator CallAdjustSpacing()
    {
        yield return null; // Đợi 1 frame
        dynamicHandLayout.AdjustSpacing(handPanel.childCount);
    }

    public void RemoveCardObject(GameObject cardObject)
    {
        CardDisplay cd = cardObject.GetComponent<CardDisplay>();
        if (cd != null)
        {
            if (playerCards.Contains(cd.cardData))
                playerCards.Remove(cd.cardData);

            Destroy(cardObject);
        }
        UpdateLocalCardCount();
    }

    public void RemoveCardByData(CardData cardData)
    {
        // Tìm đối tượng trong tay mà CardDisplay.cardData equals cardData
        // Duyệt qua các con của handPanel
        for (int i = handPanel.childCount - 1; i >= 0; i--)
        {
            GameObject cardGO = handPanel.GetChild(i).gameObject;
            CardDisplay cd = cardGO.GetComponent<CardDisplay>();
            if (cd != null && cd.cardData.Equals(cardData))
            {
                playerCards.Remove(cd.cardData); // xóa khỏi danh sách
                Destroy(cardGO);
                break;
            }
        }
        UpdateLocalCardCount();
    }

    public void UpdateLocalCardCount()
    {
        int count = playerCards.Count;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "CardCount", count } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
