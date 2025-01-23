using BepInEx.Logging;
using JBTrackIR.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TrackIRUnity;

namespace JBTrackIR.Utilities;

internal class TrackIRManager : Singleton<TrackIRManager>
{
    public TrackIRClient Client;

    public Limit PitchLimits;
    public Limit YawLimits;
    public Limit RollLimits;

    public bool Running { get; private set; }

    protected override void OnInitializing()
    {
        base.OnInitializing();
        Client = new TrackIRClient();
        PitchLimits = new Limit(Settings.PitchLowerLimit.Value, Settings.PitchUpperLimit.Value);
        YawLimits = new Limit(Settings.YawLowerLimit.Value, Settings.YawUpperLimit.Value);
        RollLimits = new Limit(Settings.RollLowerLimit.Value, Settings.RollUpperLimit.Value);
    }

    public override void ClearSingleton()
    {
        base.ClearSingleton();
        if (Client != null && Running)
        {
            Client.TrackIR_Shutdown();
            Running = false;
        }
    }

    public void Init()
    {
        if (!IsInitialized) return;

        if (Client != null && !Running)
        {
            var result = Client.TrackIR_Enhanced_Init();
            Settings.Logger.LogInfo($"com.jonbons.trackir: detecting... {result}");

            if (!string.IsNullOrEmpty(result))
            {
                Running = true;
                Settings.Logger.LogInfo($"com.jonbons.trackir: detected");
            }
            else
            {
                Settings.Logger.LogInfo($"com.jonbons.trackir: not detected");
            }
        }
    }

    public void RefreshSettings()
    {
        PitchLimits.lower = Settings.PitchLowerLimit.Value;
        PitchLimits.upper = Settings.PitchUpperLimit.Value;
        YawLimits.lower = Settings.YawLowerLimit.Value;
        YawLimits.upper = Settings.YawUpperLimit.Value;
        RollLimits.lower = Settings.RollLowerLimit.Value;
        RollLimits.upper = Settings.RollUpperLimit.Value;
    }

    public void RefreshDevices()
    {
        if (!IsInitialized) return;

        Settings.RefreshDevices.BoxedValue = false;
        Settings.RefreshDevices.Value = false;

        if (Client != null && Running) Client.TrackIR_Shutdown();
        Running = false;

        var result = Client.TrackIR_Enhanced_Init();
        if (!string.IsNullOrEmpty(result))
        {
            Running = true;
            Settings.Logger.LogInfo($"com.jonbons.trackir: detected");
        }
        else
        {
            Settings.Logger.LogInfo($"com.jonbons.trackir: not detected");
        }
    }
}
