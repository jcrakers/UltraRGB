using HarmonyLib;
using UnityEngine;

namespace UltraRGB.Components;

public class WeaponCheck : BaseCheck
{
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


    protected override void RateLimitedUpdate()
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

        if (optionsManagerCache.paused)
        {
            return;
        }

        if (gunControlCache == null)
        {
            if (gunControlCache.currentWeapon != null)
            {
                if (currentWeapon != gunControlCache.currentWeapon.name)
                {
                    currentWeapon = gunControlCache.currentWeapon.name;
                    if (currentWeapon.Contains("(Clone)"))
                        currentWeapon = currentWeapon.Replace("(Clone)", "");

                    UltraRGB.QueueUpdate("CurrentWeapon", currentWeapon, "Weapon");

                    isAlternate = FindAlternateWeapon();
                    UltraRGB.QueueUpdate("IsAlternate", isAlternate, "Weapon");

                    currentVariation = FindWeaponVariation();
                    UltraRGB.QueueUpdate("CurrentVariation", currentVariation, "Weapon");
                }
            }
            else if (gunControlCache.currentWeapon.name == null && currentWeapon != "None")
            {
                currentWeapon = "None";
                UltraRGB.QueueUpdate("CurrentWeapon", currentWeapon, "Weapon");
            }

            if (currentSlot != gunControlCache.currentSlotIndex)
            {
                currentSlot = gunControlCache.currentSlotIndex;
                UltraRGB.QueueUpdate("CurrentSlot", currentSlot, "Weapon");
            }
        }

        if (fistControlCache != null && fistControlCache.currentArmObject != null)
        {
            if (CurrentFist != fistControlCache.currentArmObject.name)
            {
                CurrentFist = fistControlCache.currentArmObject.name;
                if (CurrentFist.Contains("(Clone)"))
                        CurrentFist = CurrentFist.Replace("(Clone)", "");

                UltraRGB.QueueUpdate("CurrentFist", CurrentFist, "Weapon");
            }

            /*if (fistCooldown != fistControlCache.fistCooldown)
            {
                fistCooldown = fistControlCache.fistCooldown;
                UltraRGB.QueueUpdate("WeaponFistCooldown", fistCooldown);
            }*/
        }

        if (weaponChargesCache != null)
        {
            if (fistCooldown != weaponChargesCache.punchStamina)
            {
                fistCooldown = weaponChargesCache.punchStamina;
                UltraRGB.QueueUpdate("FistCooldown", fistCooldown * 50f, "Weapon");
            }
        }
        else if (CurrentFist != "None")
        {
            CurrentFist = "None";
            UltraRGB.QueueUpdate("CurrentFist", CurrentFist, "Weapon");
        }

        if (styleHUDCache != null)
        {
            if (weaponFreshness != styleHUDCache.GetFreshnessState(gunControlCache.currentWeapon).ToString())
            {
                weaponFreshness = styleHUDCache.GetFreshnessState(gunControlCache.currentWeapon).ToString();
                UltraRGB.QueueUpdate("WeaponFreshness", weaponFreshness, "Weapon");
            }

            if (weaponFreshnessMeter != lastWeaponFreshnessMeter.GetValue<float>())
            {
                weaponFreshnessMeter = lastWeaponFreshnessMeter.GetValue<float>();
                UltraRGB.QueueUpdate("WeaponFreshnessMeter", Mathf.InverseLerp(0.5f, 10f, weaponFreshnessMeter) * 150f, "Weapon");
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