using UnityEngine;

public class Lever : MonoBehaviour
{
    public SpikeTrap spikeTrap; // Reference to the SpikeTrap script
    private bool isActive = false;
    public GameObject leverOnSprite;
    public GameObject leverOffSprite;

    void Start()
    {
        UpdateLeverVisual();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost") && !isActive)
        {
            ActivateLever();
        }
    }
    void ActivateLever()
    {
        isActive = true;
        if (isActive)
        {
            spikeTrap.activeLeverCount++;
        }
        UpdateLeverVisual();
    }

    void DeactivateLever()
    {
        isActive = false;
        if (!isActive && spikeTrap.activeLeverCount > 0)
        {
            spikeTrap.activeLeverCount--;
        }
        UpdateLeverVisual();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            DeactivateLever();
        }
    }
    void UpdateLeverVisual()
    {
        if (leverOnSprite != null && leverOffSprite != null)
        {
            leverOnSprite.SetActive(isActive);
            leverOffSprite.SetActive(!isActive);
        }
    }
}
