using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialDoor : MonoBehaviour
{
    [Tooltip("Drag the Player GameObject here")]
    public GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject != player)
            return;

        // Load next scene immediately
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
}