using BepInEx;
using BepInEx.Logging;
using System.Net.Http;
using UnityEngine;
using UltraRGB.Components;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections;

namespace UltraRGB;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class UltraRGB : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    private static readonly HttpClient client = new();
    public static string url = "";

    public static readonly float UpdateRate = 30f;
    public static readonly bool EnableRateLimiting = true;

    private async void PostArtemis(string endPoint, object content)
    {
        try
        {
            //Logger.LogInfo("Preparing to post to Artemis...");

            //Logger.LogInfo($"Raw content type: {content?.GetType().FullName ?? "null"}");
        
            if (content is Dictionary<string, object> dict)
            {
                content = CleanDictionary(dict);
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(content, settings);
            Logger.LogInfo($"Serialized content:\n{json}");


            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "text/plain");
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");


            //Logger.LogInfo($"Sending request to {url + endPoint}");
            var response = await client.PostAsync(url + endPoint, stringContent);


            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Logger.LogWarning($"Failed to post to {endPoint}. Status: {response.StatusCode}, Error: {errorContent}");
            }
            else
            {
                //Logger.LogInfo("Successfully posted to Artemis");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"Unexpected error in PostArtemis: {ex}");
        }
    }

    private Dictionary<string, object> CleanDictionary(Dictionary<string, object> dict)
    {
        var result = new Dictionary<string, object>();
    
        foreach (var kvp in dict)
        {
            if (kvp.Value is Dictionary<string, object> nestedDict)
            {
                var cleaned = CleanDictionary(nestedDict);
                if (cleaned.Count > 0)
                {
                    result[kvp.Key] = cleaned;
                }
            }
            else if (kvp.Value != null)
            {
                result[kvp.Key] = kvp.Value;
            }
        }
    
        return result;
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

    public static GameObject ultraRGBObject;

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
        pendingUpdates = DeepCopy(JsonStructure);

        ultraRGBObject.AddComponent<PlayerCheck>();
        //ultraRGBObject.AddComponent<WeaponCheck>();
        ultraRGBObject.AddComponent<RunCheck>();
        SceneCheck.Init();
        ultraRGBObject.AddComponent<MiscellaneousCheck>();

        ultraRGBObject.AddComponent<UltraRGBUpdater>().Initialize(this);

        StatsManager.checkpointRestart += OnCheckpointRestart;
    }

    private static Dictionary<string, object> pendingUpdates;
    private static bool hasUpdates = false;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Logger.LogInfo(NewMovement.Instance.currentWallJumps);
        }

        if (hasUpdates)
        {
            //Logger.LogInfo($"Sending batched updates");
            PostArtemis("ULTRAKILLBundle", pendingUpdates);
            pendingUpdates = DeepCopy(JsonStructure);
            hasUpdates = false;
        }
    }

    private static readonly List<string> WeaponComponents = ["Revolver", "AlternateRevolver", "ShotGun", "Jackhammer", "NailGun", "SawBladeLauncher", "RocketLauncher"];
    private static readonly Dictionary<string, object> JsonStructure = new()
    {
        ["MiscellaneousData"] = new Dictionary<string, object> {},
        ["RunData"] = new Dictionary<string, object> {},
        ["CyberGrindData"] = new Dictionary<string, object> {},
        ["PlayerData"] = new Dictionary<string, object> {},
        ["WeaponData"] = new Dictionary<string, object>
        {
            ["Revolver"] = new Dictionary<string, object> {},
            ["AlternateRevolver"] = new Dictionary<string, object> {},
            ["ShotGun"] = new Dictionary<string, object> {},
            ["Jackhammer"] = new Dictionary<string, object> {},
            ["NailGun"] = new Dictionary<string, object> {},
            ["SawBladeLauncher"] = new Dictionary<string, object> {},
            ["RocketLauncher"] = new Dictionary<string, object> {},
        },
    };

    private static Dictionary<string, object> DeepCopy(Dictionary<string, object> original)
    {
        var copy = new Dictionary<string, object>();
        foreach (var entry in original)
        {
            if (entry.Value is Dictionary<string, object>)
            {
                copy[entry.Key] = DeepCopy(entry.Value as Dictionary<string, object>);
            }
            else
            {
                copy[entry.Key] = entry.Value;
            }
        }
        return copy;
    }

    public static void QueueUpdate(string key, object value, string component)
    {
        try
        {
            if (WeaponComponents.Contains(component))
            {
                //Logger.LogInfo($"QueueUpdate {key} {value} in {component} in Weapon");

                var weaponDict = pendingUpdates["WeaponData"] as Dictionary<string, object>;
                var componentDict = weaponDict[component] as Dictionary<string, object>;
                componentDict[key] = value;


            }
            else
            {
                //Logger.LogInfo($"QueueUpdate {key} {value} in {component}");

                var componentDict = pendingUpdates[component + "Data"] as Dictionary<string, object>;
                componentDict[key] = value;


            }
            hasUpdates = true;


        }
        catch (Exception e)
        {
            Logger.LogWarning($"Failed to queue update when updating {key} in {component + "Data"}: {e.Message}");
        }
    }

    private void OnCheckpointRestart()
    {
        QueueUpdate("Restarts", StatsManager.Instance.restarts, "Run");
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

    private class UltraRGBUpdater : MonoBehaviour
    {
        private UltraRGB parent;

        public void Initialize(UltraRGB parent)
        {
            this.parent = parent;
        }

        private void LateUpdate()
        {
            parent.LateUpdate();
        }
    }
}