using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay
{
    public enum TrackCameraAngle
    {
        LookingInfrontOfCar,
        LookingBehindCar,
        LookingAtCar,
        LookingAtTrack
    }

    public class TrackCameras : List<TrackCamera>
    {
    }

    public class TrackCamera
    {
        public string TrackName;
        public string CameraName;
        public int Ratio;
        public short CameraNumber;
        public TrackCameraAngle CameraAngle;
    }
}
