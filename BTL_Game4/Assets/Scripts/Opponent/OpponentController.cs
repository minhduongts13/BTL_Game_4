using UnityEngine;
using UnityEngine.UI;

public class OpponentUI : MonoBehaviour
{
    public Text nameText;
    public Image avatarImage;

    public void Setup(OpponentData data)
    {
        if (nameText != null)
            nameText.text = data.opponentName;

        if (avatarImage != null)
            avatarImage.sprite = data.avatar;
    }
}
