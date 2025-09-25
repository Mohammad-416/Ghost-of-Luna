using UnityEngine;
public class Spikes : MonoBehaviour
{

    private AudioSource audioSource;
    [Header("Sound Effects")]           
    public AudioClip DieSound;           
    [Range(0f, 1f)] public float soundVolume = 1f;

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
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only the actual player can spawn ghosts, not the ghost itself
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if it has PlayerMovement component (extra safety check)
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Debug.Log("Player hit spikes!");
                if (DieSound != null)
                {
                    audioSource.PlayOneShot(DieSound, soundVolume);
                }
                playerMovement.TriggerDeath();
            }
        }
        // Ghosts just pass through or bounce off spikes without creating more ghosts
        else if (collision.gameObject.CompareTag("Ghost"))
        {
            Debug.Log("Ghost hit spikes - no death triggered");
            // Optional: Add some visual/audio feedback for ghost hitting spikes
        }
    }
}
