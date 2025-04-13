using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardAnimationController : MonoBehaviour
{
    // Tốc độ animation có thể điều chỉnh qua Inspector
    public float enlargeDuration = 0.5f;  // Thời gian phóng to đến giữa màn hình
    public float slamDuration = 0.3f;     // Thời gian đập xuống cuối cùng
    public float enlargeScaleMultiplier = 1.5f;  // Mức phóng to (1.5x)
    public float slamScaleMultiplier = 0.5f;     // Mức scale cuối khi đập xuống (nhỏ hơn để tạo hiệu ứng nén)

    // HÀM KHỞI ĐỘNG ANIMATION – gọi hàm này khi click (hoặc từ OnClick event)
    public void AnimateCard(System.Action onAnimationComplete)
    {
        RectTransform cardRect = GetComponent<RectTransform>();
        if (cardRect == null) return;

        Vector3 startPosition = cardRect.position;
        Vector3 initialScale = cardRect.localScale;
        Vector3 centerPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, startPosition.z);
        Vector3 endPosition = centerPosition;

        StartCoroutine(AnimateCardRoutine(cardRect, startPosition, centerPosition, endPosition, initialScale, onAnimationComplete));
    }


    private IEnumerator AnimateCardRoutine(
    RectTransform cardRect,
    Vector3 startPos,
    Vector3 centerPos,
    Vector3 endPos,
    Vector3 initialScale,
    System.Action onComplete
)
    {
        float elapsed = 0f;
        Vector3 targetScale = initialScale * enlargeScaleMultiplier;

        while (elapsed < enlargeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / enlargeDuration);
            cardRect.position = Vector3.Lerp(startPos, centerPos, t);
            cardRect.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }

        cardRect.position = centerPos;
        cardRect.localScale = targetScale;

        elapsed = 0f;
        Vector3 slamFinalScale = targetScale * slamScaleMultiplier;
        while (elapsed < slamDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slamDuration);
            cardRect.position = Vector3.Lerp(centerPos, endPos, t);
            cardRect.localScale = Vector3.Lerp(targetScale, slamFinalScale, t);
            yield return null;
        }

        cardRect.position = endPos;
        cardRect.localScale = slamFinalScale;

        Debug.Log("Animation card hoàn tất");

        // 🔥 Gọi callback
        onComplete?.Invoke();
    }

}
