using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay
{
    public enum CameraAngle
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
        static Dictionary<string, CameraAngle> cameraAngles = new Dictionary<string, CameraAngle>
        {
            { "Nose", CameraAngle.LookingInfrontOfCar },
            { "Gearbox", CameraAngle.LookingBehindCar },
            { "Roll Bar", CameraAngle.LookingInfrontOfCar },
            { "LF Susp", CameraAngle.LookingInfrontOfCar },
            { "RF Susp", CameraAngle.LookingInfrontOfCar },
            { "LR Susp", CameraAngle.LookingBehindCar },
            { "RR Susp", CameraAngle.LookingBehindCar },
            { "Gyro", CameraAngle.LookingInfrontOfCar },
            { "Cockpit", CameraAngle.LookingInfrontOfCar },
            { "Blimp", CameraAngle.LookingAtCar },
            { "Chopper", CameraAngle.LookingAtCar },
            { "Chase", CameraAngle.LookingInfrontOfCar },
            { "Rear Chase", CameraAngle.LookingBehindCar },
            { "Far Chase", CameraAngle.LookingAtCar },
            { "TV1", CameraAngle.LookingAtCar },
            { "TV2", CameraAngle.LookingAtCar },
            { "TV3", CameraAngle.LookingAtCar }
        };

        public string TrackName;
        public string CameraName;
        public int Ratio;
        public short CameraNumber;
        public CameraAngle CameraAngle
        {
            get
            {
                CameraAngle cameraAngle;

                if(cameraAngles.TryGetValue(CameraName, out cameraAngle))
                    return cameraAngle;
                else 
                    return CameraAngle.LookingAtTrack;
            }
        }
    }
}
