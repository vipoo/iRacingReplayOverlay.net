using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingReplayOverlay.net.Support;

namespace iRacingReplayOverlay.net.LapAnalysis
{
    public class PositionChanges
    {
        class InterestingLaps
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
        List<InterestingLaps> laps = new List<InterestingLaps>();
        int numberOfDrivers;

        public void Process(DataSample data)
        {
            if (raceLap != data.Telemetry.RaceLaps)
            {
                numberOfDrivers = data.SessionData.DriverInfo.Drivers.Length;

                raceLap = data.Telemetry.RaceLaps;

                laps.Add(new InterestingLaps { Lap = raceLap, CarIdxDistance = data.Telemetry.CarIdxDistance });
            }
        }

        public List<DeltaLaps> LapDeltas
        {
            get
            {
                var lapDeltas = new List<DeltaLaps>();

                for (int i = 1; i < laps.Count; i++)
                {
                    var lap = laps[i];

                    Trace.WriteLine("On Lap {0}".F(lap.Lap));
                    
                    var previousPositions = laps[i - 1].CarIdxDistance
                        .Select((p, idx) => new { CarIdx = idx, Distance = p, Lap = (int)p })
                        .Where(c => c.CarIdx != 0)
                        .OrderByDescending(c => c.Distance)
                        .Select((c, p) => new { CarIdx = c.CarIdx, Position = p})
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

                            Trace.WriteLine("  Car {0} has moved {1} positions to be in position {2}".F(d.CarIdx, d.Delta, d.NewPosition));
                        }
                    }

                    lapDeltas.Add(new DeltaLaps { DeltaDetails = delta, Lap = lap.Lap });
                }

                return lapDeltas;
            }
        }
    }
}
