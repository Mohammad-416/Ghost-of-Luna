using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadMainMenu : MonoBehaviour
{
    public float delay = 10f; // Delay in seconds before loading the main menu
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadMenu());
    }
    
    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        SceneManager.LoadScene(0); // Load the scene with index 0 (Main Menu)
    }

}
