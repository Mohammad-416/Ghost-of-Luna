using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 6f;
    
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask;
    
    [Header("Quality of Life")]
    public float coyoteTime = 0.2f;        
    public float jumpBufferTime = 0.2f;    
    
    [Header("Ghost System")]
    public GameObject ghostPrefab;          // Assign your disabled ghost prefab
    public float recordInterval = 0.02f;    // Record every 0.02 seconds
    
    [Header("Death & Respawn")]
    public Transform respawnPoint;          // Where player respawns
    public string deathTag = "DeathZone";   // Tag for death triggers
    
    [Header("Sound Effects")]
    public AudioClip jumpSound;             // Jump sound effect
    public AudioClip moveSound;
    public AudioClip startSound;            // Movement start sound effect
    [Range(0f, 1f)] public float soundVolume = 1f;
    
    // Private variables
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isGrounded;
    private float horizontalInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool wasMoving = false;         // Track if we were moving last frame
    
    // Recording system
    private List<PlayerFrame> recordedFrames = new List<PlayerFrame>();
    private float recordTimer = 0f;
    private bool isRecording = true;
    private GameObject currentGhost = null;  // Only one ghost at a time
    
    // Death system
    private Vector3 startPosition;
    private bool isDead = false;
    
    [System.Serializable]
    public class PlayerFrame
    {
        public Vector3 position;
        public Vector3 scale;
        public float timestamp;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        // Create AudioSource if it doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Store starting position for respawn
        startPosition = respawnPoint != null ? respawnPoint.position : transform.position;
        
        // Create ground check point if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        
        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound, soundVolume);
        }
        
        // Start recording from game start
        StartRecording();
    }
    
    void Update()
    {
        if (isDead) return; // Don't process input when dead
        
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Ground detection using OverlapCircle
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        
        // Coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        // Jump buffering
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        // Movement sound effect
        bool isMovingNow = Mathf.Abs(horizontalInput) > 0.1f && isGrounded;
        if (isMovingNow && !wasMoving && moveSound != null)
        {
            audioSource.PlayOneShot(moveSound, soundVolume);
        }
        wasMoving = isMovingNow;
        
        // Flip sprite based on movement direction
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        
        // Record current frame
        if (isRecording)
        {
            RecordCurrentFrame();
        }
    }
    
    void FixedUpdate()
    {
        if (isDead) return;
        
        // Horizontal movement
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        
        // Jumping with sound effect
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            
            // Play jump sound
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound, soundVolume);
            }
        }
        
        // Variable jump height
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }
    
    void RecordCurrentFrame()
    {
        recordTimer += Time.deltaTime;
        
        if (recordTimer >= recordInterval)
        {
            PlayerFrame frame = new PlayerFrame
            {
                position = transform.position,
                scale = transform.localScale,
                timestamp = Time.time
            };
            
            recordedFrames.Add(frame);
            recordTimer = 0f;
        }
    }
    
    void StartRecording()
    {
        recordedFrames.Clear();
        isRecording = true;
        recordTimer = 0f;
    }
    
 
    public void Die()
    {
        if (isDead) return; // Prevent multiple deaths
        
        isDead = true;
        isRecording = false;
        
        // Create ghost from recorded frames
        if (recordedFrames.Count > 0 && ghostPrefab != null)
        {
            CreateGhost();
        }
        
        // Respawn after a short delay
        Invoke(nameof(Respawn), 1f);
    }
    
    void CreateGhost()
    {
        // Destroy previous ghost if it exists (only one ghost at a time)
        if (currentGhost != null)
        {
            Destroy(currentGhost);
        }
        
        // Instantiate new ghost
        currentGhost = Instantiate(ghostPrefab, recordedFrames[0].position, Quaternion.identity);
        currentGhost.SetActive(true);
        
        // Set ghost as ground layer and player tag for interaction
        currentGhost.layer = LayerMask.NameToLayer("Ground");
        currentGhost.tag = "Ghost";
        
        // Add ghost replay component
        GhostReplay ghostReplay = currentGhost.GetComponent<GhostReplay>();
        if (ghostReplay == null)
        {
            ghostReplay = currentGhost.AddComponent<GhostReplay>();
        }
        
        // Initialize ghost with recorded frames
        ghostReplay.Initialize(new List<PlayerFrame>(recordedFrames));
    }
    
    void Respawn()
    {
        // Reset player state
        isDead = false;
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        
        // Clear and restart recording for next life
        StartRecording();
    }
    
    // Call this to manually trigger death (for testing)
    public void TriggerDeath()
    {
        Die();
    }
    
    // Clear current ghost (useful for level restart)
    public void ClearCurrentGhost()
    {
        if (currentGhost != null)
        {
            DestroyImmediate(currentGhost);
            currentGhost = null;
        }
    }
    
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // Draw respawn point
        if (respawnPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(respawnPoint.position, Vector3.one * 0.5f);
        }
    }
}
