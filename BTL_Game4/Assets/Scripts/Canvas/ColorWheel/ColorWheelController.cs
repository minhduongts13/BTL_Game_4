using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorWheelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Wheel Settings")]
    public int diameter = 256;
    public Image targetImage;
    public CanvasGroup canvasGroup;

    [Header("Highlight Settings")]
    [Range(0f, 1f)]
    public float dimFactor = 0.3f;

    [Header("Animation Settings")]
    public float hoverScale = 1.1f;
    public float animDuration = 0.2f;
    public float openFadeDuration = 0.3f;
    public float closeFadeDuration = 0.3f;

    [Header("Callbacks")]
    public Action<Color> OnColorSelected;

    private Vector3 initialScale;
    private Texture2D wheelTexture;
    private Color[] originalColors;
    private int[] quadrantMap;
    private bool isHiding = false; // **MỚI**

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        initialScale = targetImage.rectTransform.localScale;
        GenerateColorWheel();
        HideWheelImmediate();
    }

    void GenerateColorWheel()
    {
        wheelTexture = new Texture2D(diameter, diameter, TextureFormat.ARGB32, false);
        wheelTexture.filterMode = FilterMode.Bilinear;
        originalColors = new Color[diameter * diameter];
        quadrantMap = new int[diameter * diameter];

        Vector2 center = new Vector2(diameter / 2f, diameter / 2f);
        float radius = diameter / 2f;

        for (int y = 0; y < diameter; y++)
            for (int x = 0; x < diameter; x++)
            {
                int idx = x + y * diameter;
                Vector2 pos = new Vector2(x, y);
                float dist = Vector2.Distance(pos, center);
                if (dist <= radius)
                {
                    Vector2 dir = (pos - center).normalized;
                    float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 360f) % 360f;
                    Color c; int q;
                    if (angle < 45f || angle >= 315f) { c = Color.yellow; q = 0; }
                    else if (angle < 135f) { c = Color.red; q = 1; }
                    else if (angle < 225f) { c = Color.green; q = 2; }
                    else { c = Color.blue; q = 3; }
                    originalColors[idx] = c;
                    quadrantMap[idx] = q;
                }
                else
                {
                    originalColors[idx] = new Color(0, 0, 0, 0);
                    quadrantMap[idx] = -1;
                }
            }

        wheelTexture.SetPixels(originalColors);
        wheelTexture.Apply();
        targetImage.sprite = Sprite.Create(wheelTexture, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f));
    }

    void HighlightQuadrant(int selected)
    {
        Color[] newColors = new Color[originalColors.Length];
        for (int i = 0; i < originalColors.Length; i++)
        {
            int q = quadrantMap[i];
            if (q == -1)
                newColors[i] = new Color(0, 0, 0, 0);
            else if (q == selected)
                newColors[i] = originalColors[i];
            else
            {
                Color c = originalColors[i];
                c.r *= dimFactor; c.g *= dimFactor; c.b *= dimFactor;
                newColors[i] = new Color(c.r, c.g, c.b, c.a);
            }
        }
        wheelTexture.SetPixels(newColors);
        wheelTexture.Apply();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHiding) return; // **MỚI**
        StopAllCoroutines();
        StartCoroutine(ScaleTo(initialScale * hoverScale, animDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHiding) return; // **MỚI**
        StopAllCoroutines();
        StartCoroutine(ScaleTo(initialScale, animDuration));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHiding) return; // **MỚI**

        RectTransform rect = targetImage.rectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out Vector2 localPos))
        {
            float angle = (Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg + 360f) % 360f;
            int selectedQ; string colorName; Color selectedColor;
            if (angle < 45f || angle >= 315f) { selectedQ = 0; selectedColor = Color.yellow; colorName = "Yellow"; }
            else if (angle < 135f) { selectedQ = 1; selectedColor = Color.red; colorName = "Red"; }
            else if (angle < 225f) { selectedQ = 2; selectedColor = Color.green; colorName = "Green"; }
            else { selectedQ = 3; selectedColor = Color.blue; colorName = "Blue"; }

            HighlightQuadrant(selectedQ);
            Debug.Log("Selected color: " + colorName);

            // **MỚI**: ngăn pointer event khác làm gián đoạn
            isHiding = true;
            targetImage.raycastTarget = false;

            StopAllCoroutines();
            StartCoroutine(ClickAnimation(() =>
            {
                OnColorSelected?.Invoke(selectedColor);
                HideWheel();
            }));
        }
    }

    IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 start = targetImage.rectTransform.localScale;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            targetImage.rectTransform.localScale = Vector3.Lerp(start, targetScale, t / duration);
            yield return null;
        }
        targetImage.rectTransform.localScale = targetScale;
    }

    IEnumerator ClickAnimation(Action onComplete)
    {
        Vector3 current = targetImage.rectTransform.localScale;
        Vector3 clickedScale = current * 0.8f;
        float t = 0;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            targetImage.rectTransform.localScale = Vector3.Lerp(current, clickedScale, t / animDuration);
            yield return null;
        }
        t = 0;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            targetImage.rectTransform.localScale = Vector3.Lerp(clickedScale, initialScale, t / animDuration);
            yield return null;
        }
        onComplete?.Invoke();
    }

    public void ShowWheel()
    {
        isHiding = false; // **MỚI**
        targetImage.raycastTarget = true; // **MỚI**

        if (wheelTexture != null && originalColors != null)
        {
            wheelTexture.SetPixels(originalColors);
            wheelTexture.Apply();
        }

        gameObject.SetActive(true);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            StopAllCoroutines();
            StartCoroutine(FadeTo(1, openFadeDuration));
        }
        targetImage.rectTransform.localScale = initialScale;
    }

    public void HideWheel()
    {
        if (canvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutAndDeactivate(closeFadeDuration));
        }
        else gameObject.SetActive(false);
    }

    void HideWheelImmediate()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float start = canvasGroup.alpha, t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    IEnumerator FadeOutAndDeactivate(float duration)
    {
        yield return FadeTo(0, duration);
        gameObject.SetActive(false);
    }
}
