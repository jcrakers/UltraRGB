using BepInEx;
using BepInEx.Logging;
using System.Net.Http;
using ULTRAKILL;
using UnityEngine;
using UltrakillArtemisMod.Components;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace UltrakillArtemisMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ArtemisSupport : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    private static readonly HttpClient client = new HttpClient();

    public static string url = "";

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

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (StatsManager.Instance.levelStarted && StatsManager.Instance != null)
        {
            var json = new
            {
                Time = StatsManager.Instance.seconds,
                Kills = StatsManager.Instance.kills,
                Style = StatsManager.Instance.stylePoints
            };
            var content = new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //Logger.LogInfo($"Stats: {content.ReadAsStringAsync().Result}");
            client.PostAsync(url + "RunStats", content);
            //ResponseAsync(content);
        }
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

        var content = new StringContent(SceneCheck.CurrentSceneName);
        content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        client.PostAsync(url + "CurrentSceneName", content);

        var json = new
        {
            Seconds = 0,
            Kills = 0,
            Style = 0
        };
        var statsReset = new StringContent(JsonUtility.ToJson(json));
        statsReset.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        client.PostAsync(url + "RunStats", statsReset);
    }

    private void OnSceneTypeChanged(SceneCheck.LevelType levelType)
    {
        //Logger.LogInfo($"Level type changed to {levelType}");

        var content = new StringContent(levelType.ToString());
        content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        client.PostAsync(url + "CurrentSceneType", content);
    }
}