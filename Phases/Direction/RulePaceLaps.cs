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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class RulePaceLaps : IVetoRule
    {
        readonly RemovalEdits removalEdits;
        readonly TrackCamera TV3;

        bool wasUnderPaceCar;
        TimeSpan restartEndTime;
        readonly TimeSpan RestartStickyTime = 20.Seconds();
        bool restarting = false;

        public RulePaceLaps(TrackCamera[] cameras, RemovalEdits removalEdits)
        {
            this.removalEdits = removalEdits;

            TV3 = cameras.First(tc => tc.CameraName == "TV3");

            wasUnderPaceCar = false;
        }

        public bool IsActive(DataSample data)
        {
            if (restarting)
            {
                if (data.Telemetry.SessionTimeSpan < restartEndTime)
                    return true;

                removalEdits.InterestingThingStopped(InterestState.Restart, data.Telemetry.CamCarIdx);

                restarting = false;
                return false;
            }

            if (wasUnderPaceCar)
            {
                if( data.Telemetry.UnderPaceCar)
                    return true;

                restartEndTime = data.Telemetry.SessionTimeSpan + RestartStickyTime;
                restarting = true;

                TraceInfo.WriteLine("{0} Race restarting", data.Telemetry.SessionTimeSpan);
                wasUnderPaceCar = false;
                removalEdits.InterestingThingStarted(InterestState.Restart, data.Telemetry.CamCarIdx);
                return true;
            }

            wasUnderPaceCar = data.Telemetry.UnderPaceCar;
            if (wasUnderPaceCar)
            {
                TraceInfo.WriteLineIf(wasUnderPaceCar, "{0} Double Yellows. Pace Car", data.Telemetry.SessionTimeSpan);
                iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);
            }

            return wasUnderPaceCar;
        }

        public void Direct(DataSample data)
        {
        }

        public void Redirect(DataSample data)
        {
            iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);
        }

        public string Name
        {
            get { return GetType().Name; }
        }
    }
}
