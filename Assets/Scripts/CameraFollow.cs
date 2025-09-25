using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;                    // The player to follow
    public Vector3 offset = new Vector3(0, 2, -10);  // Camera offset from player
    
    [Header("Smoothing")]
    public bool useSmoothing = true;
    public float smoothSpeed = 2f;              // Speed of camera smoothing
    
    [Header("Dead Zone (Optional)")]
    public bool useDeadZone = false;
    public Vector2 deadZoneSize = new Vector2(2f, 1f);  // Size of dead zone
    
    [Header("Bounds (Optional)")]
    public bool useBounds = false;
    public Vector2 minBounds;                   // Minimum camera position
    public Vector2 maxBounds;                   // Maximum camera position
    
    private Vector3 velocity = Vector3.zero;    // For SmoothDamp
    
    void Start()
    {
        // If no target assigned, try to find player automatically
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = target.position + offset;
        
        // Apply dead zone logic
        if (useDeadZone)
        {
            desiredPosition = ApplyDeadZone(desiredPosition);
        }
        
        // Apply bounds if enabled
        if (useBounds)
        {
            desiredPosition = ApplyBounds(desiredPosition);
        }
        
        // Move camera with or without smoothing
        if (useSmoothing)
        {
            // Smooth camera movement
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
        }
        else
        {
            // Instant camera movement
            transform.position = desiredPosition;
        }
    }
    
    Vector3 ApplyDeadZone(Vector3 desiredPosition)
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = target.position;
        
        // Calculate the difference between camera and target
        float deltaX = targetPos.x - currentPos.x;
        float deltaY = targetPos.y - currentPos.y;
        
        // Only move camera if target is outside dead zone
        if (Mathf.Abs(deltaX) > deadZoneSize.x / 2)
        {
            float sign = Mathf.Sign(deltaX);
            desiredPosition.x = targetPos.x - (sign * deadZoneSize.x / 2) + offset.x;
        }
        else
        {
            desiredPosition.x = currentPos.x;
        }
        
        if (Mathf.Abs(deltaY) > deadZoneSize.y / 2)
        {
            float sign = Mathf.Sign(deltaY);
            desiredPosition.y = targetPos.y - (sign * deadZoneSize.y / 2) + offset.y;
        }
        else
        {
            desiredPosition.y = currentPos.y;
        }
        
        return desiredPosition;
    }
    
    Vector3 ApplyBounds(Vector3 desiredPosition)
    {
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        return desiredPosition;
    }
    
    // Visualize dead zone and bounds in Scene view
    void OnDrawGizmos()
    {
        // Draw dead zone
        if (useDeadZone && target != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 deadZoneCenter = transform.position;
            deadZoneCenter.z = target.position.z;
            Gizmos.DrawWireCube(deadZoneCenter, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));
        }
        
        // Draw bounds
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 boundsCenter = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0);
            Vector3 boundsSize = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
}
