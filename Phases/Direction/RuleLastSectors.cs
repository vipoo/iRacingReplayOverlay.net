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
using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class RuleLastSectors
    {
        readonly RemovalEdits removalEdits;
        readonly TrackCamera[] cameras;
        readonly TrackCamera TV2;

        int lastFinisherCarIdx = -1;
        DateTime timeOfFinisher = DateTime.Now;

        public RuleLastSectors(TrackCamera[] cameras, RemovalEdits removalEdits)
        {
            this.cameras = cameras;
            this.removalEdits = removalEdits;

            TV2 = cameras.First(tc => tc.CameraName == "TV2");
        }
        
        public bool Process(DataSample data)
        {
            if (!OnLastSectors(data))
                return false;

            SwitchToFinishingDrivers(data);

            return true;
        }

        bool OnLastSectors(DataSample data)
        {
            var totalLaps = data.Telemetry.Session.ResultsLapsComplete;
            return data.Telemetry.RaceLapSector >= new LapSector((int)totalLaps, 1);
        }

        void SwitchToFinishingDrivers(DataSample data)
        {
            removalEdits.InterestingThingHappend(data);

            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            if (lastFinisherCarIdx != -1 && !data.Telemetry.Cars[lastFinisherCarIdx].HasSeenCheckeredFlag)
            {
                timeOfFinisher = DateTime.Now.AddSeconds(2);
                return;
            }

            if (timeOfFinisher > DateTime.Now)
                return;

            Car nextFinisher;

            if (!data.Telemetry.LeaderHasFinished)
                nextFinisher = data.Telemetry.Cars.First(c => c.Position == 1);
            else
                nextFinisher = data.Telemetry.Cars
                        .Where(c => c.TotalDistance > 0)
                        .Where(c => !c.HasSeenCheckeredFlag)
                        .Where(c => !c.IsPaceCar)
                        .Where(c => c.HasData)
                        .OrderByDescending(c => c.DistancePercentage)
                        .FirstOrDefault();

            if (nextFinisher == null)
                return;

            Trace.WriteLine("{0} Found {1} in position {2}".F(data.Telemetry.SessionTimeSpan, nextFinisher.UserName, nextFinisher.Position), "DEBUG");

            timeOfFinisher = DateTime.Now;
            lastFinisherCarIdx = nextFinisher.CarIdx;

            Trace.WriteLine("{0} Switching camera to {1} as they cross finishing line in position {2}".F(data.Telemetry.SessionTimeSpan, nextFinisher.UserName, nextFinisher.Position), "INFO");

            iRacing.Replay.CameraOnDriver(nextFinisher.CarNumber, TV2.CameraNumber);
        }
    }
}
