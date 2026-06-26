using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    // 🔹 Restart the current level
    public void RestartLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    // 🔹 Quit game (optional)
    public void QuitGame()
    {
        Debug.Log("Quit Game pressed!");
        Application.Quit();
    }
}

