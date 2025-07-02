using BepInEx;
using BepInEx.Logging;
using System.Net.Http;
using ULTRAKILL;
using UnityEngine;
using UltrakillArtemisMod.Components;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;


namespace UltrakillArtemisMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ArtemisSupport : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    private static readonly HttpClient client = new HttpClient();
    public static string url = "";

    private void PostArtemis(string endPoint, string content)
    {
        var stringContent = new StringContent(content);
        stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        client.PostAsync(url + endPoint, stringContent);
    }

    private void PostArtemis(string endPoint, object content)
    {
        var stringContent = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json");
        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        client.PostAsync(url + endPoint, stringContent);
    }

    private string GetArtemisPort()
    {
        var path = @"C:\ProgramData\Artemis\webserver.txt";
        var port = "http://localhost:9696/";
        if (File.Exists(path))
        {
            port = File.ReadAllText(path);
        }
        else
        {
            Logger.LogWarning("Artemis webserver port not found. Defaulting to 9696.");
        }

        return $"{port}plugins/A6B0DCB1-2160-4BF0-993F-03B4AC688EB2/";
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        url = GetArtemisPort();

        SceneCheck.Init();
        SceneCheck.OnSceneChanged += OnSceneChanged;
        SceneCheck.OnSceneTypeChanged += OnSceneTypeChanged;

        StatsManager.checkpointRestart += OnCheckpointRestart;

        OtherCheck.Init();
        OtherCheck.OnPause += OnPause;
        OtherCheck.OnDeath += OnDeath;
        OtherCheck.OnCheatsEnabled += OnCheatsEnabled;

        RunCheck.Init();
        RunCheck.OnRunCompleted += OnRunCompleted;
        RunCheck.OnChallenge += OnChallenge;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Logger.LogInfo($"Challenge done: {ChallengeManager.Instance.challengeDone}");
        }

        if (StatsManager.Instance != null)
        {
            if (StatsManager.Instance.levelStarted && !StatsManager.Instance.endlessMode && !OptionsManager.Instance.paused)
            {
                var statsJson = new
                {
                    Time = StatsManager.Instance.seconds,
                    TimeRank = getRank(0),
                    Kills = StatsManager.Instance.kills,
                    KillRank = getRank(1),
                    Style = StatsManager.Instance.stylePoints,
                    StyleRank = getRank(2)
                };
                PostArtemis("RunStats", statsJson);
                //ResponseAsync(content);
            }

            if (StatsManager.Instance.endlessMode && !OptionsManager.Instance.paused)
            {
                var statsJson = new
                {
                    Time = StatsManager.Instance.seconds,
                    Wave = EndlessGrid.Instance.currentWave,
                    EnemysLeft = EnemyTracker.Instance.GetCurrentEnemies().Count
                };
                PostArtemis("CyberGrindStats", statsJson);
                //ResponseAsync(content);
            }
        }

    }

    string getRank(int type)
    {
        var ranks = new List<string> { "D", "C", "B", "A", "S" };
        if (type == 0)
        {
            if (StatsManager.Instance.seconds >= StatsManager.Instance.timeRanks[0])
                return ranks[0];
            else if (StatsManager.Instance.seconds >= StatsManager.Instance.timeRanks[1])
                return ranks[1];
            else if (StatsManager.Instance.seconds >= StatsManager.Instance.timeRanks[2])
                return ranks[2];
            else if (StatsManager.Instance.seconds >= StatsManager.Instance.timeRanks[3])
                return ranks[3];
            else
                return ranks[4];
        }
        if (type == 1)
        {
            if (StatsManager.Instance.kills < StatsManager.Instance.killRanks[0])
                return ranks[0];
            else if (StatsManager.Instance.kills < StatsManager.Instance.killRanks[1])
                return ranks[1];
            else if (StatsManager.Instance.kills < StatsManager.Instance.killRanks[2])
                return ranks[2];
            else if (StatsManager.Instance.kills < StatsManager.Instance.killRanks[3])
                return ranks[3];
            else
                return ranks[4];
        }
        if (type == 2)
        {
            if (StatsManager.Instance.stylePoints < StatsManager.Instance.styleRanks[0])
                return ranks[0];
            else if (StatsManager.Instance.stylePoints < StatsManager.Instance.styleRanks[1])
                return ranks[1];
            else if (StatsManager.Instance.stylePoints < StatsManager.Instance.styleRanks[2])
                return ranks[2];
            else if (StatsManager.Instance.stylePoints < StatsManager.Instance.styleRanks[3])
                return ranks[3];
            else
                return ranks[4];
        }
        return "Unknown";
    }

    /*private async Task ResponseAsync(StringContent content)
    {
        var response = await client.PostAsync(url + "RunStats", content);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            Logger.LogInfo($"Artemis response: {responseData}");
        }
        else
        {
            Logger.LogError($"Error: {response.StatusCode}");
        }
    }*/

    private void OnSceneChanged(SceneCheck.LevelType levelType)
    {
        //Logger.LogInfo($"Level changed to {SceneCheck.CurrentSceneName}");

        PostArtemis("CurrentSceneName", SceneCheck.CurrentSceneName);

        var difficulty = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty");
        var difficultyName = difficulty switch
        {
            0 => "Harmless",
            1 => "Lenient",
            2 => "Standard",
            3 => "Violent",
            4 => "Brutal",
            5 => "UMD",
            _ => "Unknown"
        };

        var json = new
        {
            Difficulty = difficulty,
            DifficultyName = difficultyName
        };
        PostArtemis("Difficulty", json);

        PostArtemis("RunStatsReset", "true");
    }

    private void OnSceneTypeChanged(SceneCheck.LevelType levelType)
    {
        //Logger.LogInfo($"Scene type changed to {levelType}");
        PostArtemis("CurrentSceneType", levelType.ToString());
    }

    private void OnCheckpointRestart()
    {
        //Logger.LogInfo($"Checkpoint restart");
        PostArtemis("Restarts", StatsManager.Instance.restarts.ToString());
    }

    private void OnPause(bool paused)
    {
        //Logger.LogInfo($"Paused: {paused}");
        PostArtemis("Paused", paused.ToString());
    }

    private void OnDeath(bool dead)
    {
        //Logger.LogInfo($"Dead: {dead}");
        PostArtemis("Dead", dead.ToString());
    }

    private void OnCheatsEnabled(bool cheatsEnabled)
    {
        //Logger.LogInfo($"Cheats enabled: {cheatsEnabled}");
        PostArtemis("CheatsEnabled", cheatsEnabled.ToString());
    }

    private void OnRunCompleted(bool completed)
    {
        //Logger.LogInfo($"Run completed");
        PostArtemis("LevelCompleted", "true");
    }

    private void OnChallenge(bool completed)
    {
        //Logger.LogInfo($"Challenge {completed}");
        PostArtemis("ChallengeCompleted", completed.ToString());
    }
}