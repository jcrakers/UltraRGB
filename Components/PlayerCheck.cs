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
                UltraRGB.QueueUpdate("Dead", dead, "Player");
            }

            if (health != newMovementCache.hp)
            {
                health = newMovementCache.hp;
                UltraRGB.QueueUpdate("Health", health, "Player");
            }

            if (hardDamage != newMovementCache.antiHp)
            {
                hardDamage = newMovementCache.antiHp;
                UltraRGB.QueueUpdate("HardDamage", hardDamage, "Player");
            }

            if (stamina != newMovementCache.boostCharge)
            {
                stamina = newMovementCache.boostCharge;
                UltraRGB.QueueUpdate("Stamina", stamina, "Player");
            }

            if (wallJumps != newMovementCache.currentWallJumps)
            {
                wallJumps = newMovementCache.currentWallJumps;
                UltraRGB.QueueUpdate("RemainingWallJumps", (wallJumps - 3) * -1, "Player");
            }

            if (playerTrackerCache != null)
            {
                if (speed != Math.Round(playerTrackerCache.GetPlayerVelocity(true).magnitude, 5))
                {
                    speed = (float)Math.Round(playerTrackerCache.GetPlayerVelocity(true).magnitude, 5);
                    UltraRGB.QueueUpdate("Speed", speed, "Player");
                }
            }

            if (styleHUDCache != null)
            {
                if (styleMeter != lastStyleMeter.GetValue<float>())
                {
                    styleMeter = lastStyleMeter.GetValue<float>();
                    UltraRGB.QueueUpdate("StyleMeter", Mathf.Clamp(styleMeter, 0, styleHUDCache.currentRank.maxMeter) / styleHUDCache.currentRank.maxMeter * 100f, "Player");


                    if (styleMeterRank != styleHUDCache.rankIndex)
                    {
                        styleMeterRank = styleHUDCache.rankIndex;


                        if (styleMeter > 0f && !styleMeterAboveZero)
                        {
                            styleMeterAboveZero = true;
                            UltraRGB.QueueUpdate("StyleMeterRank", styleMeterRank + 1, "Player");
                            UltraRGB.QueueUpdate("StyleMeterRankName", styleMeterRankNameList[styleMeterRank + 1], "Player");
                        }
                        if (styleMeter <= 0f && styleMeterAboveZero && styleMeterRank == 0)
                        {
                            styleMeterAboveZero = false;
                            UltraRGB.QueueUpdate("StyleMeterRank", styleMeterRank, "Player");
                            UltraRGB.QueueUpdate("StyleMeterRankName", styleMeterRankNameList[styleMeterRank], "Player");
                        }
                    }
                }
            }

            if (styleCalculatorCache != null)
            {
                if (styleMeterMultiplier != styleCalculatorCache.airTime)
                {
                    styleMeterMultiplier = styleCalculatorCache.airTime;
                    UltraRGB.QueueUpdate("StyleMultiplier", Math.Round(styleMeterMultiplier, 2), "Player");
                }
            }

            if (sliding != newMovementCache.sliding)
            {
                sliding = newMovementCache.sliding;
                UltraRGB.QueueUpdate("Sliding", sliding, "Player");
            }

            if (slaming != newMovementCache.gc.heavyFall)
            {
                slaming = newMovementCache.gc.heavyFall;
                UltraRGB.QueueUpdate("Slaming", slaming, "Player");
            }

            if (slamForce != newMovementCache.slamForce)
            {
                slamForce = newMovementCache.slamForce;
                UltraRGB.QueueUpdate("SlamForce", slamForce, "Player");
            }

            if (falling != newMovementCache.falling)
            {
                falling = newMovementCache.falling;
                UltraRGB.QueueUpdate("Airborne", falling, "Player");
            }

            if (hookArmCache != null)
            {
                if (wipLashing != hookArmCache.beingPulled)
                {
                    wipLashing = hookArmCache.beingPulled;
                    UltraRGB.QueueUpdate("WipLashing", wipLashing, "Player");
                }
            }
        }
    }
}

