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

using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Direction;
using iRacingSDK;
using System;
using System.Linq;
using System.Threading;

namespace IRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        ReplayControl replayControl;
        int raceStartFrameNumber = 0;

        public void _AnalyseRace(Action onComplete)
        {
            var hwnd = Win32.Messages.FindWindow(null, "iRacing.com Simulator");
            Win32.Messages.ShowWindow(hwnd, Win32.Messages.SW_SHOWNORMAL);
            Thread.Sleep(2000);

            var gapsToLeader = new GapsToLeader();
            var positionChanges = new PositionChanges();
            var lapsToFrameNumbers = new LapsToFrameNumbers();

            foreach (var data in iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(16, d => d.Telemetry.SessionState != SessionState.Racing)
                .AtSpeed(16, d => d.Telemetry.SessionState == SessionState.Racing)
                .RaceOnly()
                .TakeWhile(d => d.Telemetry.RaceLaps < 6))
            {

                if (raceStartFrameNumber == 0 && data.Telemetry.SessionState == SessionState.Racing)
                {
                    raceStartFrameNumber = data.Telemetry.ReplayFrameNum - (60*20);
                }

                gapsToLeader.Process(data);
                positionChanges.Process(data);
                lapsToFrameNumbers.Process(data);
            }
            
            var incidents = new Incidents();
            //foreach (var data in iRacing.GetDataFeed().RaceIncidents())
            //    incidents.Process(data);
            //incidents.Stop();

            replayControl = ReplayControlFactory.CreateFrom(incidents, gapsToLeader, positionChanges, lapsToFrameNumbers);

            onComplete();
        }
    }
}
