using JBTrackIR.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using TrackIRUnity;
using UnityEngine;

namespace JBTrackIR.Helpers
{
    internal class TrackIRData
    {
        public float Pitch { get; private set; }
        public float Yaw { get; private set; }
        public float Roll { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public TrackIRData(TrackIRClient client)
        {
            UpdateValues(client);
        }

        private void UpdateValues(TrackIRClient client)
        {
            if (client == null) return;
            if (!TrackIRManager.Instance.IsInitialized) return;

            var tirData = client.client_HandleTrackIRData();

            var positionReductionFactor = 0.015f * Settings.SensitivityCoef.Value;
            var rotationReductionFactor = 0.045f * Settings.SensitivityCoef.Value;
            var rollReductionFactor = (0.045f * Settings.SensitivityCoef.Value) * 0.75f;

            Yaw = Mathf.Clamp(tirData.fNPYaw * rotationReductionFactor,
                TrackIRManager.Instance.YawLimits.lower, TrackIRManager.Instance.YawLimits.upper);
            Pitch = Mathf.Clamp(tirData.fNPPitch * rotationReductionFactor,
                TrackIRManager.Instance.PitchLimits.lower, TrackIRManager.Instance.PitchLimits.upper);
            Roll = Mathf.Clamp(-tirData.fNPRoll * rollReductionFactor,
                TrackIRManager.Instance.RollLimits.lower, TrackIRManager.Instance.RollLimits.upper);

            X = -tirData.fNPX * (positionReductionFactor * 0.15f);
            Y = tirData.fNPY * (positionReductionFactor * 0.15f);
            Z = -tirData.fNPZ * (positionReductionFactor * 0.15f);
        }
    }
}
