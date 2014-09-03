// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;

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
