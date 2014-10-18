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
// You should have received a copy of the GNU General Public License333
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.

using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.Linq;
using iRacingReplayOverlay.Support;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class CaptureCamDriver
    {
        readonly OverlayData overlayData;

        public CaptureCamDriver(OverlayData overlayData)
        {
            this.overlayData = overlayData;
        }

        OverlayData.Driver lastCamDriver = null;
        
        public void Process(DataSample data, TimeSpan relativeTime)
        {
            var camDriver = CreateCamDriver(data, relativeTime);
            if (camDriver != null)
            {
                if (lastCamDriver == null || 
                    lastCamDriver.UserName != camDriver.CurrentDriver.UserName ||
                    lastCamDriver.Position != camDriver.CurrentDriver.Position)
                {
                    var position = camDriver.CurrentDriver.Position != null ? camDriver.CurrentDriver.Position.Value.ToString() : "";
                    var indicator = camDriver.CurrentDriver.Position != null ? camDriver.CurrentDriver.Position.Value.Ordinal() : "";

                    TraceInfo.WriteLine("{0} Camera on {1} {2} in position {3}{4}",
                        data.Telemetry.SessionTimeSpan,
                        camDriver.CurrentDriver.UserName,
                        camDriver.CurrentDriver.CarNumber,
                        position, indicator);

                    lastCamDriver = camDriver.CurrentDriver;
                }
                overlayData.CamDrivers.Add(camDriver);
            }
        }

        OverlayData.CamDriver CreateCamDriver(DataSample data, TimeSpan relativeTime)
        {
            var driver = GetCurrentDriverDetails(data);

            if (driver == null)
                return null;

            return new OverlayData.CamDriver
            {
                StartTime = relativeTime.TotalSeconds,
                CurrentDriver = driver,
            };
        }

        static OverlayData.Driver GetCurrentDriverDetails(DataSample data)
        {
            var car = data.Telemetry.CamCar;
            if (car == null)
                return null;

            var position = GetPositionFor(data, car);

            var driver = new OverlayData.Driver
            {
                CarIdx = car.CarIdx,
                CarNumber = car.CarNumberDisplay,
                UserName = car.UserName,
                Position = position,
                PitStopCount = car.PitStopCount
            };

            return driver;
        }

        private static int? GetPositionFor(DataSample data, Car car)
        {
            if (data.Telemetry.RaceDistance > 1.10)
                return car.Position;

            var session = data.SessionData.SessionInfo.Sessions.Qualifying();
            if (session == null || session.ResultsPositions == null)
                return null;

            var qualifyingResult = session.ResultsPositions.FirstOrDefault(p => p.CarIdx == car.CarIdx);
            if (qualifyingResult == null)
                return null;

            return (int)qualifyingResult.Position;
        }
    }
}
