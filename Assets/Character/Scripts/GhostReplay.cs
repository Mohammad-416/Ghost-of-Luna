using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostReplay : MonoBehaviour
{
    private List<PlayerMovement.PlayerFrame> framesToReplay;
    private int currentFrameIndex = 0;
    private float replaySpeed = 1f;
    private bool isReplaying = false;

    public void Initialize(List<PlayerMovement.PlayerFrame> frames)
    {
        framesToReplay = frames;

        if (framesToReplay != null && framesToReplay.Count > 0)
        {
            // Start at first frame
            transform.position = framesToReplay[0].position;
            transform.localScale = framesToReplay[0].scale;

            StartCoroutine(ReplayMovement());
        }
    }

    IEnumerator ReplayMovement()
    {
        isReplaying = true;

        while (currentFrameIndex < framesToReplay.Count && isReplaying)
        {
            PlayerMovement.PlayerFrame currentFrame = framesToReplay[currentFrameIndex];

            // Set position and scale
            transform.position = currentFrame.position;
            transform.localScale = currentFrame.scale;

            currentFrameIndex++;

            // Wait for next frame based on record interval
            yield return new WaitForSeconds(0.02f);
        }

        // Loop the replay
        currentFrameIndex = 0;
        if (framesToReplay.Count > 0)
        {
            StartCoroutine(ReplayMovement());
        }
    }

    public void StopReplay()
    {
        isReplaying = false;
        StopAllCoroutines();
    }
    
    
}
