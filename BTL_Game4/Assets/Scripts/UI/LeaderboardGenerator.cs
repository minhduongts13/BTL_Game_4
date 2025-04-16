using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int cardsRemaining;
}

public class LeaderboardGenerator : MonoBehaviour
{
    [Header("Data đầu vào (có thể set từ Inspector)")]
    public List<PlayerData> players;

    [Header("Font và kích thước")]
    public Font font;
    public int fontSize = 24;

    [Header("Kích thước & khoảng cách")]
    public Vector2 panelSize = new Vector2(400, 600);
    public float entryHeight = 60;
    public float entrySpacing = 10;
    public Vector2 panelPosition = new Vector2(0, 0);

    Canvas _canvas;

    void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("LeaderboardGenerator phải nằm trong Canvas!");
            return;
        }

        // Sắp xếp tăng dần theo cardsRemaining (ít lá nhất = rank cao)
        players.Sort((a, b) => a.cardsRemaining.CompareTo(b.cardsRemaining));

        CreatePanel();
    }

    void CreatePanel()
    {
        // 1. Tạo Panel chính
        GameObject panelGO = new GameObject("LeaderboardPanel", typeof(RectTransform));
        panelGO.transform.SetParent(_canvas.transform, false);
        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.sizeDelta = panelSize;
        panelRT.anchoredPosition = panelPosition;

        // Thêm background
        var bg = panelGO.AddComponent<RectangleGraphic>();
        bg.color = new Color(0.1f, 0.5f, 0.8f, 0.9f);

        // 2. Tạo tiêu đề
        GameObject titleGO = new GameObject("Title", typeof(RectTransform));
        titleGO.transform.SetParent(panelGO.transform, false);
        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 1);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot = new Vector2(0.5f, 1);
        titleRT.anchoredPosition = Vector2.zero;
        titleRT.sizeDelta = new Vector2(0, 80);

        Text titleTxt = titleGO.AddComponent<Text>();
        titleTxt.font = font;
        titleTxt.fontSize = fontSize + 8;
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.text = "BẢNG XẾP HẠNG";

        // 3. Tạo từng entry
        for (int i = 0; i < players.Count; i++)
        {
            var pd = players[i];
            float yOffset = -(80 + entrySpacing) - i * (entryHeight + entrySpacing);

            GameObject entryGO = new GameObject($"Entry_{i + 1}", typeof(RectTransform));
            entryGO.transform.SetParent(panelGO.transform, false);
            RectTransform entryRT = entryGO.GetComponent<RectTransform>();
            entryRT.anchorMin = new Vector2(0, 1);
            entryRT.anchorMax = new Vector2(1, 1);
            entryRT.pivot = new Vector2(0.5f, 1);
            entryRT.anchoredPosition = new Vector2(0, yOffset);
            entryRT.sizeDelta = new Vector2(0, entryHeight);

            // background entry
            var ebg = entryGO.AddComponent<RectangleGraphic>();
            ebg.color = (i % 2 == 0)
                ? new Color(1f, 1f, 1f, 0.2f)
                : new Color(1f, 1f, 1f, 0.1f);

            // Rank
            CreateText(entryGO.transform, new Vector2(10, 0), new Vector2(100, entryHeight),
                       $"{i + 1}", TextAnchor.MiddleLeft);

            // Tên người chơi
            CreateText(entryGO.transform, new Vector2(120, 0), new Vector2(200, entryHeight),
                       pd.playerName, TextAnchor.MiddleLeft);

            // Số lá bài còn lại
            CreateText(entryGO.transform, new Vector2(-10, 0), new Vector2(100, entryHeight),
                       pd.cardsRemaining.ToString(), TextAnchor.MiddleRight);
        }
    }

    void CreateText(Transform parent, Vector2 pos, Vector2 size, string txt, TextAnchor align)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0, 0.5f);
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Text t = go.AddComponent<Text>();
        t.font = font;
        t.fontSize = fontSize;
        t.alignment = align;
        t.text = txt;
        t.color = Color.white;
    }
}
