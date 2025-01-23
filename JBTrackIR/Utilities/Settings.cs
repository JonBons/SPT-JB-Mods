using BepInEx.Configuration;
using BepInEx.Logging;
using JBTrackIR.Components;
using System;

namespace JBTrackIR.Utilities;

internal class Settings
{
    public static ManualLogSource Logger;

    private const string MainSectionTitle = "Main Settings";
    private const string DebugSectionTitle = "Debug";

    public static ConfigEntry<bool> RefreshDevices;
    public static ConfigEntry<bool> Enabled;
    public static ConfigEntry<float> SensitivityCoef;
    public static ConfigEntry<float> LeanSensitivityCoef;
    public static ConfigEntry<int> PitchLowerLimit;
    public static ConfigEntry<int> PitchUpperLimit;
    public static ConfigEntry<int> YawLowerLimit;
    public static ConfigEntry<int> YawUpperLimit;
    public static ConfigEntry<int> RollLowerLimit;
    public static ConfigEntry<int> RollUpperLimit;

    public static ConfigEntry<bool> DebugEnabled;

    public static void Init(ConfigFile config, ManualLogSource logger)
    {
        Logger = logger;

        RefreshDevices = config.Bind(
            MainSectionTitle,
            "Refresh TrackIR",
            true,
            new ConfigDescription("Refresh TrackIR devices")
        );
        RefreshDevices.SettingChanged += RefreshDevices_SettingChanged;

        Enabled = config.Bind(
            MainSectionTitle,
            "TrackIR Enabled",
            true,
            new ConfigDescription("Enable TrackIR support")
        );

        SensitivityCoef = config.Bind(
            MainSectionTitle,
            "TrackIR Sensitivity coef",
            0.5f,
            new ConfigDescription(
                "Senstivity coefficient to apply to all TrackIR inputs",
                new AcceptableValueRange<float>(0, 1)
            )
        );

        LeanSensitivityCoef = config.Bind(
            MainSectionTitle,
            "TrackIR Lean Sensitivity coef",
            1f,
            new ConfigDescription(
                "Lean Senstivity coefficient for leaning",
                new AcceptableValueRange<float>(0, 2)
            )
        );

        PitchLowerLimit = config.Bind(
            MainSectionTitle,
            "TrackIR Pitch lower limit",
            -85,
            new ConfigDescription(
                "Lower limit of TrackIR pitch angles",
                new AcceptableValueRange<int>(-180, 180)
            )
        );
        PitchLowerLimit.SettingChanged += Limit_SettingChanged;

        PitchUpperLimit = config.Bind(
            MainSectionTitle,
            "TrackIR Pitch upper limit",
            85,
            new ConfigDescription(
                "Upper limit of TrackIR pitch angles",
                new AcceptableValueRange<int>(-180, 180)
            )
        );
        PitchUpperLimit.SettingChanged += Limit_SettingChanged;

        YawLowerLimit = config.Bind(
            MainSectionTitle,
            "TrackIR Yaw lower limit",
            -150,
            new ConfigDescription(
                "Lower limit of TrackIR yaw angles",
                new AcceptableValueRange<int>(-180, 180)
            )
        );
        YawLowerLimit.SettingChanged += Limit_SettingChanged;

        YawUpperLimit = config.Bind(
            MainSectionTitle,
            "TrackIR Yaw upper limit",
            150,
            new ConfigDescription(
                "Upper limit of TrackIR yaw angles",
                new AcceptableValueRange<int>(-180, 180)
            )
        );
        YawUpperLimit.SettingChanged += Limit_SettingChanged;

        RollLowerLimit = config.Bind(
            MainSectionTitle,
            "TrackIR Roll lower limit",
            0,
            new ConfigDescription(
                "Lower limit of TrackIR roll angles",
                new AcceptableValueRange<int>(-85, 85)
            )
        );
        RollLowerLimit.SettingChanged += Limit_SettingChanged;

        RollUpperLimit = config.Bind(
            MainSectionTitle,
            "TrackIR Roll upper limit",
            0,
            new ConfigDescription(
                "Upper limit of TrackIR roll angles",
                new AcceptableValueRange<int>(-85, 85)
            )
        );
        RollUpperLimit.SettingChanged += Limit_SettingChanged;

        DebugEnabled = config.Bind(
            DebugSectionTitle,
            "TrackIR Debug Enabled",
            false,
            new ConfigDescription("Enable TrackIR debugging info")
        );
        DebugEnabled.SettingChanged += DebugEnabled_SettingChanged;
    }

    private static void RefreshDevices_SettingChanged(object sender, EventArgs e)
    {
        if (RefreshDevices.Value)
        {
            if (!TrackIRManager.Instance.IsInitialized) return;
            TrackIRManager.Instance.RefreshDevices();
        }
    }

    private static void Limit_SettingChanged(object sender, EventArgs e)
    {
        if (!TrackIRManager.Instance.IsInitialized) return;
        TrackIRManager.Instance.RefreshSettings();
    }

    private static void DebugEnabled_SettingChanged(object sender, EventArgs e)
    {
        // If no game, do nothing
        if (!Comfort.Common.Singleton<IBotGame>.Instantiated)
        {
            return;
        }

        if (DebugEnabled.Value)
        {
            TrackIRDebugComponent.Enable();
        }
        else
        {
            TrackIRDebugComponent.Disable();
        }
    }
}
