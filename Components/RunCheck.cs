using UnityEngine;

namespace UltrakillArtemisMod.Components;

public class RunCheck : MonoBehaviour
{
    private static bool initialized;
    public static void Init()
    {
        if (!initialized)
        {
            initialized = true;
            GameObject runGameObject = new GameObject("RunCheck");
            RunCheck RunCheck = runGameObject.AddComponent<RunCheck>();
            runGameObject.hideFlags = HideFlags.HideAndDontSave;
            RunCheck.enabled = true;
            DontDestroyOnLoad(runGameObject);
        }
    }

    public delegate void RunMarkHandler(bool completed);

    public static RunMarkHandler OnRunCompleted;

    public static RunMarkHandler OnChallenge;

    public bool levelCompleted = false;
    public bool challengeDone = false;


    private StatsManager statsManagerCache;
    private OptionsManager optionsManagerCache;
    private ChallengeManager challengeManagerCache;

    void Update()
    {
        if (statsManagerCache == null || optionsManagerCache == null || challengeManagerCache == null)
        {
            statsManagerCache = StatsManager.Instance;
            optionsManagerCache = OptionsManager.Instance;
            challengeManagerCache = ChallengeManager.Instance;
        }

        if (statsManagerCache != null && !optionsManagerCache.paused)
        {
            if (levelCompleted != statsManagerCache.infoSent)
            {
                levelCompleted = statsManagerCache.infoSent;
                if (statsManagerCache.infoSent)
                {
                    OnRunCompleted?.Invoke(statsManagerCache.infoSent);
                }
            }
        }

        if (challengeManagerCache != null && !optionsManagerCache.paused)
        {
            if (challengeDone != challengeManagerCache.challengeDone)
            {
                challengeDone = challengeManagerCache.challengeDone;
                OnChallenge?.Invoke(challengeManagerCache.challengeDone);
            }
        }
    }
}