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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Analysis
{
    public class PositionChanges : IEnumerable<PositionChanges.DeltaLaps>
    {
        class CarDistances
        {
            public int Lap;
            public float[] CarIdxDistance;
        }

        public class DeltaLaps
        {
            public int Lap;
            public IEnumerable<DeltaDetail> DeltaDetails;
        }

        public class DeltaDetail
        {
            public int CarIdx;
            public int Delta;
            public int NewPosition;
        }

        int raceLap = 0;
        List<CarDistances> carDistances = new List<CarDistances>();
        int numberOfDrivers;
        Dictionary<int, DeltaLaps> deltaLaps;

        public void Process(DataSample data)
        {
            if (raceLap == data.Telemetry.RaceLaps)
                return;
            
            numberOfDrivers = data.SessionData.DriverInfo.Drivers.Length;

            raceLap = data.Telemetry.RaceLaps;

            carDistances.Add(new CarDistances { Lap = raceLap, CarIdxDistance = data.Telemetry.CarIdxDistance });
        }

        public DeltaLaps this[int lapNumber]
        {
            get
            {
                BuildListOfDeltaLaps();
                return deltaLaps[lapNumber];    
            }
        }

        public IEnumerator<PositionChanges.DeltaLaps> GetEnumerator()
        {
            BuildListOfDeltaLaps();
            return deltaLaps.Values.GetEnumerator();
        }

        void BuildListOfDeltaLaps()
        {
            if (deltaLaps != null)
                return;

            deltaLaps = new Dictionary<int, DeltaLaps>();

            for (int i = 1; i < carDistances.Count; i++)
                CalculatePositionChanges(carDistances[i-1], carDistances[i]);
        }

        private void CalculatePositionChanges(CarDistances previousLap, CarDistances lap)
        {
            var previousPositions = previousLap.CarIdxDistance
                .Select((p, idx) => new { CarIdx = idx, Distance = p, Lap = (int)p })
                .Where(c => c.CarIdx != 0)
                .OrderByDescending(c => c.Distance)
                .Select((c, p) => new { CarIdx = c.CarIdx, Position = p })
                .ToDictionary(kv => kv.CarIdx, kv => kv.Position);

            var positions = lap.CarIdxDistance
                .Select((d, idx) => new { CarIdx = idx, Distance = d, Lap = (int)d })
                .Where(c => c.CarIdx != 0)
                .OrderByDescending(c => c.Distance)
                .Select((c, p) => new { CarIdx = c.CarIdx, Position = p });

            var delta = new List<DeltaDetail>();

            foreach (var car in positions)
            {
                int previousPosition;
                if (previousPositions.TryGetValue(car.CarIdx, out previousPosition))
                {
                    var d = new DeltaDetail { CarIdx = car.CarIdx, NewPosition = car.Position, Delta = previousPosition - car.Position };
                    delta.Add(d);

                    Trace.WriteLine("Car {0} has moved {1} positions to be in position {2}".F(d.CarIdx, d.Delta, d.NewPosition));
                }
            }

            deltaLaps.Add(lap.Lap, new DeltaLaps { DeltaDetails = delta, Lap = lap.Lap });
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            BuildListOfDeltaLaps();
            return deltaLaps.Values.GetEnumerator();
        }
    }
}
