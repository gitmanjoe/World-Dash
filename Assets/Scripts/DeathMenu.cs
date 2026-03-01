using UnityEngine;
using UnityEngine.SceneManagement;

// Simple death menu: call Show() when the player dies. Provides UI hooks for Restart, Menu and Quit.
public class DeathMenu : MonoBehaviour
{
    [Tooltip("Root UI GameObject for the death menu (panels/buttons)")]
    public GameObject deathMenuUI;

    void Start()
    {
        // If not assigned in inspector, try a few safe auto-find fallbacks so the component is usable
        if (deathMenuUI == null)
        {
            // try by name
            var foundByName = GameObject.Find("DeathMenuUI");
            if (foundByName != null)
            {
                deathMenuUI = foundByName;
                Debug.Log("DeathMenu: auto-assigned deathMenuUI by name 'DeathMenuUI'.");
            }
            else
            {
                // try by tag (if you tagged the UI root with "DeathMenu")
                try
                {
                    var foundByTag = GameObject.FindWithTag("DeathMenu");
                    if (foundByTag != null)
                    {
                        deathMenuUI = foundByTag;
                        Debug.Log("DeathMenu: auto-assigned deathMenuUI by tag 'DeathMenu'.");
                    }
                }
                catch { /* ignore if tag doesn't exist */ }
            }
        }

        if (deathMenuUI != null)
            deathMenuUI.SetActive(false);
        else
            Debug.LogWarning("DeathMenu: deathMenuUI is not assigned in the inspector (and auto-find failed). Use the Context Menu 'Auto Assign' or assign manually.");
    }

    // Show the death menu and pause the game. Call this from your player/death logic.
    public void Show()
    {
        Debug.Log("DeathMenu.Show() called");
        if (deathMenuUI != null) deathMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Helper you can run from the inspector (component context menu) to try auto-assigning the UI.
    [ContextMenu("Auto Assign Death Menu UI")]
    void AutoAssign()
    {
        if (deathMenuUI == null)
        {
            var found = GameObject.Find("DeathMenuUI");
            if (found != null) deathMenuUI = found;
            else
            {
                try
                {
                    found = GameObject.FindWithTag("DeathMenu");
                    if (found != null) deathMenuUI = found;
                }
                catch { }
            }
        }

        Debug.Log("DeathMenu.AutoAssign() result: " + (deathMenuUI != null ? deathMenuUI.name : "null"));
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
