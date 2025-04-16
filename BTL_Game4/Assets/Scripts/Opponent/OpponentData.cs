using UnityEngine;

public enum OpponentPosition
{
    Left,

    Top,

    Right
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewOpponent", menuName = "UNO/OpponentData")]
public class OpponentData : ScriptableObject  // Kế thừa từ ScriptableObject
{
    public string opponentName;
    public Sprite avatar;
    public OpponentPosition fixedPosition;  // Enum chứa các lựa chọn
}
