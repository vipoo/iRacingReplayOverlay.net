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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Support;
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class RuleRandomDriver : IVetoRule
    {
        readonly CameraControl cameraControl;        
        readonly SessionData sessionData;
        readonly TimeSpan stickyTime;
        readonly long[] preferredCarIndexes;
        readonly Random randomDriverNumber;

        bool isWatchingRandomDriver;
        TimeSpan finishWatchingRandomDriverAt;
        SessionData._DriverInfo._Drivers car;
        TrackCamera camera;

        public RuleRandomDriver(CameraControl cameraControl, SessionData sessionData, TimeSpan stickyTime)
        {
            this.cameraControl = cameraControl;
            this.sessionData = sessionData;
            this.stickyTime = stickyTime;

            if (Settings.Default.PreferredDriverNames != null && Settings.Default.PreferredDriverNames.Length > 0)
            {
                var preferredDriverNames = Settings.Default.PreferredDriverNames.Split(new char[] { ',', ';' }).Select(name => name.Trim().ToLower()).ToList();

                preferredCarIndexes = sessionData.DriverInfo.Drivers.Where(x => preferredDriverNames.Contains(x.UserName.ToLower())).Select(x => x.CarIdx).ToArray();
            }
            else
                preferredCarIndexes = sessionData.DriverInfo.Drivers.Where(x => !x.IsPaceCar).Select(x => x.CarIdx).ToArray();

            randomDriverNumber = new Random();
        }

        public bool IsActive(DataSample data)
        {
            if (isWatchingRandomDriver && data.Telemetry.SessionTimeSpan < finishWatchingRandomDriverAt)
                return true;

            isWatchingRandomDriver = false;
            return false;
        }

        public void Direct(DataSample data)
        {
            if (isWatchingRandomDriver)
                return;

            isWatchingRandomDriver = true;

            finishWatchingRandomDriverAt = data.Telemetry.SessionTimeSpan + stickyTime;

            camera = cameraControl.FindACamera(CameraAngle.LookingInfrontOfCar, CameraAngle.LookingAtCar, CameraAngle.LookingAtTrack);
            car = FindADriver(data);

            TraceInfo.WriteLine("{0} Changing camera to random driver: {1}; camera: {2}", data.Telemetry.SessionTimeSpan, car.UserName, camera.CameraName);
            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        public void Redirect(DataSample data)
        {
            TraceInfo.WriteLine("{0} Changing camera back to driver: {1}; camera: {2}", data.Telemetry.SessionTimeSpan, car.UserName, camera.CameraName);
            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        SessionData._DriverInfo._Drivers FindADriver(DataSample data)
        {
            var activeDrivers = preferredCarIndexes
                .Select(carIdx => data.Telemetry.Cars[carIdx])
                .Where(c => c.HasData && c.TrackSurface != TrackLocation.InPitStall)
                .Select(c => c.CarIdx)
                .ToList();

            var next = randomDriverNumber.Next(activeDrivers.Count);

            return sessionData.DriverInfo.Drivers[activeDrivers[next]];
        }

        public string Name
        {
            get { return GetType().Name; }
        }
    }
}
