using UnityEngine;

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
            ArtemisSupport.Logger.LogInfo($"PlayerCheck Init");
        }
    }

    public delegate void PlayerCheckBoolHandler(bool value);
    public delegate void PlayerCheckIntHandler(int value);
    public delegate void PlayerCheckFloatHandler(float value);
    public delegate void PlayerCheckStringHandler(string value);


    public static PlayerCheckIntHandler OnHealthChanged;
    public static PlayerCheckFloatHandler OnHardDamageChanged;
    public static PlayerCheckFloatHandler OnStaminaChanged;
    public static PlayerCheckIntHandler OnWallJumpsChanged;
    public static PlayerCheckFloatHandler OnSpeedChanged;
    //public static PlayerCheckFloatHandler OnComboMeterChanged;
    //public static PlayerCheckStringHandler OnComboMeterRankChanged;
    //public static PlayerCheckFloatHandler OnComboMeterMultiplierChanged;
    public static PlayerCheckBoolHandler OnSlidingChanged;
    public static PlayerCheckBoolHandler OnSlamingChanged;
    public static PlayerCheckBoolHandler OnFallingChanged;
    public static PlayerCheckBoolHandler OnDashingChanged;
    public static PlayerCheckBoolHandler OnDeath;
    

    public int health = 100;
    public float hardDamage = 0f;
    public float stamina = 300f;
    public int wallJumps = 3;
    public float speed = 0f;
    //public float comboMeter = 0f;
    //public string comboMeterRank = "None";
    public bool sliding = false;
    public bool slaming = false;
    public bool falling = false;
    public bool dashing = false;
    public bool dead = false;


    private NewMovement nm;

    void Update()
    {
        if (NewMovement.Instance != null )
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

            if (speed != PlayerTracker.Instance.GetPlayerVelocity(true).magnitude)
            {
                speed = PlayerTracker.Instance.GetPlayerVelocity(true).magnitude;
                OnSpeedChanged?.Invoke(speed);
            }

            /*if (comboMeter != nm.comboMeter)
            {
                comboMeter = nm.comboMeter;
                OnComboMeterChanged?.Invoke(comboMeter);
            }

            if (comboMeterRank != nm.comboMeterRank)
            {
                comboMeterRank = nm.comboMeterRank;
                OnComboMeterRankChanged?.Invoke(comboMeterRank);
            }

            if (comboMeterMultiplier != nm.comboMeterMultiplier)
            {
                comboMeterMultiplier = nm.comboMeterMultiplier;
                OnComboMeterMultiplierChanged?.Invoke(comboMeterMultiplier);
            }*/

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

            if (dashing != nm.boost)
            {
                dashing = nm.boost;
                OnDashingChanged?.Invoke(dashing);
            }

            if (falling != nm.falling)
            {
                falling = nm.falling;
                OnFallingChanged?.Invoke(falling);
            }
        }
    }
}