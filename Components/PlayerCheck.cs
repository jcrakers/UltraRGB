using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace UltraRGB.Components;

public class PlayerCheck : BaseCheck
{
    public static int health = 100;
    public static float hardDamage = 0f;
    public static float stamina = 300f;
    public static int wallJumps = 3;
    public static float speed = 0f;
    public static float styleMeter = 0f; private Traverse lastStyleMeter;
    public static int styleMeterRank = 0; private bool styleMeterAboveZero = false; private readonly List<string> styleMeterRankNameList = ["None", "Destructive", "Chaotic", "Brutal", "Anarchic", "Supreme", "SSadistic", "SSShitstorm", "ULTRAKILL"];
    public static float styleMeterMultiplier = 1f;
    public static bool sliding = false;
    public static bool slaming = false;
    public static float slamForce = 0f;
    public static bool falling = false;
    public static bool wipLashing = false;
    public static bool dead = false;

    private GunControl gunControlCache;
    private OptionsManager optionsManagerCache;
    private PlayerTracker playerTrackerCache;
    private NewMovement newMovementCache;
    private StyleHUD styleHUDCache;
    private StyleCalculator styleCalculatorCache;
    private HookArm hookArmCache;

    protected override void RateLimitedUpdate()
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
            lastStyleMeter = Traverse.Create(styleHUDCache).Field("currentMeter");
        }

        if (gunControlCache != null && !optionsManagerCache.paused && newMovementCache != null && styleHUDCache != null && styleCalculatorCache != null && hookArmCache != null)
        {
            if (dead != newMovementCache.dead)
            {
                dead = newMovementCache.dead;
                UltraRGB.QueueUpdate("PlayerDead", dead);
            }

            if (health != newMovementCache.hp)
            {
                health = newMovementCache.hp;
                UltraRGB.QueueUpdate("PlayerHealth", health);
            }

            if (hardDamage != newMovementCache.antiHp)
            {
                hardDamage = newMovementCache.antiHp;
                UltraRGB.QueueUpdate("PlayerHardDamage", hardDamage);
            }

            if (stamina != newMovementCache.boostCharge)
            {
                stamina = newMovementCache.boostCharge;
                UltraRGB.QueueUpdate("PlayerStamina", stamina);
            }

            if (wallJumps != newMovementCache.currentWallJumps)
            {
                wallJumps = newMovementCache.currentWallJumps;
                UltraRGB.QueueUpdate("PlayerWallJumps", (wallJumps - 3) * -1);
            }

            if (playerTrackerCache != null)
            {
                if (Mathf.Abs(speed - playerTrackerCache.GetPlayerVelocity(true).magnitude) > 0.01f)
                {
                    speed = playerTrackerCache.GetPlayerVelocity(true).magnitude;
                    UltraRGB.QueueUpdate("PlayerSpeed", speed);
                }
            }

            if (styleHUDCache != null)
            {
                if (styleMeter != lastStyleMeter.GetValue<float>())
                {
                    styleMeter = lastStyleMeter.GetValue<float>();
                    UltraRGB.QueueUpdate("PlayerStyleMeter", Mathf.Clamp(styleMeter, 0, styleHUDCache.currentRank.maxMeter) / styleHUDCache.currentRank.maxMeter * 100f);

                        if (styleMeter > 0f && !styleMeterAboveZero)
                        {
                            styleMeterAboveZero = true;
                            UltraRGB.QueueUpdate("PlayerStyleMeterRank", styleMeterRankNameList[styleMeterRank]);
                        }
                        if (styleMeter <= 0f && styleMeterAboveZero && styleMeterRank == 0)
                        {
                            styleMeterAboveZero = false;
                            UltraRGB.QueueUpdate("PlayerStyleMeterRank", styleMeterRankNameList[styleMeterRank]);
                        }
                }

                if (styleMeterRank != styleHUDCache.rankIndex)
                {
                    styleMeterRank = styleHUDCache.rankIndex;
                    UltraRGB.QueueUpdate("PlayerStyleMeterRank", styleMeterRankNameList[styleMeterRank]);
                }
            }

            if (styleCalculatorCache != null)
            {
                if (styleMeterMultiplier != styleCalculatorCache.airTime)
                {
                    styleMeterMultiplier = styleCalculatorCache.airTime;
                    UltraRGB.QueueUpdate("PlayerStyleMeterMultiplier", Math.Round(styleMeterMultiplier, 2));
                }
            }

            if (sliding != newMovementCache.sliding)
            {
                sliding = newMovementCache.sliding;
                UltraRGB.QueueUpdate("PlayerSliding", sliding);
            }

            if (slaming != newMovementCache.gc.heavyFall)
            {
                slaming = newMovementCache.gc.heavyFall;
                UltraRGB.QueueUpdate("PlayerSlaming", slaming);
            }

            if (slamForce != newMovementCache.slamForce)
            {
                slamForce = newMovementCache.slamForce;
                UltraRGB.QueueUpdate("PlayerSlamForce", slamForce);
            }

            if (falling != newMovementCache.falling)
            {
                falling = newMovementCache.falling;
                UltraRGB.QueueUpdate("PlayerFalling", falling);
            }

            if (hookArmCache != null)
            {
                if (wipLashing != hookArmCache.beingPulled)
                {
                    wipLashing = hookArmCache.beingPulled;
                    UltraRGB.QueueUpdate("PlayerWipLashing", wipLashing);
                }
            }
        }
    }
}

