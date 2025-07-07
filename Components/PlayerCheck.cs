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

    private NewMovement nm;

    void Update()
    {
        if (NewMovement.Instance != null && !OptionsManager.Instance.paused)
        {
            if (nm == null)
            {
                nm = NewMovement.Instance;
            }
    
            if (dead != nm.dead)
            {
                dead = nm.dead;
                OnDeath?.Invoke(dead);
            }

            if (health != nm.hp)
            {
                health = nm.hp;
                OnHealthChanged?.Invoke(health);
            }

            if (hardDamage != nm.antiHp)
            {
                hardDamage = nm.antiHp;
                OnHardDamageChanged?.Invoke(hardDamage);
            }

            if (stamina != nm.boostCharge)
            {
                stamina = nm.boostCharge;
                OnStaminaChanged?.Invoke(stamina);
            }

            if (wallJumps != nm.currentWallJumps)
            {
                wallJumps = nm.currentWallJumps;
                OnWallJumpsChanged?.Invoke((wallJumps - 3) * -1);
            }

            if (PlayerTracker.Instance != null)
            {
                if (speed != PlayerTracker.Instance.GetPlayerVelocity(true).magnitude)
                {
                    speed = PlayerTracker.Instance.GetPlayerVelocity(true).magnitude;
                    OnSpeedChanged?.Invoke(speed);
                }
            }

            if (StyleHUD.Instance != null)
            {
                lastStyleMeter = Traverse.Create(StyleHUD.Instance).Field("currentMeter").GetValue<float>();
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

                if (styleMeterRank != StyleHUD.Instance.rankIndex)
                {
                    styleMeterRank = StyleHUD.Instance.rankIndex;
                    OnStyleMeterRankChanged?.Invoke(styleMeterRank);
                } 
            }

            if (StyleCalculator.Instance != null)
            {
                if (styleMeterMultiplier != StyleCalculator.Instance.airTime)
                {
                    styleMeterMultiplier = StyleCalculator.Instance.airTime;
                    OnStyleMeterMultiplierChanged?.Invoke(styleMeterMultiplier);
                }
            }

            if (sliding != nm.sliding)
            {
                sliding = nm.sliding;
                OnSlidingChanged?.Invoke(sliding);
            }

            if (slaming != nm.gc.heavyFall)
            {
                slaming = nm.gc.heavyFall;
                OnSlamingChanged?.Invoke(slaming);
            }

            if (slamForce != nm.slamForce)
            {
                slamForce = nm.slamForce;
                OnSlamForceChanged?.Invoke(slamForce);
            }

            if (falling != nm.falling)
            {
                falling = nm.falling;
                OnFallingChanged?.Invoke(falling);
            }

            if (HookArm.Instance != null)
            {
                if (wipLashing != HookArm.Instance.beingPulled)
                {
                    wipLashing = HookArm.Instance.beingPulled;
                    OnWipLashingChanged?.Invoke(wipLashing);
                }
            }
        }
    }
}