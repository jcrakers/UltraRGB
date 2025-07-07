using HarmonyLib;
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
            WeaponCheck GunCheck = WeaponGameObject.AddComponent<WeaponCheck>();
            WeaponGameObject.hideFlags = HideFlags.HideAndDontSave;
            GunCheck.enabled = true;
            DontDestroyOnLoad(WeaponGameObject);
            ArtemisSupport.Logger.LogInfo($"GunCheck Init");
        }
    }

    public delegate void GunCheckBoolHandler(bool value);
    public delegate void GunCheckIntHandler(int value);
    public delegate void GunCheckFloatHandler(float value);
    public delegate void GunCheckStringHandler(string value);


    public static GunCheckStringHandler OnWeaponChange;
    public static GunCheckStringHandler OnWeaponVariationChange;
    public static GunCheckStringHandler OnWeaponFreshnessChange;
    public static GunCheckFloatHandler OnWeaponFreshnessMeterChange;
    public static GunCheckStringHandler OnFistChange;
    public static GunCheckFloatHandler OnFistCooldownChange;
    public static GunCheckIntHandler OnWeaponSlotChange;
    public static GunCheckBoolHandler OnAlternateChange;


    public string currentWeapon = "";
    public string currentVariation = "";
    public string CurrentFist = "";
    public string weaponFreshness = "";
    public float weaponFreshnessMeter = 0f;
    public float fistCooldown = 0f;
    public int currentSlot = 0;
    public bool isAlternate = false;

    private GunControl gunControl;

    void Update()
    {
        if (GunControl.Instance != null && !OptionsManager.Instance.paused && NewMovement.Instance != null)
        {
            if (gunControl == null)
            {
                gunControl = GunControl.Instance;
            }

            if (GunControl.Instance.currentWeapon != null)
            {
                if (currentWeapon != gunControl.currentWeapon.name)
                {
                    currentWeapon = gunControl.currentWeapon.name;
                    if (currentWeapon.Contains("(Clone)"))
                        currentWeapon = currentWeapon.Replace("(Clone)", "");

                    OnWeaponChange?.Invoke(currentWeapon);

                    isAlternate = FindAlternateWeapon();
                    OnAlternateChange?.Invoke(isAlternate);

                    currentVariation = FindWeaponVariation();
                    OnWeaponVariationChange?.Invoke(currentVariation);
                }
            }

            if ((gunControl.currentWeapon == null || gunControl.currentWeapon.name == null) && currentWeapon != "None")
            {
                currentWeapon = "None";
                OnWeaponChange.Invoke(currentWeapon);
            }

            if (currentSlot != gunControl.currentSlotIndex)
            {
                currentSlot = gunControl.currentSlotIndex;
                OnWeaponSlotChange?.Invoke(currentSlot);
            }

            if (FistControl.Instance != null && FistControl.Instance.currentArmObject != null)
            {
                if (CurrentFist != FistControl.Instance.currentArmObject.name)
                {
                    CurrentFist = FistControl.Instance.currentArmObject.name;
                    if (CurrentFist.Contains("(Clone)"))
                        CurrentFist = CurrentFist.Replace("(Clone)", "");

                    OnFistChange?.Invoke(CurrentFist);
                }

                if (fistCooldown != FistControl.Instance.fistCooldown)
                {
                    fistCooldown = FistControl.Instance.fistCooldown;
                    OnFistCooldownChange?.Invoke(fistCooldown);
                }
            }

            if (WeaponCharges.Instance != null)
            {
                if (fistCooldown != WeaponCharges.Instance.punchStamina)
                {
                    fistCooldown = WeaponCharges.Instance.punchStamina;
                    OnFistCooldownChange?.Invoke(fistCooldown);
                }
            }
            else if (CurrentFist != "None")
            {
                CurrentFist = "None";
                OnFistChange?.Invoke(CurrentFist);
            }

            if (StyleHUD.Instance != null)
            {
                if (weaponFreshness != StyleHUD.Instance.GetFreshnessState(gunControl.currentWeapon).ToString())
                {
                    weaponFreshness = StyleHUD.Instance.GetFreshnessState(gunControl.currentWeapon).ToString();
                    OnWeaponFreshnessChange?.Invoke(weaponFreshness);
                }

                if (weaponFreshnessMeter != Traverse.Create(StyleHUD.Instance).Field("freshnessSliderValue").GetValue<float>())
                {
                    weaponFreshnessMeter = Traverse.Create(StyleHUD.Instance).Field("freshnessSliderValue").GetValue<float>();
                    OnWeaponFreshnessMeterChange?.Invoke(weaponFreshnessMeter);
                }
            }
        }
    }

    private bool FindAlternateWeapon()
    {
        if (gunControl.currentWeapon == null || gunControl.currentWeapon.name == null)
            return false;

        return currentWeapon switch
        {
            _ when currentWeapon.Contains("Alternative") => true,
            _ when currentWeapon.Contains("Hammer") => true,
            _ when currentWeapon.Contains("Sawblade Launcher") => true,
            _ => false
        };
    }
    
    private string FindWeaponVariation()
    {
        return currentWeapon switch
        {
            _ when currentWeapon.Contains("Pierce") => "Blue",
            _ when currentWeapon.Contains("Ricochet") => "Green",
            _ when currentWeapon.Contains("Twirl") => "Red",

            _ when currentWeapon.Contains("Grenade") => "Blue",
            _ when currentWeapon.Contains("Pump") => "Green",
            _ when currentWeapon.Contains(" Saw") => "Red",

            _ when currentWeapon.Contains("Magnet") => "Blue",
            _ when currentWeapon.Contains("Overheat") => "Green",
            _ when currentWeapon.Contains("Zapper") => "Red",

            _ when currentWeapon.Contains("Electric") => "Blue",
            _ when currentWeapon.Contains("Harpoon") => "Green",
            _ when currentWeapon.Contains("Malicious") => "Red",

            _ when currentWeapon.Contains("Freeze") => "Blue",
            _ when currentWeapon.Contains("Cannonball") => "Green",
            _ when currentWeapon.Contains("Napalm") => "Red",
            _ => "None"
        };
    }
}