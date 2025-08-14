using UnityEngine;
using System.Collections.Generic;

namespace UltraRGB.Components;

public class RunCheck : BaseCheck
{
    public static bool levelCompleted = false;
    public static bool challengeDone = false;
    public static float time = 0f;
    public static float kills = 0f;
    public static float style = 0f;
    public static string timeRank = "Unknown";
    public static string killRank = "Unknown";
    public static string styleRank = "Unknown";

    public static float cgTime = 0f;
    public static float cgWave = 0f;
    public static float cgEnemiesLeft = 0f;
    public static float cgMaxEnemies = 0f;


    private StatsManager statsManagerCache;
    private OptionsManager optionsManagerCache;
    private ChallengeManager challengeManagerCache;
    private EndlessGrid endlessGridCache;
    private EnemyTracker enemyTrackerCache;

    protected override void RateLimitedUpdate()
    {
        if (statsManagerCache == null || optionsManagerCache == null || challengeManagerCache == null || endlessGridCache == null || enemyTrackerCache == null)
        {
            statsManagerCache = StatsManager.Instance;
            optionsManagerCache = OptionsManager.Instance;
            challengeManagerCache = ChallengeManager.Instance;
            endlessGridCache = EndlessGrid.Instance;
            enemyTrackerCache = EnemyTracker.Instance;
        }

        if (statsManagerCache != null && !optionsManagerCache.paused)
        {
            if (levelCompleted != statsManagerCache.infoSent)
            {
                levelCompleted = statsManagerCache.infoSent;
                if (statsManagerCache.infoSent)
                {
                    UltraRGB.QueueUpdate("RunLevelCompleted", statsManagerCache.infoSent);
                }
            }
        }

        if (challengeManagerCache != null && !optionsManagerCache.paused)
        {
            if (challengeDone != challengeManagerCache.challengeDone)
            {
                challengeDone = challengeManagerCache.challengeDone;
                UltraRGB.QueueUpdate("RunChallengeCompleted", challengeManagerCache.challengeDone);
            }
        }

        if (statsManagerCache != null && !optionsManagerCache.paused && !statsManagerCache.endlessMode) {
            if (timeRank != GetRank(0))
            {
                timeRank = GetRank(0);
                UltraRGB.QueueUpdate("RunTimeRank", GetRank(0));
            }
            if (killRank != GetRank(1))
            {
                killRank = GetRank(1);
                UltraRGB.QueueUpdate("RunKillRank", GetRank(1));
            }
            if (styleRank != GetRank(2))
            {
                styleRank = GetRank(2);
                UltraRGB.QueueUpdate("RunStyleRank", GetRank(2));
            }
        }

        if (statsManagerCache != null && !optionsManagerCache.paused && !statsManagerCache.endlessMode)
        {
            if (time != statsManagerCache.seconds)
            {
                time = statsManagerCache.seconds;
                UltraRGB.QueueUpdate("RunTime", statsManagerCache.seconds);
            }
            if (kills != statsManagerCache.kills)
            {
                kills = statsManagerCache.kills;
                UltraRGB.QueueUpdate("RunKills", statsManagerCache.kills);
            }
            if (style != statsManagerCache.stylePoints)
            {
                style = statsManagerCache.stylePoints;
                UltraRGB.QueueUpdate("RunStyle", statsManagerCache.stylePoints);
            }
        }

        if (statsManagerCache != null && !optionsManagerCache.paused && statsManagerCache.endlessMode)
        {
            if (cgTime != statsManagerCache.seconds)
            {
                cgTime = statsManagerCache.seconds;
                UltraRGB.QueueUpdate("CyberGrindTime", statsManagerCache.seconds);
            }
            if (cgWave != endlessGridCache.currentWave)
            {
                cgWave = endlessGridCache.currentWave;
                UltraRGB.QueueUpdate("CyberGrindWave", endlessGridCache.currentWave);
            }
            if (cgEnemiesLeft != enemyTrackerCache.GetCurrentEnemies().Count)
            {
                cgEnemiesLeft = enemyTrackerCache.GetCurrentEnemies().Count;
                UltraRGB.QueueUpdate("CyberGrindEnemiesLeft", enemyTrackerCache.GetCurrentEnemies().Count);
            }
            if (cgMaxEnemies != endlessGridCache.tempEnemyAmount)
            {
                cgMaxEnemies = endlessGridCache.tempEnemyAmount;
                UltraRGB.QueueUpdate("CyberGrindMaxEnemies", endlessGridCache.tempEnemyAmount);
            }
        }
    }
    private string GetRank(int type)
    {
        var ranks = new List<string> { "D", "C", "B", "A", "S" };
        if (type == 0)
        {
            if (statsManagerCache.seconds >= statsManagerCache.timeRanks[0])
                return ranks[0];
            else if (statsManagerCache.seconds >= statsManagerCache.timeRanks[1])
                return ranks[1];
            else if (statsManagerCache.seconds >= statsManagerCache.timeRanks[2])
                return ranks[2];
            else if (statsManagerCache.seconds >= statsManagerCache.timeRanks[3])
                return ranks[3];
            else
                return ranks[4];
        }
        if (type == 1)
        {
            if (statsManagerCache.kills < statsManagerCache.killRanks[0])
                return ranks[0];
            else if (statsManagerCache.kills < statsManagerCache.killRanks[1])
                return ranks[1];
            else if (statsManagerCache.kills < statsManagerCache.killRanks[2])
                return ranks[2];
            else if (statsManagerCache.kills < statsManagerCache.killRanks[3])
                return ranks[3];
            else
                return ranks[4];
        }
        if (type == 2)
        {
            if (statsManagerCache.stylePoints < statsManagerCache.styleRanks[0])
                return ranks[0];
            else if (statsManagerCache.stylePoints < statsManagerCache.styleRanks[1])
                return ranks[1];
            else if (statsManagerCache.stylePoints < statsManagerCache.styleRanks[2])
                return ranks[2];
            else if (statsManagerCache.stylePoints < statsManagerCache.styleRanks[3])
                return ranks[3];
            else
                return ranks[4];
        }
        return "Unknown";
    }
}