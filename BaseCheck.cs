using UnityEngine;

public abstract class BaseCheck : MonoBehaviour
{
    protected float lastUpdateTime = 0f;
    protected float updateInterval;

    protected virtual void Start()
    {
        updateInterval = 1f / UltraRGB.UltraRGB.UpdateRate; // Convert desired update rate to interval
    }
    
    private void Update()
    {
       // If rate limiting is disabled, always update
        if (!UltraRGB.UltraRGB.EnableRateLimiting)
            return;
            
        // If we have changes and enough time has passed, update
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            RateLimitedUpdate();
        }
    }

    protected abstract void RateLimitedUpdate();
}