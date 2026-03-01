using UnityEngine;
using UnityEngine.SceneManagement;

// Brackeys-style pause menu
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [Tooltip("Root UI GameObject for the pause menu (panels/buttons)")]
    public GameObject pauseMenuUI;

    void Start()
    {
        // Ensure game starts unpaused
        Time.timeScale = 1f;
        GameIsPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("Resuming game");
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // UI button hooks
    public void LoadMenu ()
    {
        Time.timeScale = 1f;
        // Load the first scene in the Build Settings (index 0)
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        // In editor this does nothing; in a built player it quits
        Application.Quit();
    }
}
