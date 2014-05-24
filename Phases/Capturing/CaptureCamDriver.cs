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

using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class CaptureCamDriver
    {
        readonly OverlayData overlayData;

        public CaptureCamDriver(OverlayData overlayData)
        {
            this.overlayData = overlayData;
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            var camDriver = CreateCamDriver(data, relativeTime);
            if (camDriver != null)
                overlayData.CamDrivers.Add(camDriver);
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

            var driver = new OverlayData.Driver
            {
                CarIdx = car.CarIdx,
                CarNumber = car.CarNumber,
                Indicator = car.Position.Ordinal(),
                UserName = car.UserName,
                Position = car.Position
            };

            return driver;
        }
    }
}
