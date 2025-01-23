using EFT;
using JBTrackIR.Helpers;
using JBTrackIR.Utilities;
using SPT.Reflection.Patching;
using System.Reflection;
using TrackIRUnity;
using UnityEngine;

namespace JBTrackIR.Patches;

public class PlayerLookPatch : ModulePatch
{
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
        UpdateHeadPosition(__instance, data);
        UpdateHeadRotation(__instance, data);

        //Logger.LogInfo(string.Format("TIR DATA Final pos = {0}; Final rot = {1}", pos, rot));
    }

    private static void UpdateHeadPosition(Player __instance, TrackIRData data)
    {
        var debugMode = Settings.DebugValMode.Value;
        var debugMode2 = Settings.DebugValMode2.Value;

        // TODO: figure out how to let Q and E lean keys still work

        // TODO: do we even want to handle forward/side/upward translation? seems jank and not worth the effort
        //      would be neat to unlock the lean axis as well to bind to pedals
        if (debugMode2 == 1)
        {
            // default to disabled
        } 
        else if (debugMode2 == 2)
        {
            var pos = __instance.ProceduralWeaponAnimation.HandsContainer.CameraTransform;
            __instance.ProceduralWeaponAnimation.HandsContainer.CameraTransform.Translate(data.X, data.Y, data.Z, pos);
        }
        else if (debugMode2 == 3)
        {

        }
        //__instance.ProceduralWeaponAnimation.HandsContainer.CameraTransform.Translate(-smoothPosX, smoothPosY, -smoothPosZ, pos);

        // handle tilt
        if (debugMode == 1)
        {
            __instance.MovementContext.SmoothedTilt = data.X; // arma like ctrl+a/d movement of arm (no angle tilt)
        }
        else if (debugMode == 2)
        {
            __instance.MovementContext.SetTilt(data.X, true);
        }
        else if (debugMode == 3) 
        {
            __instance.CurrentManagedState.SetTilt(data.X);
        }
    }

    private static void UpdateHeadRotation(Player __instance, TrackIRData data)
    {
        Vector3 rot = __instance.ProceduralWeaponAnimation.HandsContainer.CameraTransform.localRotation.eulerAngles;
        rot.y = data.Yaw;
        rot.x = data.Pitch;
        rot.z = data.Roll;

        __instance.HeadRotation = rot; // sync head angles
        __instance.ProceduralWeaponAnimation.SetHeadRotation(rot); // update camera
    }
}