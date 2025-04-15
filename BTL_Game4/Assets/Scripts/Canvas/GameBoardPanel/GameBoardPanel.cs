using UnityEngine;
using UnityEngine.UI;

public class GameBoardPanel : MonoBehaviour
{
    public Image backgroundImage;

    public void SetBackgroundColor(Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }
}
