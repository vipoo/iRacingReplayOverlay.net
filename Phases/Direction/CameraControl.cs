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

using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class CameraControl
    {
        readonly TrackCamera[] cameras;
        readonly Random random;
        readonly TrackCamera defaultCamera;

        public short LastLapCameraNumber
        {
            get
            {
                return cameras.First(tc => tc.IsLastLap).CameraNumber;
            }
        }

        public short IncidentCameraNumber
        {
            get
            {
                return cameras.First(tc => tc.IsIncident).CameraNumber;
            }
        }

        public short RaceStartCameraNumber
        {
            get
            {
                return cameras.First(tc => tc.IsRaceStart).CameraNumber;
            }
        }

        public CameraControl(TrackCamera[] cameras)
        {
            this.cameras = cameras;
            this.random = new Random();

            defaultCamera = cameras.First(tc => tc.IsRaceStart);
        }
        
        public TrackCamera FindACamera(IEnumerable<CameraAngle> cameraAngles, TrackCamera adjustedCamera = null, int adjustRatioBy = 1)
        {
            if (adjustedCamera != null)
                TraceInfo.WriteLine("Adjusting ratio for camera {0} by 1/{1}", adjustedCamera.CameraName, adjustRatioBy);

            var rand = 0;
            var offset = 0;
            var camera = defaultCamera;

            var selectableCameras = cameras.Where(x => cameraAngles.Contains(x.CameraAngle));
            int total = selectableCameras.Sum(x => x == adjustedCamera ? x.Ratio / adjustRatioBy: x.Ratio);

            // If no camera within specified cameraAngles has non zero ratio select among all
            if (total == 0)
            {
                selectableCameras = cameras;
                rand = random.Next(100);
            }
            else
                rand = random.Next(total);

            foreach (var tc in selectableCameras)
            {
                var ratio = tc == adjustedCamera ? tc.Ratio / adjustRatioBy : tc.Ratio;

                if (rand < ratio + offset)
                {
                    camera = tc;
                    break;
                }
                offset += ratio;
            }

            return camera;
        }

        public void CameraOnDriver(short carNumber, short group, short camera = 0)
        {
            iRacing.Replay.CameraOnDriver(carNumber, group, camera);
            TraceDebug.WriteLine("CameraOnDriver called for carNumber: {0} cameraGroup: {1} camera: {2}".F(carNumber, group, camera));
        }

        public void CameraOnPositon(short carPosition, short group, short camera = 0)
        {
            iRacing.Replay.CameraOnPositon(carPosition, group, camera);
            TraceDebug.WriteLine("CameraOnPositon called for carNumber: {0} cameraGroup: {1} camera: {2}".F(carPosition, group, camera));
        }
    }
}
