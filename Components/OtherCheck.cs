using UnityEngine;

namespace UltraRGB.Components;

public class OtherCheck : BaseCheck
{
    private static bool paused = false;
    private static bool cheatsEnabled = false;

    private OptionsManager optionsManagerCache;
    private CheatsController cheatsControllerCache;

    protected override void RateLimitedUpdate()
    {
        if (optionsManagerCache == null || cheatsControllerCache == null)
        {
            optionsManagerCache = OptionsManager.Instance;
            cheatsControllerCache = CheatsController.Instance;
        }

        if (optionsManagerCache != null)
        {
            if (paused != optionsManagerCache.paused)
            {
                paused = optionsManagerCache.paused;
                UltraRGB.QueueUpdate("OtherPaused", paused);
            }
        }

        if (cheatsControllerCache != null && !paused)
        {
            if (cheatsEnabled != cheatsControllerCache.cheatsEnabled)
            {
                cheatsEnabled = cheatsControllerCache.cheatsEnabled;
                UltraRGB.QueueUpdate("OtherCheatsEnabled", cheatsEnabled);
            }
        }
    }
}
