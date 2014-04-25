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
                .Select((d, i) => new  { CarIdx = i, Distance = d })
                .OrderBy(d => d.Distance)
                .ToList();

            var gap = Enumerable.Range(1, distances.Count-1)
                .Select(i => new 
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
}
