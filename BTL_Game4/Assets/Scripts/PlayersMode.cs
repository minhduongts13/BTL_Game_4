using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersMode  : MonoBehaviour
{
    public void OnPlayersButtonClicked()
    {
        Debug.Log("✅ Button Players được nhấn!");
        // Thêm hành động tại đây
        SceneManager.LoadScene("Menu");
    }

    public void OnAIButtonClicked()
    {
        Debug.Log("✅ Button AI được nhấn!");
        // Thêm hành động tại đây
        SceneManager.LoadScene("Ranking");
    }
}
