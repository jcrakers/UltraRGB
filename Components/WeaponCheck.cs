using HarmonyLib;
using UnityEngine;

namespace UltraRGB.Components;

public class WeaponCheck : MonoBehaviour
{
    private static bool initialized;
    public static void Init()
    {
        if (!initialized)
        {
            initialized = true;
            GameObject WeaponGameObject = new("OtherCheck");
            WeaponCheck GunCheck = WeaponGameObject.AddComponent<WeaponCheck>();
            WeaponGameObject.hideFlags = HideFlags.HideAndDontSave;
            GunCheck.enabled = true;
            DontDestroyOnLoad(WeaponGameObject);
            //UltraRGB.Logger.LogInfo($"GunCheck Init");
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


    public static string currentWeapon = "";
    public static string currentVariation = "";
    public static string CurrentFist = "";
    public static string weaponFreshness = "";
    public static float weaponFreshnessMeter = 0f; private Traverse lastWeaponFreshnessMeter;
    public static float fistCooldown = 0f;
    public static int currentSlot = 0;
    public static bool isAlternate = false;

    private GunControl gunControlCache;
    private OptionsManager optionsManagerCache;
    private NewMovement newMovementCache;
    private FistControl fistControlCache;
    private WeaponCharges weaponChargesCache;
    private StyleHUD styleHUDCache;


    private void LateUpdate()
    {
        if (gunControlCache == null || optionsManagerCache == null || newMovementCache == null || fistControlCache == null || weaponChargesCache == null || styleHUDCache == null)
        {
            gunControlCache = GunControl.Instance;
            optionsManagerCache = OptionsManager.Instance;
            newMovementCache = NewMovement.Instance;
            fistControlCache = FistControl.Instance;
            weaponChargesCache = WeaponCharges.Instance;
            styleHUDCache = StyleHUD.Instance;
            lastWeaponFreshnessMeter = Traverse.Create(styleHUDCache).Field("freshnessSliderValue");
        }

        if (gunControlCache != null && !optionsManagerCache.paused && newMovementCache != null && fistControlCache != null && weaponChargesCache != null && styleHUDCache != null)
        {
            if (gunControlCache.currentWeapon != null)
            {
                if (currentWeapon != gunControlCache.currentWeapon.name)
                {
                    currentWeapon = gunControlCache.currentWeapon.name;
                    if (currentWeapon.Contains("(Clone)"))
                        currentWeapon = currentWeapon.Replace("(Clone)", "");

                    OnWeaponChange?.Invoke(currentWeapon);

                    isAlternate = FindAlternateWeapon();
                    OnAlternateChange?.Invoke(isAlternate);

                    currentVariation = FindWeaponVariation();
                    OnWeaponVariationChange?.Invoke(currentVariation);
                }
            }

            if ((gunControlCache.currentWeapon == null || gunControlCache.currentWeapon.name == null) && currentWeapon != "None")
            {
                currentWeapon = "None";
                OnWeaponChange.Invoke(currentWeapon);
            }

            if (currentSlot != gunControlCache.currentSlotIndex)
            {
                currentSlot = gunControlCache.currentSlotIndex;
                OnWeaponSlotChange?.Invoke(currentSlot);
            }

            if (fistControlCache != null && fistControlCache.currentArmObject != null)
            {
                if (CurrentFist != fistControlCache.currentArmObject.name)
                {
                    CurrentFist = fistControlCache.currentArmObject.name;
                    if (CurrentFist.Contains("(Clone)"))
                        CurrentFist = CurrentFist.Replace("(Clone)", "");

                    OnFistChange?.Invoke(CurrentFist);
                }

                if (fistCooldown != fistControlCache.fistCooldown)
                {
                    fistCooldown = fistControlCache.fistCooldown;
                    OnFistCooldownChange?.Invoke(fistCooldown);
                }
            }

            if (weaponChargesCache != null)
            {
                if (fistCooldown != weaponChargesCache.punchStamina)
                {
                    fistCooldown = weaponChargesCache.punchStamina;
                    OnFistCooldownChange?.Invoke(fistCooldown);
                }
            }
            else if (CurrentFist != "None")
            {
                CurrentFist = "None";
                OnFistChange?.Invoke(CurrentFist);
            }

            if (styleHUDCache != null)
            {
                if (weaponFreshness != styleHUDCache.GetFreshnessState(gunControlCache.currentWeapon).ToString())
                {
                    weaponFreshness = styleHUDCache.GetFreshnessState(gunControlCache.currentWeapon).ToString();
                    OnWeaponFreshnessChange?.Invoke(weaponFreshness);
                }

                if (weaponFreshnessMeter != lastWeaponFreshnessMeter.GetValue<float>())
                {
                    weaponFreshnessMeter = lastWeaponFreshnessMeter.GetValue<float>();
                    OnWeaponFreshnessMeterChange?.Invoke(weaponFreshnessMeter);
                }
            }
        }
    }

    private bool FindAlternateWeapon()
    {
        if (gunControlCache.currentWeapon == null || gunControlCache.currentWeapon.name == null)
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
            _ when currentWeapon.Equals("Revolver Pierce") => "Blue",
            _ when currentWeapon.Equals("Revolver Ricochet") => "Green",
            _ when currentWeapon.Equals("Revolver Twirl") => "Red",

            _ when currentWeapon.Equals("Alternative Revolver Pierce") => "Blue",
            _ when currentWeapon.Equals("Alternative Revolver Ricochet") => "Green",
            _ when currentWeapon.Equals("Alternative Revolver Twirl") => "Red",

            _ when currentWeapon.Equals("Shotgun Grenade") => "Blue",
            _ when currentWeapon.Equals("Shotgun Pump") => "Green",
            _ when currentWeapon.Equals("Shotgun Saw") => "Red",

            _ when currentWeapon.Equals("Hammer Grenade") => "Blue",
            _ when currentWeapon.Equals("Hammer Pump") => "Green",
            _ when currentWeapon.Equals("Hammer Saw") => "Red",

            _ when currentWeapon.Equals("Nailgun Magnet") => "Blue",
            _ when currentWeapon.Equals("Nailgun Overheat") => "Green",
            _ when currentWeapon.Equals("Nailgun Zapper") => "Red",

            _ when currentWeapon.Equals("Sawblade Launcher Magnet") => "Blue",
            _ when currentWeapon.Equals("Sawblade Launcher Overheat") => "Green",
            _ when currentWeapon.Equals("Sawblade Launcher Zapper") => "Red",

            _ when currentWeapon.Equals("Railcannon Electric") => "Blue",
            _ when currentWeapon.Equals("Railcannon Harpoon") => "Green",
            _ when currentWeapon.Equals("Railcannon Malicious") => "Red",

            _ when currentWeapon.Equals("Rocket Launcher Freeze") => "Blue",
            _ when currentWeapon.Equals("Rocket Launcher Cannonball") => "Green",
            _ when currentWeapon.Equals("Rocket Launcher Napalm") => "Red",
            _ => "None"
        };
    }
}