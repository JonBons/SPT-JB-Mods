using JBTrackIR.Utilities;
using TrackIRUnity;
using UnityEngine;

namespace JBTrackIR.Helpers
{
    internal class TrackIRData
    {
        public float RawPitch { get; private set; }
        public float RawYaw { get; private set; }
        public float RawRoll { get; private set; }
        public float RawX { get; private set; }
        public float RawY { get; private set; }
        public float RawZ { get; private set; }

        public float Pitch { get; private set; }
        public float Yaw { get; private set; }
        public float Roll { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public float TargetTilt { get; private set; }

        public TrackIRData(TrackIRClient client)
        {
            UpdateValues(client);
        }

        private void UpdateValues(TrackIRClient client)
        {
            if (client == null) return;
            if (!TrackIRManager.Instance.IsInitialized) return;

            var tirData = client.client_HandleTrackIRData();

            RawPitch = tirData.fNPPitch;
            RawYaw = tirData.fNPYaw;
            RawRoll = tirData.fNPRoll;
            RawX = tirData.fNPX;
            RawY = tirData.fNPY;
            RawZ = tirData.fNPZ;

            var positionMultiplier = 0.000031f;
            var rotationMultiplier = 0.011f;

            // TODO: add XYZ limits??
            X = Mathf.Round((-RawX * positionMultiplier * Settings.SensitivityCoef.Value) * 1000) / 1000;
            Y = Mathf.Round((RawY * positionMultiplier * Settings.SensitivityCoef.Value) * 1000) / 1000;
            Z = Mathf.Round((-RawZ * positionMultiplier * Settings.SensitivityCoef.Value) * 1000) / 1000;

            Yaw = Mathf.Clamp(RawYaw * rotationMultiplier * Settings.SensitivityCoef.Value,
                TrackIRManager.Instance.YawLimits.lower, TrackIRManager.Instance.YawLimits.upper);
            Yaw = Mathf.Round(Yaw * 1000) / 1000;

            Pitch = Mathf.Clamp(RawPitch * rotationMultiplier * Settings.SensitivityCoef.Value,
                TrackIRManager.Instance.PitchLimits.lower, TrackIRManager.Instance.PitchLimits.upper);
            Pitch = Mathf.Round(Pitch * 1000) / 1000;

            Roll = Mathf.Clamp(-RawRoll * rotationMultiplier * Settings.SensitivityCoef.Value,
                TrackIRManager.Instance.RollLimits.lower, TrackIRManager.Instance.RollLimits.upper);
            Roll = Mathf.Round(Roll * 1000) / 1000;

            var targetTilt = (X * 20) * Settings.LeanSensitivityCoef.Value;
            TargetTilt = Mathf.Round(targetTilt * 10) / 10;


        }
    }
}
