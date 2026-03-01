using UnityEngine;
using UnityEngine.SceneManagement;

// Simple death menu: call Show() when the player dies. Provides UI hooks for Restart, Menu and Quit.
public class DeathMenu : MonoBehaviour
{
    [Tooltip("Root UI GameObject for the death menu (panels/buttons)")]
    public GameObject deathMenuUI;
    public GameObject depthMenuUI;

    void Start()
    {
        if (deathMenuUI != null)
            deathMenuUI.SetActive(false);
        else
            Debug.LogWarning("DeathMenu: deathMenuUI is not assigned in the inspector.");
    }

    // Show the death menu and pause the game. Call this from your player/death logic.
    public void Show()
    {
        if (deathMenuUI != null) deathMenuUI.SetActive(true);
        depthMenuUI.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Hide the death menu and resume the game (optional)
    public void Hide()
    {
        if (deathMenuUI != null) deathMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // UI button hooks
    public void RestartLevel()
    {
        Debug.Log("Restarting level");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
