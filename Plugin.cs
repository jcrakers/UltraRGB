using BepInEx;
using BepInEx.Logging;
using GameConsole.pcon;
using ULTRAKILL;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltrakillArtemisMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ArtemisSupport : BaseUnityPlugin
{
    public static float seconds;
    public static new ManualLogSource Logger;
    //private NewMovement player;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        InGameCheck.Init();

        InGameCheck.OnLevelChanged += OnLevelChanged;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Logger.LogInfo($"Current level type: {InGameCheck.CurrentLevelType}");
    }
    
    private void Update()
    {
        seconds = StatsManager.Instance.seconds;
        if (Input.GetKey(KeyCode.F1))
        {
            Logger.LogInfo($"Seconds: {seconds}");
        }
    }

    

    private static void OnLevelChanged(InGameCheck.LevelType levelType)
    {
        Logger.LogInfo($"Scene changed to {InGameCheck.CurrentSceneName}");
        Logger.LogInfo($"Level changed to {levelType}");
    }
}

public static class InGameCheck
{
    private static bool initialized;
    public static void Init()
    {
        if (!initialized)
        {
            initialized = true;
            SceneManager.sceneLoaded += OnSceneLoad;
        }
    }

    /// <summary>
    /// Enumerated version of the Ultrakill scene types
    /// </summary>
    public enum LevelType { Intro, MainMenu, Tutorial, Level, Credits, Sandbox, CyberGrind, Secret, PrimeSanctum, Encore, Custom, Unknown }
    /// <summary>
    /// Returns the current level type
    /// </summary>
    public static LevelType CurrentLevelType = LevelType.Intro;

    /// <summary>
    /// Returns the currently active ultrakill scene name.
    /// </summary>
    public static string CurrentSceneName = "";

    public delegate void OnLevelChangedHandler(LevelType LevelType);

    /// <summary>
    /// Invoked whenever the current level type is changed.
    /// </summary>
    public static OnLevelChangedHandler OnLevelTypeChanged;

    /// <summary>
    /// Invoked whenever the scene is changed.
    /// </summary>
    public static OnLevelChangedHandler OnLevelChanged;


    private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene != SceneManager.GetActiveScene())
            return;

        LevelType newScene = GetLevelType(SceneHelper.CurrentScene);

        if (newScene != CurrentLevelType)
        {
            CurrentLevelType = newScene;
            CurrentSceneName = SceneHelper.CurrentScene;
            OnLevelTypeChanged?.Invoke(newScene);
        }


        OnLevelChanged?.Invoke(CurrentLevelType);
    }
    
    public static LevelType GetLevelType(string sceneName)
    {
        sceneName = sceneName.Contains("P-") ? "PrimeSanctum" : sceneName;
        sceneName = sceneName.Contains("-S") ? "Secret" : sceneName;
        sceneName = sceneName.Contains("Level") ? "Level" : sceneName;
        sceneName = sceneName.Contains("-E") ? "Encore" : sceneName;

        return sceneName switch
        {
            "Intro" => LevelType.Intro,
            "Main Menu" => LevelType.MainMenu,
            "Tutorial" => LevelType.Tutorial,
            "Level" => LevelType.Level,
            "CreditsMuseum2" => LevelType.Credits,
            "uk_construct" => LevelType.Sandbox,
            "Endless" => LevelType.CyberGrind,
            "Secret" => LevelType.Secret,
            "PrimeSanctum" => LevelType.PrimeSanctum,
            "Custom" => LevelType.Custom,
            _ => LevelType.Unknown,
        };
    }
}