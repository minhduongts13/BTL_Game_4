using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class ReplayButton : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetImage;  // Kéo Image con vào đây trong Inspector
    private Vector3 originalScale;

    void Start()
    {
        if (targetImage != null)
        {
            originalScale = targetImage.transform.localScale;
        }

        // Ẩn cha (Replay), kéo theo ẩn luôn con
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Hello");

        if (targetImage != null)
        {
            targetImage.transform.localScale = originalScale * 0.9f;
            Invoke("RestoreScale", 0.2f);
        }
    }

    private void RestoreScale()
    {
        if (targetImage != null)
        {
            targetImage.transform.localScale = originalScale;
        }
        SceneManager.LoadScene("Menu");
    }

    public void ShowReplayButton()
    {
        gameObject.SetActive(true);
    }
}
