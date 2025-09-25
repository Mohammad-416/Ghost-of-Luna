using UnityEngine;

public class BombDeath : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip deathSoundClip;        // Death sound when player dies in blast
    [Range(0f, 1f)] public float soundVolume = 1f;
    
  
    
    // Private components
    private BoxCollider2D blastCollider;
    private AudioSource audioSource;
    public Bomb bomb; // Reference to the Bomb script to check explosion state
    
    void Start()
    {
        SetupComponents();
    }
    
    void SetupComponents()
    {
        // Get or create BoxCollider2D
        blastCollider = GetComponent<BoxCollider2D>();
        if (blastCollider == null)
        {
            blastCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Configure blast collider
        blastCollider.isTrigger = true;
        
        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // This will be called when bomb explodes and player is in range
        if (other.CompareTag("Player") && bomb != null && bomb.isExploded)
        {
            KillPlayer(other);
            bomb.isExploded = false; // Prevent multiple kills per explosion`
        }
        
    }

  

    void KillPlayer(Collider2D player)
    {
        // Play death sound
        if (deathSoundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSoundClip, soundVolume);
            Debug.Log("Player killed by blast! Death sound played.");
        }
        
        // Trigger player death
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.TriggerDeath();
            Debug.Log("Player died from bomb blast!");
        }
    }
    
}
