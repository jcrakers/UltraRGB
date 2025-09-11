namespace UltraRGB.Components;

public class MiscellaneousCheck : BaseCheck
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
                UltraRGB.QueueUpdate("IsPaused", paused, "Miscellaneous");
            }
        }

        if (cheatsControllerCache != null && !paused)
        {
            if (cheatsEnabled != cheatsControllerCache.cheatsEnabled)
            {
                cheatsEnabled = cheatsControllerCache.cheatsEnabled;
                UltraRGB.QueueUpdate("CheatsEnabled", cheatsEnabled, "Miscellaneous");
            }
        }
    }
}
