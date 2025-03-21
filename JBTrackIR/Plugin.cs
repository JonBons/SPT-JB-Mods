﻿using BepInEx;
using System;
using JBTrackIR.Patches;
using JBTrackIR.Utilities;

namespace JBTrackIR;

[BepInPlugin("com.jonbons.trackir", "JonBons.TrackIR", "1.0.8")]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo($"Plugin com.jonbons.trackir is loaded!");

        // Settings must init first
        Settings.Init(Config, Logger);
        TrackIRManager.Instance.Init();

        try
        {
            new DebugPatch().Enable();
            new PlayerLookPatch().Enable();
            new PlayerControlsPatch().Enable();
            new PlayerLeanFixPatch().Enable();
        }
        catch (Exception ex)
        {
            Logger.LogError($"{GetType().Name}: {ex}");
            throw;
        }
    }

    private void OnDestroy()
    {
        if (TrackIRManager.Instance.IsInitialized)
            TrackIRManager.Instance.ClearSingleton();
    }
}
