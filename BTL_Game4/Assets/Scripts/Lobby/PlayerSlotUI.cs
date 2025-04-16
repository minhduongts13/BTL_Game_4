using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class PlayerSlotUI : MonoBehaviour
{
    public Text playerNameText;
    public Button readyButton;
    public void SetPlayerInfo(string name, Button button)
    {
        playerNameText.text = name;
        readyButton = button;
        // Bạn có thể cập nhật avatar hoặc trạng thái ready tại đây nếu có
    }

    public void UpdateReadyUI(bool isReady)
    {
        if (readyButton != null)
        {
            Image btnImg = readyButton.GetComponent<Image>();
            if (isReady)
            {
                // Nền vàng, chữ trắng, text = Ready
                btnImg.color = Color.yellow;
            }
            else
            {
                // Nền xám, chữ đen, text = Not Ready
                btnImg.color = Color.gray;
            }
        }
    }
    public void Clear()
    {
        if (playerNameText != null)
            playerNameText.text = "Waiting...";
        // Ẩn avatar hoặc indicator nếu muốn
    }
}
