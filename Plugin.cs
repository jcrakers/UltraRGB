using BepInEx;
using BepInEx.Logging;
using System.Net.Http;
using UnityEngine;
using UltraRGB.Components;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UltraRGB;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class UltraRGB : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    private static readonly HttpClient client = new();
    public static string url = "";

    public static readonly float UpdateRate = 30f;
    public static readonly bool EnableRateLimiting = true;

    private static Dictionary<string, object> pendingUpdates = [];
    private readonly float batchInterval = 0.0333f;
    private float lastBatchTime = 0f;

    public static GameObject ultraRGBObject;

    private async void PostArtemis(string endPoint, object content)
    {
        Logger.LogInfo($"Posting {content}");
        var stringContent = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json");
        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(url + endPoint, stringContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Logger.LogWarning($"Failed to post {content}: {errorContent}");
        }
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

        ultraRGBObject = new GameObject("UltraRGBObject")
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        DontDestroyOnLoad(ultraRGBObject);

        url = GetArtemisPort();

        ultraRGBObject.AddComponent<PlayerCheck>();
        ultraRGBObject.AddComponent<WeaponCheck>();
        ultraRGBObject.AddComponent<RunCheck>();
        ultraRGBObject.AddComponent<OtherCheck>();

        StatsManager.checkpointRestart += OnCheckpointRestart;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} initialized");
    }

    private void Update()
    {
        Logger.LogInfo($"Pending updates: ");
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Logger.LogInfo($"BatchUpdate");
        }

        if (pendingUpdates.Count > 0 && Time.time - lastBatchTime >= batchInterval)
        {
            Logger.LogInfo($"Sending batched updates");
            PostArtemis("UltrakillBundle", pendingUpdates);
            pendingUpdates.Clear();
            lastBatchTime = Time.time;
        }
    }

    public static void QueueUpdate(string key, object value)
    {
        pendingUpdates[key] = value;
    }

    private void OnCheckpointRestart()
    {
        QueueUpdate("RunCheckRestarts", StatsManager.Instance.restarts);
    }

    /*private void OnStyleMeterChanged(float styleMeter)
    {
        //Logger.LogInfo($"Style meter changed to {styleMeter}");
        styleMeter = Mathf.Clamp(styleMeter, 0, StyleHUD.Instance.currentRank.maxMeter);
        styleMeter /= StyleHUD.Instance.currentRank.maxMeter;
        //styleMeter /= 0.75f;
        styleMeter *= 100f;

        PostArtemis("StyleMeter", styleMeter.ToString());
    }*/


    /*private readonly List<string> styleMeterRankNameList = ["None", "Destructive", "Chaotic", "Brutal", "Anarchic", "Supreme", "SSadistic", "SSShitstorm", "ULTRAKILL"];
    private void OnStyleMeterRankChanged(int styleMeterRank)
    {

        if (PlayerCheck.styleMeter > 0f)
        {
            styleMeterRank += 1;
        }

        var json = new
        {
            StyleRank = styleMeterRank,
            StyleRankName = styleMeterRankNameList[styleMeterRank]
        };

        //Logger.LogInfo($"Style meter rank changed to {styleMeterRank}");
        PostArtemis("StyleRank", json);
    }*/

    /*private void OnStyleMeterMultiplierChanged(float styleMeterMultiplier)
    {
        //Logger.LogInfo($"StyleMeterMultiplier changed to {styleMeterMultiplier}");
        PostArtemis("StyleMultiplier", Math.Round(styleMeterMultiplier, 2).ToString());
    }*/

    /*private void OnFistCooldownChange(float fistCooldown)
    {
        //Logger.LogInfo($"Fist cooldown changed to {fistCooldown}");
        fistCooldown *= 50f;
        PostArtemis("FistCooldown", fistCooldown.ToString());
    }*/
    
    /*private void OnWeaponFreshnessMeterChange(float weaponFreshness)
    {
        //Logger.LogInfo($"Style meter rank changed to {styleMeterRank}");
        weaponFreshness = Mathf.InverseLerp(0.5f, 10f, weaponFreshness);
        weaponFreshness *= 150f;
        PostArtemis("WeaponFreshnessMeter", weaponFreshness.ToString());
    }*/
}