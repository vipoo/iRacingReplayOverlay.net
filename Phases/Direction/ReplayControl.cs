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

using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class ReplayControl
    {
        class CameraDetails
        {
            public double SessionTime;
            public short CarNumber;
            public short CameraGroupNumber;
            public bool isOverride = false;
            public string Reason;
        }

        SessionData sessionData;
        List<CameraDetails> directions = new List<CameraDetails>();
        double lastSessionTime;
        List<CameraDetails>.Enumerator nextCamera;
        bool isMoreCameraChanges;

        public ReplayControl(SessionData sessionData)
        {
            this.sessionData = sessionData;
        }

        public void AddCarChange(double sessionTime, int carIdx, string cameraGroupName, string reason)
        {
            cameraGroupName = cameraGroupName.ToLower();

            var cameraGroup = sessionData.CameraInfo.Groups.First(g => g.GroupName.ToLower() == cameraGroupName);
            var car = sessionData.DriverInfo.Drivers.First(d => d.CarIdx == carIdx);

            directions.Add(new CameraDetails { SessionTime = sessionTime, CameraGroupNumber = (short)cameraGroup.GroupNum, CarNumber = (short)car.CarNumber,
            Reason = reason});
        }

        public void AddShortCarChange(double startTime, double endTime, int carIdx, string cameraGroupName, string reason)
        {
            var cameraGroup = sessionData.CameraInfo.Groups.First(g => g.GroupName == cameraGroupName);
            var car = sessionData.DriverInfo.Drivers.First(d => d.CarIdx == carIdx);

            var directionJustBefore = directions.Where(d => !d.isOverride).OrderByDescending(d => d.SessionTime).First(d => d.SessionTime <= endTime);

            directions.RemoveAll(d => !d.isOverride && d.SessionTime >= startTime && d.SessionTime <= endTime);

            directions.Add(new CameraDetails { SessionTime = startTime, CarNumber = (short)car.CarNumber, CameraGroupNumber = (short)cameraGroup.GroupNum, isOverride = true, Reason = reason });
            directions.Add(new CameraDetails { SessionTime = endTime, CarNumber = directionJustBefore.CarNumber, CameraGroupNumber = directionJustBefore.CameraGroupNumber, isOverride = true, Reason = directionJustBefore.Reason });
        }

        public void Start()
        {
            lastSessionTime = -1;

            directions = directions.OrderBy(d => d.SessionTime).ToList();
            nextCamera = directions.GetEnumerator();
            isMoreCameraChanges = nextCamera.MoveNext();
        }

        public void Process(DataSample data)
        {
            if (lastSessionTime == data.Telemetry.SessionTime)
                return;

            if (!isMoreCameraChanges)
                return;

            lastSessionTime = data.Telemetry.SessionTime;

            if (lastSessionTime < nextCamera.Current.SessionTime)
                return;

            var cameraDetails = nextCamera.Current;

            Trace.WriteLine("{0} - Changing camera to driver number {1}, using camera number {2} because {3}".F(TimeSpan.FromSeconds(lastSessionTime), cameraDetails.CarNumber, cameraDetails.CameraGroupNumber, cameraDetails.Reason));
            iRacing.Replay.CameraOnDriver(cameraDetails.CarNumber, cameraDetails.CameraGroupNumber);

            isMoreCameraChanges = nextCamera.MoveNext();
        }
    }
}
