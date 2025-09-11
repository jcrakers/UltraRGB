using UnityEngine.SceneManagement;

namespace UltraRGB.Components;

public static class SceneCheck
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

    //Code frome EasyPZ which is made by Hydraxous
    //https://github.com/Hydraxous/EasyPZ-ULTRAKILL
    //Modified by me

    /// <summary>
    /// Enumerated version of the Ultrakill scene types
    /// </summary>
    public enum LevelType { Bootstrap, Intro, MainMenu, Tutorial, Level, Credits, Sandbox, CyberGrind, Secret, PrimeSanctum, Encore, Custom, Unknown }
    /// <summary>
    /// Returns the current level type
    /// </summary>
    public static LevelType CurrentLevelType = LevelType.Intro;

    /// <summary>
    /// Returns the currently active ultrakill scene name.
    /// </summary>
    public static string CurrentSceneName = "";

    private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        UltraRGB.Logger.LogInfo($"OnSceneLoad: {scene.name}");
        if (scene != SceneManager.GetActiveScene())
            return;

        LevelType newScene = GetLevelType(SceneHelper.CurrentScene);

        if (newScene != CurrentLevelType)
        {
            CurrentLevelType = newScene;
            CurrentSceneName = SceneHelper.CurrentScene;
            UltraRGB.QueueUpdate("CurrentSceneType", CurrentLevelType.ToString(), "Miscellaneous");
        }

        UltraRGB.QueueUpdate("CurrentSceneName", CurrentSceneName, "Miscellaneous");

        var difficulty = PrefsManager.Instance.GetInt("difficulty");
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

        UltraRGB.QueueUpdate("Difficulty", difficulty, "Miscellaneous");
        UltraRGB.QueueUpdate("DifficultyName", difficultyName, "Miscellaneous");
    }
    
    public static LevelType GetLevelType(string sceneName)
    {
        sceneName = sceneName.Contains("P-") ? "PrimeSanctum" : sceneName;
        sceneName = sceneName.Contains("-S") ? "Secret" : sceneName;
        sceneName = sceneName.Contains("Level") ? "Level" : sceneName;
        sceneName = sceneName.Contains("-E") ? "Encore" : sceneName;

        return sceneName switch
        {
            "Bootstrap" => LevelType.Bootstrap,
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