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
            GameObject gameObject = new GameObject("OtherCheck");
            OtherCheck otherCheck = gameObject.AddComponent<OtherCheck>();
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            otherCheck.enabled = true;
            DontDestroyOnLoad(gameObject);
        }
    }
    public delegate void DeathPauseHandler(bool state);
    public static DeathPauseHandler OnDeath;
    public static DeathPauseHandler OnPause;
    public static bool oldPaused = false;
    public static bool oldDead = false;

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
    }
}