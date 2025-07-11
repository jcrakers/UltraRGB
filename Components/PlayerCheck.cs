using UnityEngine;
using HarmonyLib;


namespace UltrakillArtemisMod.Components;

public class PlayerCheck : MonoBehaviour
{
    private static bool initialized;
    public static void Init()
    {
        if (!initialized)
        {
            initialized = true;
            GameObject playerGameObject = new GameObject("OtherCheck");
            PlayerCheck playerCheck = playerGameObject.AddComponent<PlayerCheck>();
            playerGameObject.hideFlags = HideFlags.HideAndDontSave;
            playerCheck.enabled = true;
            DontDestroyOnLoad(playerGameObject);
            //ArtemisSupport.Logger.LogInfo($"PlayerCheck Init");
        }
    }

    public delegate void PlayerCheckBoolHandler(bool value);
    public delegate void PlayerCheckIntHandler(int value);
    public delegate void PlayerCheckFloatHandler(float value);


    public static PlayerCheckIntHandler OnHealthChanged;
    public static PlayerCheckFloatHandler OnHardDamageChanged;
    public static PlayerCheckFloatHandler OnStaminaChanged;
    public static PlayerCheckIntHandler OnWallJumpsChanged;
    public static PlayerCheckFloatHandler OnSpeedChanged;
    public static PlayerCheckFloatHandler OnStyleMeterChanged;
    public static PlayerCheckIntHandler OnStyleMeterRankChanged;
    public static PlayerCheckFloatHandler OnStyleMeterMultiplierChanged;
    public static PlayerCheckBoolHandler OnSlidingChanged;
    public static PlayerCheckBoolHandler OnSlamingChanged;
    public static PlayerCheckFloatHandler OnSlamForceChanged;
    public static PlayerCheckBoolHandler OnFallingChanged;
    public static PlayerCheckBoolHandler OnWipLashingChanged;
    public static PlayerCheckBoolHandler OnDeath;


    public int health = 100;
    public float hardDamage = 0f;
    public float stamina = 300f;
    public int wallJumps = 3;
    public float speed = 0f;
    public static float styleMeter = 0f; private float lastStyleMeter = 0f;
    public int styleMeterRank = 0; private bool styleMeterAboveZero = false;
    public float styleMeterMultiplier = 1f;
    public bool sliding = false;
    public bool slaming = false;
    public float slamForce = 0f;
    public bool falling = false;
    public bool wipLashing = false;
    public bool dead = false;

    private GunControl gunControlCache;
    private OptionsManager optionsManagerCache;
    private PlayerTracker playerTrackerCache;
    private NewMovement newMovementCache;
    private StyleHUD styleHUDCache;
    private StyleCalculator styleCalculatorCache;
    private HookArm hookArmCache;

    void Update()
    {
        if (gunControlCache == null || playerTrackerCache == null || newMovementCache == null || styleHUDCache == null || styleCalculatorCache == null || hookArmCache == null)
        {
            gunControlCache = GunControl.Instance;
            optionsManagerCache = OptionsManager.Instance;
            playerTrackerCache = PlayerTracker.Instance;
            newMovementCache = NewMovement.Instance;
            styleHUDCache = StyleHUD.Instance;
            styleCalculatorCache = StyleCalculator.Instance;
            hookArmCache = HookArm.Instance;
        }

        if (gunControlCache != null && !optionsManagerCache.paused && newMovementCache != null && styleHUDCache != null && styleCalculatorCache != null && hookArmCache != null)
        {
            if (dead != newMovementCache.dead)
            {
                dead = newMovementCache.dead;
                OnDeath?.Invoke(dead);
            }

            if (health != newMovementCache.hp)
            {
                health = newMovementCache.hp;
                OnHealthChanged?.Invoke(health);
            }

            if (hardDamage != newMovementCache.antiHp)
            {
                hardDamage = newMovementCache.antiHp;
                OnHardDamageChanged?.Invoke(hardDamage);
            }

            if (stamina != newMovementCache.boostCharge)
            {
                stamina = newMovementCache.boostCharge;
                OnStaminaChanged?.Invoke(stamina);
            }

            if (wallJumps != newMovementCache.currentWallJumps)
            {
                wallJumps = newMovementCache.currentWallJumps;
                OnWallJumpsChanged?.Invoke((wallJumps - 3) * -1);
            }

            if (playerTrackerCache != null)
            {
                if (speed != playerTrackerCache.GetPlayerVelocity(true).magnitude)
                {
                    speed = playerTrackerCache.GetPlayerVelocity(true).magnitude;
                    OnSpeedChanged?.Invoke(speed);
                }
            }

            if (styleHUDCache != null)
            {
                lastStyleMeter = Traverse.Create(styleHUDCache).Field("currentMeter").GetValue<float>();
                if (styleMeter != lastStyleMeter)
                {
                    styleMeter = lastStyleMeter;
                    OnStyleMeterChanged?.Invoke(styleMeter);

                    if (styleMeter > 0f && !styleMeterAboveZero)
                    {
                        styleMeterAboveZero = true;
                        OnStyleMeterRankChanged?.Invoke(styleMeterRank);
                    }
                    if (styleMeter <= 0f && styleMeterAboveZero && styleMeterRank == 0)
                    {
                        styleMeterAboveZero = false;
                        OnStyleMeterRankChanged?.Invoke(styleMeterRank);
                    }
                }

                if (styleMeterRank != styleHUDCache.rankIndex)
                {
                    styleMeterRank = styleHUDCache.rankIndex;
                    OnStyleMeterRankChanged?.Invoke(styleMeterRank);
                }
            }

            if (styleCalculatorCache != null)
            {
                if (styleMeterMultiplier != styleCalculatorCache.airTime)
                {
                    styleMeterMultiplier = styleCalculatorCache.airTime;
                    OnStyleMeterMultiplierChanged?.Invoke(styleMeterMultiplier);
                }
            }

            if (sliding != newMovementCache.sliding)
            {
                sliding = newMovementCache.sliding;
                OnSlidingChanged?.Invoke(sliding);
            }

            if (slaming != newMovementCache.gc.heavyFall)
            {
                slaming = newMovementCache.gc.heavyFall;
                OnSlamingChanged?.Invoke(slaming);
            }

            if (slamForce != newMovementCache.slamForce)
            {
                slamForce = newMovementCache.slamForce;
                OnSlamForceChanged?.Invoke(slamForce);
            }

            if (falling != newMovementCache.falling)
            {
                falling = newMovementCache.falling;
                OnFallingChanged?.Invoke(falling);
            }

            if (hookArmCache != null)
            {
                if (wipLashing != hookArmCache.beingPulled)
                {
                    wipLashing = hookArmCache.beingPulled;
                    OnWipLashingChanged?.Invoke(wipLashing);
                }
            }
        }
    }
}