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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using iRacingReplayOverlay.Support;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class RecordFastestLaps
    {
        readonly OverlayData overlayData;
        OverlayData.FastLap lastFastLap;

        public int[] lastDriverLaps = new int[64];
        public double[] driverLapStartTime = new double[64];

        public double fastestLapTime = double.MaxValue;
        public double? timeToNoteFastestLap = null;

        public RecordFastestLaps(OverlayData overlayData)
        {
            this.overlayData = overlayData;
        }

        public void Process(iRacingSDK.DataSample data, TimeSpan relativeTime)
        {
            if (timeToNoteFastestLap != null && timeToNoteFastestLap.Value < data.Telemetry.SessionTime)
            {
                Trace.WriteLine("{0} Showing Driver {1} recorded a new fast lap of {2:0.00}".F(data.Telemetry.SessionTimeSpan, lastFastLap.Driver.UserName, lastFastLap.Time), "INFO");

                overlayData.FastestLaps.Add(lastFastLap);
                timeToNoteFastestLap = null;
            }

            foreach( var lap in data.Telemetry
                .CarIdxLap
                .Select((l, i) => new {CarIdx = i, Lap = l })
                .Skip(1)
                .Take(data.SessionData.DriverInfo.Drivers.Length-1))
            {
                if (lap.Lap == -1)
                    continue;

                if( lap.Lap == lastDriverLaps[lap.CarIdx]+1)
                {
                    var lapTime = data.Telemetry.SessionTime - driverLapStartTime[lap.CarIdx];

                    driverLapStartTime[lap.CarIdx] = data.Telemetry.SessionTime;
                    lastDriverLaps[lap.CarIdx] = lap.Lap;

                    if( lap.Lap > 1 && lapTime < fastestLapTime)
                    {
                        fastestLapTime = lapTime;

                        lastFastLap = new OverlayData.FastLap
                        {
                            Time = lapTime,
                            StartTime = (int)relativeTime.TotalSeconds,
                            Driver = new OverlayData.Driver
                            {
                                UserName = data.SessionData.DriverInfo.Drivers[lap.CarIdx].UserName,
                                CarNumber = (int)data.SessionData.DriverInfo.Drivers[lap.CarIdx].CarNumber
                            }
                        };

                        if (timeToNoteFastestLap == null)
                            timeToNoteFastestLap = data.Telemetry.SessionTime + 20;
                        Trace.WriteLine("{0} Driver {1} recorded a new fast lap of {2:0.00}".F(data.Telemetry.SessionTimeSpan, lastFastLap.Driver.UserName, lastFastLap.Time), "INFO");
                    }
                }
            }
        }
    }
}
