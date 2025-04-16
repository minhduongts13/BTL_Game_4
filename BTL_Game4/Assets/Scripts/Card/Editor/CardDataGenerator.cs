using UnityEngine;
using UnityEditor;
using System.IO;

public class CardDataGenerator
{
    [MenuItem("Tools/Generate CardData Assets")]
    public static void GenerateCardAssets()
    {
        string spriteFolderPath = "Assets/Cards_Image";
        string assetOutputPath = "Assets/Data/Cards";

        if (!Directory.Exists(assetOutputPath))
            Directory.CreateDirectory(assetOutputPath);

        string[] spritePaths = Directory.GetFiles(spriteFolderPath, "*.png", SearchOption.TopDirectoryOnly);

        foreach (string fullPath in spritePaths)
        {
            string assetPath = fullPath.Replace("\\", "/");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite == null)
            {
                Debug.LogWarning($"Không thể load sprite từ: {assetPath}");
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(assetPath); // Ví dụ: Red_5
            string[] parts = fileName.Split('_');

            if (parts.Length < 2)
            {
                Debug.LogWarning($"Tên file không hợp lệ (phải có dạng Color_Type): {fileName}");
                continue;
            }

            string cardColor = parts[0];           // Red, Blue, etc.
            string cardValueRaw = parts[1];        // 5, Skip, Draw4...

            int cardNumber = -1;
            if (int.TryParse(cardValueRaw, out int number))
            {
                cardNumber = number;               // Số thường: 0–9
            }
            else
            {
                // Gán số đặc biệt cho lá bài đặc biệt
                switch (cardValueRaw.ToLower())
                {
                    case "skip": cardNumber = -10; break;
                    case "reverse": cardNumber = -11; break;
                    case "draw2": cardNumber = -12; break;
                    case "draw4": cardNumber = -13; break;
                    case "wild": cardNumber = -14; break;
                    default:
                        Debug.LogWarning($"Giá trị lá bài không xác định: {cardValueRaw}");
                        continue;
                }
            }

            CardData newCard = ScriptableObject.CreateInstance<CardData>();
            newCard.cardColor = cardColor;
            newCard.cardNumber = cardNumber;
            newCard.cardSprite = sprite;

            string assetName = $"{cardColor}_{cardValueRaw}.asset";
            string savePath = Path.Combine(assetOutputPath, assetName);

            AssetDatabase.CreateAsset(newCard, savePath);
            Debug.Log($"Đã tạo: {savePath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Tạo CardData hoàn tất!");
    }
}
