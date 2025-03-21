﻿using EFT.InputSystem;
using SPT.Reflection.Patching;
using System.Reflection;

namespace JBTrackIR.Patches
{
    public class PlayerControlsPatch : ModulePatch
    {
        public static int CommandLean = 0;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Class1602).GetMethod(nameof(Class1602.TranslateCommand), BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static void PatchPrefix(ref Class1602 __instance, ECommand command)
        {
            switch (command)
            {
                case ECommand.ToggleLeanLeft:
                    CommandLean = -1;
                    break;
                case ECommand.ToggleLeanRight:
                    CommandLean = 1;
                    break;
                case ECommand.EndLeanLeft:
                    CommandLean = 0;
                    break;
                case ECommand.EndLeanRight:
                    CommandLean = 0;
                    break;
            }
        }
    }
}
