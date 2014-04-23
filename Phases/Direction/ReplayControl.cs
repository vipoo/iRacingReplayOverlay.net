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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class ReplayControl
    {
        class CameraDetails
        {
            public int FrameNumber;
            public short CarNumber;
            public short CameraGroupNumber;
        }

        SessionData sessionData;
        List<CameraDetails> directions = new List<CameraDetails>();
        int lastFrameNumber;
        List<CameraDetails>.Enumerator nextCamera;
        bool isMoreCameraChanges;

        public ReplayControl(SessionData sessionData)
        {
            this.sessionData = sessionData;
        }

        public void AddCarChange(int frameNumber, int carIdx, string cameraGroupName)
        {
            var cameraGroup = sessionData.CameraInfo.Groups.First(g => g.GroupName == cameraGroupName);
            var car = sessionData.DriverInfo.Drivers.First(d => d.CarIdx == carIdx);

            directions.Add(new CameraDetails { FrameNumber = frameNumber, CameraGroupNumber = (short)cameraGroup.GroupNum, CarNumber = (short)car.CarNumber });
        }

        public void Start()
        {
            nextCamera = directions.GetEnumerator();
            lastFrameNumber = -1;
            isMoreCameraChanges = nextCamera.MoveNext();
        }

        public void Process(DataSample data)
        {
            if (lastFrameNumber == data.Telemetry.ReplayFrameNum)
                return;

            if (!isMoreCameraChanges)
                return;

            lastFrameNumber = data.Telemetry.ReplayFrameNum;

            if (lastFrameNumber < nextCamera.Current.FrameNumber)
                return;

            var cameraDetails = nextCamera.Current;
            Trace.WriteLine("Changing camera to driver number {0}, using camera number {1}".F(cameraDetails.CarNumber, cameraDetails.CameraGroupNumber));
            iRacing.Replay.CameraOnDriver(cameraDetails.CarNumber, cameraDetails.CameraGroupNumber);

            isMoreCameraChanges = nextCamera.MoveNext();
        }
    }
}
