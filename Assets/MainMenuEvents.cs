using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UIDocument mainMenuDocument;
    public UIDocument pauseMenuDocument;

    private VisualElement mainMenuUI;
    private VisualElement pauseMenuUI;

    private Button startButton;
    private Button quitButton;
    private Button resumeButton;
    private Button exitToMainMenuButton;

    private bool isPaused = false;

    void Start()
    {
        // Get root elements
        mainMenuUI = mainMenuDocument.rootVisualElement;
        pauseMenuUI = pauseMenuDocument.rootVisualElement;

        // Get buttons from main menu
        startButton = mainMenuUI.Q<Button>("StartButton");
        quitButton = mainMenuUI.Q<Button>("Quit");

        // Get buttons from pause menu
        resumeButton = pauseMenuUI.Q<Button>("ResumeGame");
        exitToMainMenuButton = pauseMenuUI.Q<Button>("ExitToMainMenu");

        // Set button actions
        startButton.clicked += StartGame;
        quitButton.clicked += QuitGame;
        resumeButton.clicked += ResumeGame;
        exitToMainMenuButton.clicked += ReturnToMainMenu;

        // Show main menu at start
        Time.timeScale = 0f;
        mainMenuUI.style.display = DisplayStyle.Flex;
        pauseMenuUI.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && mainMenuUI.style.display == DisplayStyle.None)
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        mainMenuUI.style.display = DisplayStyle.None;
        isPaused = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenuUI.style.display = DisplayStyle.Flex;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenuUI.style.display = DisplayStyle.None;
        isPaused = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 0f;
        pauseMenuUI.style.display = DisplayStyle.None;
        mainMenuUI.style.display = DisplayStyle.Flex;
        isPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit.");
        Application.Quit();
    }
}
