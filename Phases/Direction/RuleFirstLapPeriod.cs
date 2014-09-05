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
    public class RuleFirstLapPeriod : IDirectionRule
    {
        readonly RemovalEdits removalEdits;
        readonly TrackCamera TV3;

        DateTime reselectLeaderAt = DateTime.Now;
        bool startedFirstLapPeriod = false;
        bool completedFirstLapPeriod = false;
        bool raceHasStarted;
        TimeSpan raceStartTime;

        public RuleFirstLapPeriod(TrackCamera[] cameras, RemovalEdits removalEdits)
        {
            this.removalEdits = removalEdits;
            TV3 = cameras.First(tc => tc.CameraName == "TV3");
        }

        public bool IsActive(DataSample data)
        {
            var isInFirstPeriod = InFirstLapPeriod(data);

            if (isInFirstPeriod)
                OnlyOnce(ref startedFirstLapPeriod, () => TraceInfo.WriteLine("{0} Tracking leader from race start for period of {1}".F(data.Telemetry.SessionTimeSpan, Settings.Default.FollowLeaderAtRaceStartPeriod)));
            else
                OnlyOnce(ref completedFirstLapPeriod, () => TraceInfo.WriteLine("{0} Leader has completed first lap period.  Activating normal camera/driver selection rules.".F(data.Telemetry.SessionTimeSpan)));

            return isInFirstPeriod;
        }

        public void Direct(DataSample data)
        {
            SwitchToLeader(data);
        }

        bool InFirstLapPeriod(DataSample data)
        {
            if( !raceHasStarted)
            {
                raceHasStarted = data.Telemetry.SessionState == SessionState.Racing;
                raceStartTime = data.Telemetry.SessionTimeSpan;
                return true;
            }

            return data.Telemetry.SessionTimeSpan < raceStartTime + Settings.Default.FollowLeaderAtRaceStartPeriod;
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

        void OnlyOnce(ref bool latch, Action action)
        {
            if (!latch)
                action();

            latch = true;
        }

        public string Name
        {
            get { return GetType().Name; }
        }
    }
}
