using UnityEngine;

[CreateAssetMenu(fileName = "NewOpponent", menuName = "UNO/OpponentData")]
public class OpponentData : ScriptableObject
{
    public string opponentName;
    public Sprite avatar;
    public int initialCardCount = 7;
    // sau này có thể thêm AI type, chiến thuật,...
}
