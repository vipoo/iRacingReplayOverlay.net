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

using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Analysis
{
    public class GapsToLeader : IEnumerable<GapsToLeader.GapsByLap>
    {
        public class GapsByLap
        {
            public int Lap;
            public Dictionary<int, double> GapsByCarIndex;
        }

        DataSample data;
        int numberOfDrivers;
        Dictionary<int, GapsByLap> gapsOnLaps = new Dictionary<int, GapsByLap>();

        int currentRaceLap;
        double currentleaderTimeStamp;
        int currentLeader;
        int[] lastLaps = new int[64];
        Dictionary<int, double> currentGapsByCarIndex;
 
        public void Process(DataSample data)
        {
            this.data = data;
            numberOfDrivers = data.SessionData.DriverInfo.Drivers.Length;

            ProcessNewLeaderLap();

            ProcessFollowers();
        }

        public IEnumerator<GapsToLeader.GapsByLap> GetEnumerator()
        {
            return gapsOnLaps.Values.GetEnumerator();
        }

        public GapsByLap this[int lapNumber]
        {
            get
            {
                return gapsOnLaps[lapNumber];
            }
        }

        void ProcessNewLeaderLap()
        {
            if (currentRaceLap == data.Telemetry.RaceLaps)
                return;

            currentRaceLap = data.Telemetry.RaceLaps;

            Trace.WriteLine("RaceLaps: {0}".F(currentRaceLap));

            currentLeader = data.Telemetry.CarIdxLap
                .Select((l, i) => new { Lap = l, CarIdx = i, Pct = data.Telemetry.CarIdxLapDistPct[i] })
                .Where(l => l.Lap == currentRaceLap)
                .OrderByDescending(l => l.Pct)
                .First()
                .CarIdx;

            currentleaderTimeStamp = data.Telemetry.SessionTime;

            currentGapsByCarIndex = new Dictionary<int, double>();
            gapsOnLaps.Add(currentRaceLap, new GapsByLap { Lap = currentRaceLap, GapsByCarIndex = currentGapsByCarIndex });

            Trace.WriteLine("Leader {0} crossed line at {1}".F(currentLeader, currentleaderTimeStamp));

            if (currentRaceLap == 1)
                ProcessStartingGrid();
        }

        void ProcessStartingGrid()
        {
            foreach (var startingPosition in data.Telemetry
                .CarIdxLapDistPct
                .Select((p, i) => new { CarIdx = i, Pct = p })
                .Where(l => l.CarIdx < numberOfDrivers && l.CarIdx >= 1 && l.CarIdx != currentLeader)
                .OrderByDescending(l => l.Pct))
            {
                lastLaps[startingPosition.CarIdx] = 1;
                currentGapsByCarIndex.Add(startingPosition.CarIdx, 0);

                Trace.WriteLine("Driver {0} starting behind leader".F(startingPosition.CarIdx));
            }
        }

        void ProcessFollowers()
        {
            for (int i = 1; i < numberOfDrivers; i++)
            {
                if (i == currentLeader)
                    continue;

                if (lastLaps[i] == data.Telemetry.CarIdxLap[i])
                    continue;

                if (data.Telemetry.CarIdxLap[i] == -1)
                    Trace.WriteLine("Driver {0} has retired".F(i));

                else if (data.Telemetry.CarIdxLap[i] == currentRaceLap)
                    CaptureTimeToLeader(i);

                else
                    CaptureLapsToLeader(i);

                lastLaps[i] = data.Telemetry.CarIdxLap[i];
            }
        }

        void CaptureTimeToLeader(int i)
        {
            var gap = data.Telemetry.SessionTime - currentleaderTimeStamp;
            if (currentGapsByCarIndex.ContainsKey(i))
                Trace.WriteLine("Driver passed {0} start finished twice - {1}!!".F(i, gap));
            else
                currentGapsByCarIndex.Add(i, gap);

            Trace.WriteLine("Driver {0} cross {1} seconds after leader".F(i, gap));
        }

        void CaptureLapsToLeader(int i)
        {
            currentGapsByCarIndex.Add(i, data.Telemetry.CarIdxLap[i] - currentRaceLap);
            Trace.WriteLine("Driver {0} is {1} laps down".F(i, data.Telemetry.CarIdxLap[i] - currentRaceLap));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
