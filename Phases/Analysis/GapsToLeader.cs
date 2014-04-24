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
    public class GapsToLeader : IEnumerable<GapsToLeader.ClosestGapAtTimeStamp>
    {
        public class ClosestGapAtTimeStamp
        {
            public double TimeStamp;
            public int CarIdx;
            public float Gap; //In percentage of lap
        }

        class DriverGaps
        {
            public int CarIdx;
            public float Distance;
        }

        List<ClosestGapAtTimeStamp> gaps = new List<ClosestGapAtTimeStamp>();

        double lastTimeStamp = 0;

        public void Process(DataSample data)
        {
            if (data.Telemetry.RaceLapSector.LapNumber < 1)
                return;

            if (lastTimeStamp + 20.0 > data.Telemetry.SessionTime)
                return;

            lastTimeStamp = data.Telemetry.SessionTime;

            var g = new ClosestGapAtTimeStamp();
            gaps.Add(g);
            g.TimeStamp = data.Telemetry.SessionTime;

            var distances = data.Telemetry.CarIdxDistance
                .Select((d, i) => new DriverGaps { CarIdx = i, Distance = d })
                .OrderBy(d => d.Distance)
                .ToList();

            var gap = Enumerable.Range(1, distances.Count-1)
                .Select(i => new DriverGaps
                        {
                            CarIdx = distances[i].CarIdx,
                            Distance = distances[i].Distance - distances[i - 1].Distance
                        })
                .OrderBy(d => d.Distance)
                .First();

            g.CarIdx = gap.CarIdx;
            g.Gap = gap.Distance;
        }

        public IEnumerator<GapsToLeader.ClosestGapAtTimeStamp> GetEnumerator()
        {
            return gaps.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class _GapsToLeader : IEnumerable<_GapsToLeader.GapsByLap>
    {
        public class GapsByLap
        {
            public LapSector LapSector;
            public Dictionary<int, double> GapsByCarIndex;
        }

        DataSample data;
        int numberOfDrivers;
        Dictionary<LapSector, GapsByLap> gapsOnLaps = new Dictionary<LapSector, GapsByLap>();

        LapSector currentRaceLap;
        double currentleaderTimeStamp;
        int currentLeader;
        LapSector[] lastLaps = new LapSector[64];
        Dictionary<int, double> currentGapsByCarIndex;
 
        public void Process(DataSample data)
        {
            this.data = data;
            numberOfDrivers = data.SessionData.DriverInfo.Drivers.Length;

            ProcessNewLeaderLap();

            ProcessFollowers();
        }

        public IEnumerator<_GapsToLeader.GapsByLap> GetEnumerator()
        {
            return gapsOnLaps.Values.GetEnumerator();
        }

        public GapsByLap this[LapSector lapSector]
        {
            get
            {
                return gapsOnLaps[lapSector];
            }
        }

        static readonly LapSector RaceStartLapSector = new LapSector(1, 0);

        void ProcessNewLeaderLap()
        {
            if (data.Telemetry.RaceLapSector.LapNumber < 1)
                return;

            if (currentRaceLap == data.Telemetry.RaceLapSector)
                return;

            currentRaceLap = data.Telemetry.RaceLapSector;

            Trace.WriteLine("RaceLaps: {0}".F(currentRaceLap));

            currentLeader = data.Telemetry.CarSectorIdx
                .Select((l, i) => new { LapSector = l, CarIdx = i, Pct = data.Telemetry.CarIdxLapDistPct[i] })
                .Where(l => l.LapSector == currentRaceLap)
                .OrderByDescending(l => l.Pct)
                .First()
                .CarIdx;

            currentleaderTimeStamp = data.Telemetry.SessionTime;

            currentGapsByCarIndex = new Dictionary<int, double>();
            gapsOnLaps.Add(currentRaceLap, new GapsByLap { LapSector = currentRaceLap, GapsByCarIndex = currentGapsByCarIndex });

            Trace.WriteLine("Leader {0} crossed line at {1}".F(currentLeader, currentleaderTimeStamp));

            if (currentRaceLap == RaceStartLapSector)
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
                lastLaps[startingPosition.CarIdx] = new LapSector(1, 0);
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

                if (lastLaps[i] == data.Telemetry.CarSectorIdx[i])
                    continue;

                if (data.Telemetry.CarIdxLap[i] == -1)
                    Trace.WriteLine("Driver {0} has retired".F(i));

                else if (data.Telemetry.CarIdxLap[i] == currentRaceLap.LapNumber)
                    CaptureTimeToLeader(i);

                else
                    CaptureLapsToLeader(i);

                lastLaps[i] = data.Telemetry.CarSectorIdx[i];
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
            currentGapsByCarIndex.Add(i, data.Telemetry.CarIdxLap[i] - currentRaceLap.LapNumber);
            Trace.WriteLine("Driver {0} is {1} laps down".F(i, data.Telemetry.CarIdxLap[i] - currentRaceLap.LapNumber));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
