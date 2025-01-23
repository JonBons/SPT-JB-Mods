using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using System.Reflection;

namespace JBTrackIR.Patches;

public class PlayerLeanFixPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
    }

    [PatchPostfix]
    public static void PatchPostfix()
    {
        // This is awful, please please find the culprit and fix it there
        // BUT this works for now

        var _gameWorld = Singleton<GameWorld>.Instance;
        var _player = _gameWorld?.MainPlayer;

        if (_player != null)
        {
            _player.method_3(-1);
            _player.WaitSeconds(0.1f, () => {
                var _gameWorld = Singleton<GameWorld>.Instance;
                var _player = _gameWorld?.MainPlayer;

                if (_player != null)
                {
                    _player.method_4(-1);
                }
            });
        }
    }
}