using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine("Capturing data for lap {0}", raceLap);
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

                    var previousPositions = new Dictionary<int, int>();
                    foreach( var pp in laps[i - 1].CarIdxDistance
                        .Select((p, idx) => new { CarIdx = idx, Distance = p, Lap = (int)p })
                        .Where(c => c.CarIdx != 0)
                        .OrderByDescending(c => c.Distance)
                        .Select((c, p) => new { CarIdx = c.CarIdx, Position = p}))
                        previousPositions.Add(pp.CarIdx, pp.Position);

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
                            delta.Add(new DeltaDetail { CarIdx = car.CarIdx, NewPosition = car.Position, Delta = previousPosition - car.Position });
                    }

                    lapDeltas.Add(new DeltaLaps { DeltaDetails = delta, Lap = lap.Lap });
                }

                return lapDeltas;
            }
        }
    }
}
