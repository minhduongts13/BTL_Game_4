using UnityEngine;
using UnityEngine.UI;

public class TurnHighlighter : MonoBehaviour
{
    [Header("Player Name Texts")]
    // Mảng 4 text dùng để hiển thị tên người chơi (theo thứ tự mong muốn)
    public Text playerName1;
    public Text playerName2;
    public Text playerName3;
    public Text playerName4;

    private Text[] playerNameTexts;
    [Header("Color Settings")]
    public Color highlightColor = Color.yellow; // Màu để highlight
    public Color defaultColor = Color.white;    // Màu mặc định

    /// <summary>
    /// Update highlight của tên dựa trên index của người chơi có lượt.
    /// Ví dụ: Nếu newTurnIndex = 0 thì text thứ nhất được highlight,
    /// còn các text khác trở về màu mặc định.
    /// </summary>
    /// <param name="newTurnIndex">Index của người chơi có lượt (0 - 3)</param>
    
    TurnHighlighter(){
        playerNameTexts = new Text[] { playerName1, playerName2, playerName3, playerName4 };
    }
    public void SetTurn(int newTurnIndex)
    {
        
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i == newTurnIndex)
            {
                playerNameTexts[i].color = highlightColor;
            }
            else
            {
                playerNameTexts[i].color = defaultColor;
            }
        }
    }
}
