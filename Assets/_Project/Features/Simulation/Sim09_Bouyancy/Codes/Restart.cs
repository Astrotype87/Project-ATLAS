using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f; // resume time in case it was paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
