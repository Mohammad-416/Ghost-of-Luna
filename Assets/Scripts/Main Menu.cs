using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject CreditsPanel;
    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenCredits()
    {
        CreditsPanel.SetActive(true);
    }
    
    public void CloseCredits()
    {
        CreditsPanel.SetActive(false);
    }

}
