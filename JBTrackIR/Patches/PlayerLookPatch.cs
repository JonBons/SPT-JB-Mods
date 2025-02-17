using EFT;
using JBTrackIR.Helpers;
using JBTrackIR.Utilities;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace JBTrackIR.Patches;

public class PlayerLookPatch : ModulePatch
{
    private static bool wasCommandedLean = false;
    private static bool storeCommandedLean = false;
    private static float commandedTilt = 0f;
    private static float lastTilt = 0f;

    const float lerpSpeed = 2.5f; // Adjust this for faster/slower interpolation
    const float threshold = 4.0f; // Threshold to stop interpolation

    private static Vector3 startPosition = Vector3.zero;

    protected override MethodBase GetTargetMethod()
    {
        return typeof(Player).GetMethod("Look", BindingFlags.Instance | BindingFlags.Public);
    }

    [PatchPostfix]
    private static void PatchPostfix(ref Player __instance)
    {
        if (!TrackIRManager.Instance.Running) return;
        if (!Settings.Enabled.Value) return;

        var data = new TrackIRData(TrackIRManager.Instance.Client);
        UpdateHeadRotation(ref __instance, ref data);
        UpdateHeadPosition(ref __instance, ref data);

        //Logger.LogInfo(string.Format("TIR DATA Final pos = {0}; Final rot = {1}", pos, rot));
    }

    private static void UpdateLeaning(ref Player __instance, ref TrackIRData data)
    {
        float tiltValue = data.TargetTilt;

        // Check if the lean was commanded via keyboard
        if (PlayerControlsPatch.CommandLean != 0) wasCommandedLean = true;

        // Store tilt value when releasing key
        if (PlayerControlsPatch.CommandLean == 0 && wasCommandedLean && !storeCommandedLean) storeCommandedLean = true;
        if (storeCommandedLean)
        {
            commandedTilt = __instance.MovementContext.SmoothedTilt;
            storeCommandedLean = false;
        }

        // Lean key released and we're lerping
        if (PlayerControlsPatch.CommandLean == 0 && wasCommandedLean)
        {
            // Lerp from commandedTilt to targetTilt
            __instance.MovementContext.SmoothedTilt = Mathf.Lerp(commandedTilt, tiltValue, lerpSpeed * Time.deltaTime);

            // If close enough to the target, release control back to the axis
            if (Mathf.Abs(__instance.MovementContext.SmoothedTilt - tiltValue) < threshold)
            {
                wasCommandedLean = false;
            }
        }

        // Leaning via axis (only if no leaning key is pressed and interpolation is done)
        if (PlayerControlsPatch.CommandLean == 0 && !wasCommandedLean && (tiltValue != lastTilt))
        {
            __instance.MovementContext.SmoothedTilt = tiltValue;
        }

        lastTilt = tiltValue;
    }

    private static void UpdateHeadPosition(ref Player __instance, ref TrackIRData data)
    {
        UpdateLeaning(ref __instance, ref data);
    }

    private static void UpdateHeadRotation(ref Player __instance, ref TrackIRData data)
    {
        var localEulers = __instance.ProceduralWeaponAnimation.HandsContainer.CameraTransform.localRotation.eulerAngles;

        localEulers.y = data.Yaw;
        localEulers.x = data.Pitch;
        localEulers.z = data.Roll;

        __instance.HeadRotation = localEulers; // Sync head angles
        __instance.ProceduralWeaponAnimation.SetHeadRotation(localEulers); // Update camera
    }
}