using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject fireAnimation;        // Assign your fire animation GameObject
    public float respawnTime = 2f;         // Time before bomb respawns
    public float explosionDuration = 0.5f;   // Duration of the explosion effect
    
    [Header("Visual Settings")]
    public SpriteRenderer bombSprite;      // Optional: for visual feedback
    public Collider2D bombCollider;       // The trigger collider
    
    // Private variables
    public bool isExploded = false;
    
    void Start()
    {
        // Get components automatically if not assigned
        if (bombSprite == null)
            bombSprite = GetComponent<SpriteRenderer>();
        
        if (bombCollider == null)
            bombCollider = GetComponent<Collider2D>();
        
        // Make sure collider is set as trigger
        if (bombCollider != null)
            bombCollider.isTrigger = true;
        
        // Make sure fire animation starts disabled
        if (fireAnimation != null)
            fireAnimation.SetActive(false);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if player or ghost triggered the bomb
        if (other.CompareTag("Player") && !isExploded)
        {
            ExplodeBomb();
            StartCoroutine(Exploding());
        }
        if( other.CompareTag("Ghost") && !isExploded)
        {
            ExplodeBomb();
        }
    }

    IEnumerator Exploding()
    {
        yield return new WaitForSeconds(explosionDuration);
        isExploded = true;
    }

    void ExplodeBomb()
    {
        if (isExploded) return; // Prevent multiple explosions



        // Activate fire animation
        if (fireAnimation != null)
        {
            fireAnimation.SetActive(true);
            Debug.Log("Bomb exploded! Fire animation activated.");
        }

        // Hide bomb visually
        if (bombSprite != null)
            bombSprite.enabled = false;

        // Disable collider so it can't trigger again
        if (bombCollider != null)
            bombCollider.enabled = false;

        
        // Start respawn timer
        StartCoroutine(RespawnBomb());
        
    }

 
    
    IEnumerator RespawnBomb()
    {
        // Wait for respawn time
        yield return new WaitForSeconds(respawnTime);
        
        // Reset bomb state
        isExploded = false;
        
        // Re-enable bomb visually
        if (bombSprite != null)
            bombSprite.enabled = true;
        
        // Re-enable collider
        if (bombCollider != null)
            bombCollider.enabled = true;
        
        // Disable fire animation
        if (fireAnimation != null)
        {
            fireAnimation.SetActive(false);
            Debug.Log("Bomb respawned!");
        }
    }
    
    // Optional: Manual trigger for testing
    [ContextMenu("Trigger Bomb")]
    public void TriggerBombManually()
    {
        ExplodeBomb();
    }
    
    // Optional: Reset bomb state
    public void ResetBomb()
    {
        StopAllCoroutines();
        isExploded = false;
        
        if (bombSprite != null)
            bombSprite.enabled = true;
        
        if (bombCollider != null)
            bombCollider.enabled = true;
        
        if (fireAnimation != null)
            fireAnimation.SetActive(false);
    }
}
