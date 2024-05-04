using Aki.Reflection.Patching;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using EFT.Animations;
using EFT.InventoryLogic;
using HarmonyLib;
using RootMotion;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using TrackIRUnity;
using UnityEngine;

namespace JBTrackIR;

[BepInPlugin("com.jonbons.trackir", "JonBons.TrackIR", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    static ConfigEntry<bool> tirEnabled;
    static ConfigEntry<float> tirSensitivityCoef;
    static ConfigEntry<int> tirLimitPitchLower;
    static ConfigEntry<int> tirLimitPitchUpper;
    static ConfigEntry<int> tirLimitYawLower;
    static ConfigEntry<int> tirLimitYawUpper;
    static TrackIRClient tirClient;
    static bool tirRunning = false;

    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin com.jonbons.trackir is loaded!");

        tirClient = new TrackIRClient();
        if (tirClient != null && !tirRunning)
        {
            tirClient.TrackIR_Enhanced_Init();
            tirRunning = true;
            Logger.LogInfo($"com.jonbons.trackir: trackir is running");
        }

        BindSettings();

        new Transpiler().Enable();
    }

    private void OnDestroy()
    {
        if (tirClient != null && tirRunning)
        {
            tirClient.TrackIR_Shutdown();
            tirRunning = false;
        }
    }

    private void BindSettings()
    {
        tirEnabled = Config.Bind(
            "Main Settings",
            "TrackIR Enabled",
            true,
            new ConfigDescription("Enable TrackIR support")
        );

        tirSensitivityCoef = Config.Bind(
            "Main Settings",
            "TrackIR Sensitivity coef",
            0.5f,
            new ConfigDescription("Senstivity coefficient to apply to all TrackIR inputs",
            new AcceptableValueRange<float>(0, 1))
        );

        tirLimitPitchLower = Config.Bind(
            "Main Settings",
            "TrackIR Pitch lower limit",
            -85,
            new ConfigDescription("Lower limit of TrackIR pitch angles",
            new AcceptableValueRange<int>(-180, 180))
        );

        tirLimitPitchUpper = Config.Bind(
            "Main Settings",
            "TrackIR Pitch upper limit",
            85,
            new ConfigDescription("Upper limit of TrackIR pitch angles",
            new AcceptableValueRange<int>(-180, 180))
        );

        tirLimitYawLower = Config.Bind(
            "Main Settings",
            "TrackIR Yaw lower limit",
            -150,
            new ConfigDescription("Lower limit of TrackIR yaw angles",
            new AcceptableValueRange<int>(-180, 180))
        );

        tirLimitYawUpper = Config.Bind(
            "Main Settings",
            "TrackIR Yaw upper limit",
            150,
            new ConfigDescription("Upper limit of TrackIR yaw angles",
            new AcceptableValueRange<int>(-180, 180))
        );
    }

    [Serializable]
    public class Limit
    {
        public Limit()
        {
            lower = -180;
            upper = 180;
        }
        public Limit(float low, float up)
        {
            lower = low;
            upper = up;
        }
        public float lower, upper;
    }

    public class Transpiler : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Look", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(ref Player __instance)
        {
            if (!tirRunning) return;
            if (!tirEnabled.Value) return;

            float positionReductionFactor = 0.045f * tirSensitivityCoef.Value;
            float rotationReductionFactor = 0.045f * tirSensitivityCoef.Value;
            Limit positionXLimits = new Limit();
            Limit positionYLimits = new Limit();
            Limit positionZLimits = new Limit();
            Limit pitchLimits = new Limit(tirLimitPitchLower.Value, tirLimitPitchUpper.Value);
            Limit yawLimits = new Limit(tirLimitYawLower.Value, tirLimitYawUpper.Value);
            Limit rollLimits = new Limit(-100, 100);
            bool useLimits = true;

            //Logger.LogInfo(string.Format("TIR DATA Start"));

            TrackIRClient tirClient = Plugin.tirClient;

            TrackIRClient.LPTRACKIRDATA tid = tirClient.client_HandleTrackIRData(); // Data for head tracking

            //Logger.LogInfo(string.Format("TIR DATA Pitch = {0}; Yaw = {1}, Roll = {2}", tid.fNPPitch, tid.fNPYaw, tid.fNPRoll));

            Vector3 rot = __instance.ProceduralWeaponAnimation.HandsContainer.CameraTransform.localRotation.eulerAngles;
            rot.z = 0; // we don't need to use the existing Z value
            if (!useLimits)
            {
                rot.y = tid.fNPYaw * rotationReductionFactor;
                rot.x = tid.fNPPitch * rotationReductionFactor;
                //rot.z = -tid.fNPRoll * rotationReductionFactor;
            }
            else
            {
                rot.y = Mathf.Clamp(tid.fNPYaw * rotationReductionFactor, yawLimits.lower, yawLimits.upper);
                rot.x = Mathf.Clamp(tid.fNPPitch * rotationReductionFactor, pitchLimits.lower, pitchLimits.upper);
                //rot.z = Mathf.Clamp(-tid.fNPRoll * rotationReductionFactor, rollLimits.lower, rollLimits.upper);
            }

            __instance.ProceduralWeaponAnimation.SetHeadRotation(rot);
            //Logger.LogInfo(string.Format("TIR DATA Final pos = {0}; Final rot = {1}", pos, rot));
        }
    }
}
