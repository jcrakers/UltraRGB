using BepInEx;
using BepInEx.Logging;
using GameConsole.pcon;
using System.Net.Http;
using ULTRAKILL;
using UnityEngine;
using UnityEngine.SceneManagement;
using UltrakillArtemisMod.SceneCheck;

namespace UltrakillArtemisMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ArtemisSupport : BaseUnityPlugin
{
    public static float seconds;
    public static new ManualLogSource Logger;
    //static readonly HttpClient client = new HttpClient();

    private GetArtemisWebServerPort()
    {
        
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;



        SceneCheck.Init();
        SceneCheck.OnLevelChanged += OnLevelChanged;
        SceneCheck.OnLevelTypeChanged += OnLevelTypeChanged;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
    
    private void Update()
    {
        seconds = StatsManager.Instance.seconds;
        if (Input.GetKey(KeyCode.F1))
        {
            Logger.LogInfo($"Seconds: {seconds}");
        }
    }

    private void OnLevelChanged(LevelType levelType)
    {
        Logger.LogInfo($"Level changed to {SceneCheck.CurrentSceneName}");
    }
    
    private void OnLevelTypeChanged(LevelType levelType)
    {
        Logger.LogInfo($"Level type changed to {levelType}");
    }
}