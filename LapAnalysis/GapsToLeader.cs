using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingReplayOverlay.net.Support;
using System.Diagnostics;

namespace iRacingReplayOverlay.net.LapAnalysis
{
    public class GapsToLeader
    {
        public class GapsByLap
        {
            public int Lap;
            public Dictionary<int, double> GapsByCarIndex;
        }

        int lastRaceLaps;
        int currentLeader;
        double currentleaderTimeStamp;
        int[] lastLaps = new int[64];

        List<GapsByLap> gapsOnLaps = new List<GapsByLap>();
        GapsByLap gapsForCurrentLap;
        private int numberOfDrivers;
        private int raceLaps;
        private DataSample data;

        public List<GapsByLap> GapsByLaps
        {
            get { return gapsOnLaps; }
        }

        public void Process(DataSample data)
        {
            this.data = data;
            raceLaps = data.Telemetry.RaceLaps;
            numberOfDrivers = data.SessionData.DriverInfo.Drivers.Length;

            if (lastRaceLaps != raceLaps)
                lastRaceLaps = ProcessNewLeaderLap();

            ProcessFollowers(gapsForCurrentLap.GapsByCarIndex);
        }

        private void ProcessFollowers(Dictionary<int, double> gapsByCarIndex)
        {
            for (int i = 1; i < numberOfDrivers; i++)
            {
                if (i == currentLeader)
                    continue;

                if (lastLaps[i] != data.Telemetry.CarIdxLap[i])
                {
                    if (data.Telemetry.CarIdxLap[i] == raceLaps)
                    {
                        var gap = data.Telemetry.SessionTime - currentleaderTimeStamp;
                        gapsByCarIndex.Add(i, gap);

                        Trace.WriteLine("Driver {0} cross {1} seconds after leader".F(i, gap));
                    }
                    else
                    {
                        gapsByCarIndex.Add(i, data.Telemetry.CarIdxLap[i] - raceLaps);
                        Trace.WriteLine("Driver {0} is {1} laps down".F(i, data.Telemetry.CarIdxLap[i] - raceLaps));
                    }
                }

                lastLaps[i] = data.Telemetry.CarIdxLap[i];
            }
        }

        private int ProcessNewLeaderLap()
        {
            Trace.WriteLine("RaceLaps: {0}".F(raceLaps));

            currentLeader = data.Telemetry.CarIdxLap
                .Select((l, i) => new { Lap = l, CarIdx = i, Pct = data.Telemetry.CarIdxLapDistPct[i] })
                .Where(l => l.Lap == raceLaps)
                .OrderByDescending(l => l.Pct)
                .First()
                .CarIdx;

            currentleaderTimeStamp = data.Telemetry.SessionTime;

            gapsForCurrentLap = new GapsByLap { Lap = raceLaps, GapsByCarIndex = new Dictionary<int, double>() };
            gapsOnLaps.Add(gapsForCurrentLap);
            Trace.WriteLine("Leader {0} crossed line at {1}".F(currentLeader, currentleaderTimeStamp));

            if (raceLaps == 1)
                ProcessStartingGrid();

            return raceLaps;
        }

        private void ProcessStartingGrid()
        {
            foreach (var startingPosition in data.Telemetry
                .CarIdxLapDistPct
                .Select((p, i) => new { CarIdx = i, Pct = p })
                .Where(l => l.CarIdx < numberOfDrivers && l.CarIdx >= 1 && l.CarIdx != currentLeader)
                .OrderByDescending(l => l.Pct))
            {
                lastLaps[startingPosition.CarIdx] = raceLaps;
                gapsForCurrentLap.GapsByCarIndex.Add(startingPosition.CarIdx, 0);

                Trace.WriteLine("Driver {0} starting behind leader".F(startingPosition.CarIdx));
            }
        }
    }
}

