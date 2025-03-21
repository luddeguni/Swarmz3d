using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;

    void Start()
    {
        Time.timeScale = 0f; // Ensure the game is paused on start
        mainMenuUI.SetActive(true);
    }

    public void StartGame()
    {
        Time.timeScale = 1f; // Resume the game
        mainMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit.");
        Application.Quit();
    }
}
