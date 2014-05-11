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

using iRacingReplayOverlay.Phases.Direction;
using iRacingSDK;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace iRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        void _CaptureOpeningScenes(Action onComplete)
        {
            var data = iRacing.GetDataFeed().First();
            iRacing.Replay.MoveToQualifying();
            data = iRacing.GetDataFeed().First();
            var f = data.Telemetry.ReplayFrameNum;
            iRacing.Replay.MoveToFrame(f + 60 * 4);
            iRacing.Replay.SetSpeed(1);

            var scenicCameras = data.SessionData.CameraInfo.Groups.First( c => c.GroupName == "Scenic").GroupNum;
            var aCar = data.SessionData.DriverInfo.Drivers[1].CarNumber;
            iRacing.Replay.CameraOnDriver((short)aCar, (short)scenicCameras);

            var videoCapture = new VideoCapture();

            videoCapture.Activate();

            Thread.Sleep(4000);

            videoCapture.Deactivate();
        }
    }
}
