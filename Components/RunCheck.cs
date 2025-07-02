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

    private static bool oldCompleted = false;
    private static bool oldChallenge = false;



    void Update()
    {
        if (StatsManager.Instance != null)
        {
            if (oldCompleted != StatsManager.Instance.infoSent)
            {
                oldCompleted = StatsManager.Instance.infoSent;
                if (StatsManager.Instance.infoSent)
                {
                    OnRunCompleted?.Invoke(StatsManager.Instance.infoSent);
                }
            }
        }
        if (ChallengeManager.Instance != null)
        {
            if (oldChallenge != ChallengeManager.Instance.challengeDone)
            {
                oldChallenge = ChallengeManager.Instance.challengeDone;
                OnChallenge?.Invoke(ChallengeManager.Instance.challengeDone);

            }
        }
    }
}