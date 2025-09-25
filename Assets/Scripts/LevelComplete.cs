using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelComplete : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("Sound Effects")]           
    public AudioClip WinSound;           
    [Range(0f, 1f)] public float soundVolume = 1f;
    public GameObject completePanel;
    public GameObject pausePanel;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Create AudioSource if it doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }


    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    ///
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level Complete!");
            if (WinSound != null)
            {
                audioSource.PlayOneShot(WinSound, soundVolume);
            }
            completePanel.SetActive(true);
            pausePanel.SetActive(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            // Here you can add code to load the next level or show a level complete UI
        }
    } 
}
