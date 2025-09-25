using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public GameObject spikeTrap;
    public int activeLeverCount = 0;
    public int requiredLeverCount = 2; // Number of levers required to activate the trap

    void Start()
    {
        if (spikeTrap != null)
            spikeTrap.SetActive(true);
    }

    void Update()
    {
        if(activeLeverCount >= requiredLeverCount)
        {
            if (spikeTrap != null && spikeTrap.activeSelf)
                spikeTrap.SetActive(false);
        }
        else
        {
            if (spikeTrap != null && !spikeTrap.activeSelf)
                spikeTrap.SetActive(true);
        }
    }

}
