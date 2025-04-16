using UnityEngine;
using TMPro;

public class Ranking : MonoBehaviour
{
    public Transform ranksParent; // Gán object "Ranks" vào đây
    public string[] playerNames = new string[] { "haha", "hoho", "hehe", "hihi" }; // Mặc định ban đầu
    public ReplayButton replayButton;

    void Start()
    {
        UpdateRanking(playerNames);
        // TestUpdateRanking();
        StartCoroutine(BlurAfterTime(10f));
    }

    // Coroutine để làm mờ sau một khoảng thời gian
    System.Collections.IEnumerator BlurAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay); // Chờ trong thời gian đã cho

        // Thực hiện blur (ở đây, thay đổi alpha của các đối tượng UI để tạo hiệu ứng mờ)
        foreach (Transform rankItem in ranksParent)
        {
            CanvasGroup canvasGroup = rankItem.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = rankItem.gameObject.AddComponent<CanvasGroup>(); // Nếu chưa có CanvasGroup, tạo mới
            }

            // Dần dần làm mờ UI (alpha giảm xuống)
            float elapsedTime = 0f;
            float duration = 1f; // Thời gian làm mờ (1 giây)
            float startAlpha = canvasGroup.alpha;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration); // Làm mờ dần
                yield return null;
            }

            canvasGroup.alpha = 0f; // Đảm bảo alpha = 0 sau khi hết thời gian
        }
        // Hiển thị ReplayButton sau khi đã làm mờ xong
        if (replayButton != null)
        {
            replayButton.ShowReplayButton(); // Gọi phương thức ShowReplayButton để hiển thị nút Replay
        }
    }
    /// <summary>
    /// Cập nhật lại danh sách tên và hiển thị trên UI
    /// </summary>
    /// <param name="newNames">Mảng tên người chơi mới</param>

    public void UpdateRanking(string[] newNames)
    {
        playerNames = newNames;

        for (int i = 0; i < ranksParent.childCount; i++)
        {
            Transform rankItem = ranksParent.GetChild(i);

            // Nếu có tên tương ứng thì hiển thị và cập nhật text
            if (i < playerNames.Length)
            {
                rankItem.gameObject.SetActive(true);

                TMP_Text nameText = rankItem.GetComponentInChildren<TMP_Text>();
                if (nameText != null)
                {
                    nameText.text = playerNames[i];
                }
            }
            else
            {
                // Nếu không có tên, ẩn khung này đi
                rankItem.gameObject.SetActive(false);
            }
        }
    }


    public void TestUpdateRanking()
    {
        Debug.Log("🔁 Đang test UpdateRanking...");

        // Danh sách ví dụ
        string[] test1 = new string[] { "Alice", "Bob" };
        string[] test2 = new string[] { "Charlie", "Daisy", "Eve" };
        string[] test3 = new string[] { "Foo", "Bar", "Baz", "Qux" };

        // Gọi lần lượt từng test
        StartCoroutine(TestRoutine(test1, test2, test3));
    }

    // ⏱ Coroutine để test theo thời gian (delay vài giây giữa các lần)
    System.Collections.IEnumerator TestRoutine(params string[][] testSets)
    {
        foreach (var testSet in testSets)
        {
            UpdateRanking(testSet);
            Debug.Log("Đang hiển thị: " + string.Join(", ", testSet));
            yield return new WaitForSeconds(2f); // Chờ 2 giây để quan sát
        }
    }
}
