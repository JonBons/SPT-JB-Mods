using BepInEx.Logging;
using Comfort.Common;
using EFT;
using JBTrackIR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JBTrackIR.Components
{
    internal class TrackIRDebugComponent : MonoBehaviour, IDisposable
    {
        protected ManualLogSource Logger;
        private GUIContent guiContent;
        private GUIStyle guiStyle;

        private static List<UnityEngine.Object> gameObjects = new List<UnityEngine.Object>();

        private TrackIRDebugComponent()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(GetType().Name);
        }

        public void Awake()
        {
            Logger.LogInfo("TrackIRDebugComponent::Awake");
        }

        public void Dispose()
        {
            Logger.LogInfo("TrackIRDebugComponent::Dispose");
            gameObjects.ForEach(Destroy);
            gameObjects.Clear();
        }

        public void OnGUI()
        {
            // Setup GUI objects if they're not setup yet
            if (guiStyle == null)
            {
                guiStyle = new GUIStyle(GUI.skin.box);
                guiStyle.alignment = TextAnchor.MiddleRight;
                guiStyle.fontSize = 36;
                guiStyle.margin = new RectOffset(3, 3, 3, 3);
            }

            if (guiContent == null)
            {
                guiContent = new GUIContent();
            }

            var tirData = new Helpers.TrackIRData(TrackIRManager.Instance.Client);

            // Build the data to show in the GUI
            string guiText = "TrackIR Debug\n";
            guiText += "-----------------------\n";
            guiText += $"Pitch: {tirData.Pitch}\n";
            guiText += $"Yaw: {tirData.Yaw}\n";
            guiText += $"Roll: {tirData.Roll}\n";
            guiText += $"X: {tirData.X}\n";
            guiText += $"Y: {tirData.Y}\n";
            guiText += $"Z: {tirData.Z}\n";
            guiText += $"Tilt: {tirData.TargetTilt}\n";

            // Draw the GUI
            guiContent.text = guiText;
            Vector2 guiSize = guiStyle.CalcSize(guiContent);
            UnityEngine.Rect guiRect = new(
                Screen.width - guiSize.x - 5f,
                Screen.height - guiSize.y - 50f,
                guiSize.x,
                guiSize.y);

            GUI.Box(guiRect, guiContent, guiStyle);
        }

        public static void Enable()
        {
            if (Singleton<IBotGame>.Instantiated && Settings.DebugEnabled.Value)
            {
                var gameWorld = Singleton<GameWorld>.Instance;
                gameObjects.Add(gameWorld.GetOrAddComponent<TrackIRDebugComponent>());
            }
        }

        public static void Disable()
        {
            if (Singleton<IBotGame>.Instantiated)
            {
                var gameWorld = Singleton<GameWorld>.Instance;
                gameWorld.GetComponent<TrackIRDebugComponent>()?.Dispose();
            }
        }
    }
}
