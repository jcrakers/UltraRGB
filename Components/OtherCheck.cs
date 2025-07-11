using UnityEngine;

namespace UltrakillArtemisMod.Components;

public class OtherCheck : MonoBehaviour
{
    private static bool initialized;
    public static void Init()
    {
        if (!initialized)
        {
            initialized = true;
            GameObject otherGameObject = new GameObject("OtherCheck");
            OtherCheck otherCheck = otherGameObject.AddComponent<OtherCheck>();
            otherGameObject.hideFlags = HideFlags.HideAndDontSave;
            otherCheck.enabled = true;
            DontDestroyOnLoad(otherGameObject);
        }
    }
    public delegate void PauseCheatHandler(bool state);

    public static PauseCheatHandler OnPause;
    public static PauseCheatHandler OnCheatsEnabled;

    private static bool paused = false;
    private static bool cheatsEnabled = false;

    private OptionsManager optionsManagerCache;
    private CheatsController cheatsControllerCache;

    void Update()
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
                OnPause?.Invoke(optionsManagerCache.paused);
            }
        }

        if (cheatsControllerCache != null && !paused)
        {
            if (cheatsEnabled != cheatsControllerCache.cheatsEnabled)
            {
                cheatsEnabled = cheatsControllerCache.cheatsEnabled;
                OnCheatsEnabled?.Invoke(cheatsControllerCache.cheatsEnabled);
            }
        }
    }
}