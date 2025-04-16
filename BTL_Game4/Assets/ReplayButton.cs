using UnityEngine;
using UnityEngine.EventSystems;  // Để làm việc với EventTrigger

public class ReplayButton : MonoBehaviour, IPointerClickHandler
{
    // Phương thức này sẽ được gọi khi click vào image
    void Start()
    {
        gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Hello");
    }

    public void ShowReplayButton()
    {
        gameObject.SetActive(true);
    }
}