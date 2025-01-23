using Comfort.Common;
using EFT;
using EFT.Console.Core;
using EFT.UI;
using JBTrackIR.Components;
using SPT.Reflection.Patching;
using System.Reflection;

namespace JBTrackIR.Patches;

public class DebugPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
    }

    [PatchPrefix]
    public static void PatchPrefix()
    {
        TrackIRDebugComponent.Enable();
    }
}