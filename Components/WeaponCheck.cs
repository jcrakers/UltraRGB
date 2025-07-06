using UnityEngine;

namespace UltrakillArtemisMod.Components;

public class WeaponCheck : MonoBehaviour
{
    private static bool initialized;
    public static void Init()
    {
        if (!initialized)
        {
            initialized = true;
            GameObject WeaponGameObject = new GameObject("OtherCheck");
            WeaponCheck playerCheck = WeaponGameObject.AddComponent<WeaponCheck>();
            WeaponGameObject.hideFlags = HideFlags.HideAndDontSave;
            playerCheck.enabled = true;
            DontDestroyOnLoad(WeaponGameObject);
            ArtemisSupport.Logger.LogInfo($"PlayerCheck Init");
        }
    }
}