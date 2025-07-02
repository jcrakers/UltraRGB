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

    void Update()
    {
        if (MonoSingleton<OptionsManager>.Instance != null)
        {
            if (paused != OptionsManager.Instance.paused)
            {
                paused = OptionsManager.Instance.paused;
                OnPause?.Invoke(OptionsManager.Instance.paused);
            }
        }

        if (CheatsController.Instance != null)
        {
            if (cheatsEnabled != CheatsController.Instance.cheatsEnabled)
            {
                cheatsEnabled = CheatsController.Instance.cheatsEnabled;
                OnCheatsEnabled?.Invoke(CheatsController.Instance.cheatsEnabled);
            }
        }
    }
}