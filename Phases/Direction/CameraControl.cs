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

using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class CameraControl
    {
        readonly TrackCamera[] cameras;
        readonly Random random;
        readonly TrackCamera defaultCamera;

        public CameraControl(TrackCamera[] cameras)
        {
            this.cameras = cameras;
            this.random = new Random();

            defaultCamera = cameras.First(tc => tc.IsRaceStart);
        }
        
        public TrackCamera FindACamera(params CameraAngle[] cameraAngles)
        {
            return FindACamera(cameraAngles as IEnumerable<CameraAngle>);
        }

        public TrackCamera FindACamera(IEnumerable<CameraAngle> cameraAngles)
        {
            var rand = 0;
            var offset = 0;
            var camera = defaultCamera;

            var selectableCameras = cameras.Where(x => cameraAngles.Contains(x.CameraAngle));
            int total = selectableCameras.Sum(x => x.Ratio);

            // If no camera within specified cameraAngles has non zero ratio select among all
            if (total == 0)
            {
                selectableCameras = cameras;
                rand = random.Next(100);
            }
            else
            {
                rand = random.Next(total);
            }

            foreach (var tc in selectableCameras)
            {
                if (rand < tc.Ratio + offset)
                {
                    camera = tc;
                    break;
                }
                offset += tc.Ratio;
            }

            return camera;
        }
    }
}
