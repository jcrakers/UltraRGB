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
    public delegate void DeathPauseCheatHandler(bool state);
    public static DeathPauseCheatHandler OnDeath;
    public static DeathPauseCheatHandler OnPause;
    public static DeathPauseCheatHandler OnCheatsEnabled;
    private static bool oldPaused = false;
    private static bool oldDead = false;
    private static bool oldCheatsEnabled = false;

    void Update()
    {
        if (MonoSingleton<OptionsManager>.Instance != null)
        {
            if (oldPaused != OptionsManager.Instance.paused)
            {
                oldPaused = OptionsManager.Instance.paused;
                OnPause?.Invoke(OptionsManager.Instance.paused);
            }
        }

        if (MonoSingleton<NewMovement>.Instance != null)
        {
            if (oldDead != MonoSingleton<NewMovement>.Instance.dead)
            {
                oldDead = MonoSingleton<NewMovement>.Instance.dead;
                OnDeath?.Invoke(MonoSingleton<NewMovement>.Instance.dead);
            }
        }

        if (CheatsController.Instance != null)
        {
            if (oldCheatsEnabled != CheatsController.Instance.cheatsEnabled)
            {
                oldCheatsEnabled = CheatsController.Instance.cheatsEnabled;
                OnCheatsEnabled?.Invoke(CheatsController.Instance.cheatsEnabled);
            }
        }
    }
}