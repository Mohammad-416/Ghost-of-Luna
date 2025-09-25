using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public GameObject pausePanel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }
    
    void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
}
