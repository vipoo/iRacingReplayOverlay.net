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
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class RuleFirstSectors : IDirectionRule
    {
        readonly RemovalEdits removalEdits;
        readonly TrackCamera TV3;

        DateTime reselectLeaderAt = DateTime.Now;

        public RuleFirstSectors(TrackCamera[] cameras, RemovalEdits removalEdits)
        {
            this.removalEdits = removalEdits;
            TV3 = cameras.First(tc => tc.CameraName == "TV3");
        }

        public bool IsActive(DataSample data)
        {
            return OnFirstSecotrs(data);
        }

        public void Direct(DataSample data)
        {
            SwitchToLeader(data);
        }

        bool OnFirstSecotrs(DataSample data)
        {
            return data.Telemetry.RaceLapSector.LapNumber < 1 || (data.Telemetry.RaceLapSector.LapNumber == 1 && data.Telemetry.RaceLapSector.Sector < 2);
        }

        void SwitchToLeader(DataSample data)
        {
            removalEdits.InterestingThingHappend(InterestState.FirstLap, -1);

            if (reselectLeaderAt < DateTime.Now)
            {
                iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);

                reselectLeaderAt = DateTime.Now + 5.Seconds(); ;
            }
        }

        public string Name
        {
            get { return GetType().Name; }
        }
    }
}
