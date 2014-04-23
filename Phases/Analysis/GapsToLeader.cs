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

        int numberOfDrivers;
        int currentRaceLap;
        int[] lastLaps = new int[64];
        Dictionary<int, GapsByLap> gapsOnLaps = new Dictionary<int, GapsByLap>();
        GapsByLap gapsForCurrentLap;
        DataSample data;
        double currentleaderTimeStamp;
        int currentLeader;

        public void Process(DataSample data)
        {
            this.data = data;
            numberOfDrivers = data.SessionData.DriverInfo.Drivers.Length;

            ProcessNewLeaderLap();

            ProcessFollowers(gapsForCurrentLap.GapsByCarIndex);
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

        private void ProcessFollowers(Dictionary<int, double> gapsByCarIndex)
        {
            for (int i = 1; i < numberOfDrivers; i++)
            {
                if (i == currentLeader)
                    continue;

                if (lastLaps[i] != data.Telemetry.CarIdxLap[i])
                {
                    if (data.Telemetry.CarIdxLap[i] == currentRaceLap)
                    {
                        var gap = data.Telemetry.SessionTime - currentleaderTimeStamp;
                        if (gapsByCarIndex.ContainsKey(i))
                            Trace.WriteLine("Driver passed {0} start finished twice - {1}!!".F(i, gap));
                        else
                            gapsByCarIndex.Add(i, gap);

                        Trace.WriteLine("Driver {0} cross {1} seconds after leader".F(i, gap));
                    }
                    else if( data.Telemetry.CarIdxLap[i] == -1)
                    {   //Retired
                        Trace.WriteLine("Driver {0} has retired".F(i));
                    } 
                    else
                    {
                        Trace.WriteLine("--Adding  {0}".F(data.SessionData.DriverInfo.Drivers[i].UserName));

                        gapsByCarIndex.Add(i, data.Telemetry.CarIdxLap[i] - currentRaceLap);
                        Trace.WriteLine("Driver {0} is {1} laps down".F(i, data.Telemetry.CarIdxLap[i] - currentRaceLap));
                    }
                }

                lastLaps[i] = data.Telemetry.CarIdxLap[i];
            }
        }

        private void ProcessNewLeaderLap()
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

            gapsForCurrentLap = new GapsByLap { Lap = currentRaceLap, GapsByCarIndex = new Dictionary<int, double>() };
            gapsOnLaps.Add(currentRaceLap, gapsForCurrentLap);
            Trace.WriteLine("Leader {0} crossed line at {1}".F(currentLeader, currentleaderTimeStamp));

            if (currentRaceLap == 1)
                ProcessStartingGrid();
        }

        private void ProcessStartingGrid()
        {
            foreach (var startingPosition in data.Telemetry
                .CarIdxLapDistPct
                .Select((p, i) => new { CarIdx = i, Pct = p })
                .Where(l => l.CarIdx < numberOfDrivers && l.CarIdx >= 1 && l.CarIdx != currentLeader)
                .OrderByDescending(l => l.Pct))
            {
                lastLaps[startingPosition.CarIdx] = 1;
                gapsForCurrentLap.GapsByCarIndex.Add(startingPosition.CarIdx, 0);

                Trace.WriteLine("Driver {0} starting behind leader".F(startingPosition.CarIdx));
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

