using UnityEngine;
using UnityEngine.UI;

public class DynamicHandLayout : MonoBehaviour
{
    public HorizontalLayoutGroup layoutGroup;   // Gán từ Inspector
    public RectTransform handPanelRect;           // Gán từ Inspector, là RectTransform của vùng chứa các card
    public float cardWidth = 100f;                // Chiều rộng của card (ước tính)
    private int lastCardCount = 0;
    public float minSpacing = -250f;             // Giới hạn spacing tối thiểu
    public float maxSpacing = -50f;              // Giới hạn spacing tối đa

    void Update()
    {
        // Nếu số lượng card thay đổi, cập nhật spacing
        int currentCount = handPanelRect.childCount;
        if (currentCount != lastCardCount)
        {
            Debug.Log("Child count thay đổi: " + currentCount);
            AdjustSpacing(currentCount);
            lastCardCount = currentCount;
        }
    }

    // Hàm tính toán spacing dựa vào số card hiện có và kích thước panel
    public void AdjustSpacing(int cardCount)
    {
        if (cardCount <= 1)
        {
            layoutGroup.spacing = 0;
            return;
        }

        // Lấy chiều rộng của panel
        float panelWidth = handPanelRect.rect.width;
        // Tính tổng chiều rộng của các card không có spacing
        float totalCardsWidth = cardCount * cardWidth;
        // Tính khoảng cách cần thiết; giá trị này có thể âm để các card chồng lên nhau
        float spacing = (panelWidth - 200 - totalCardsWidth) / (cardCount - 1);

        if (spacing > maxSpacing) spacing = maxSpacing;
        else if (spacing < minSpacing) spacing = minSpacing;
        layoutGroup.spacing = spacing;
        ///Debug.Log("Cập nhật spacing: " + spacing);
        // Debug.Log("Cập nhật panelWidth: " + panelWidth);
        // Debug.Log("Cập nhật totalCardsWidth: " + totalCardsWidth);
    }

    // public void UpdateSpacing()
    // {
    //     AdjustSpacing(handPanelRect.childCount);
    // }

}
