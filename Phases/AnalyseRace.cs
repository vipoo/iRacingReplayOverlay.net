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
using iRacingSDK;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using iRacingReplayOverlay.Support;
using iRacingSDK.Support;

namespace iRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        int raceStartFrameNumber = 0;
        internal Incidents incidents;

        public void _AnalyseRace(Action onComplete)
        {
            var hwnd = Win32.Messages.FindWindow(null, "iRacing.com Simulator");
            Win32.Messages.ShowWindow(hwnd, Win32.Messages.SW_SHOWNORMAL);
            Win32.Messages.SetForegroundWindow(hwnd);
            Thread.Sleep(2000);

            var data = iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(16)
                .RaceOnly()
                .First(d => d.Telemetry.SessionState == SessionState.Racing);

            raceStartFrameNumber = data.Telemetry.ReplayFrameNum - (60 * 20);

            if (raceStartFrameNumber < 0)
            {
                TraceInfo.WriteLine("Unable to start capturing at 20 seconds prior to race start.  Starting at start of replay file.");
                raceStartFrameNumber = 0;
            }

            AnalyseIncidents();

            onComplete();
        }

        void AnalyseIncidents()
        {
            iRacing.Replay.MoveToFrame(raceStartFrameNumber);

            incidents = new Incidents();

            var incidentSamples = iRacing.GetDataFeed().RaceIncidents(shortTestOnly ? 12 : int.MaxValue);

            foreach (var data in incidentSamples)
                incidents.Process(data);
        }
    }
}
