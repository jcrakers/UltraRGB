using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltrakillArtemisMod.Components;

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

    public delegate void OnLevelChangedHandler(LevelType LevelType);

    /// <summary>
    /// Invoked whenever the current level type is changed.
    /// </summary>
    public static OnLevelChangedHandler OnSceneTypeChanged;

    /// <summary>
    /// Invoked whenever the scene is changed.
    /// </summary>
    public static OnLevelChangedHandler OnSceneChanged;


    private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene != SceneManager.GetActiveScene())
            return;

        LevelType newScene = GetLevelType(SceneHelper.CurrentScene);

        if (newScene != CurrentLevelType)
        {
            CurrentLevelType = newScene;
            CurrentSceneName = SceneHelper.CurrentScene;
            OnSceneTypeChanged?.Invoke(newScene);
        }


        OnSceneChanged?.Invoke(CurrentLevelType);
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