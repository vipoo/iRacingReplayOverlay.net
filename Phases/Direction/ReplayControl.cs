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

using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Capturing;
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
        readonly SessionData sessionData;
        readonly TrackCamera[] cameras;
        readonly TrackCamera TV2;
        readonly TrackCamera TV3;
        readonly Random randomDriverNumber;
        readonly Random randomPreferredDriver;
        readonly CommentaryMessages commentaryMessages;
        readonly RemovalEdits removalEdits;
        readonly IList<SessionData._DriverInfo._Drivers> preferredCars;
        readonly List<CameraAngle> normalCameraAngles;

        readonly IDirectionRule[] directionRules;
        readonly CameraControl cameraControl;
        private TimeSpan jumptToNextRandomCarAt;

        public ReplayControl(SessionData sessionData, Incidents incidents, CommentaryMessages commentaryMessages, RemovalEdits removalEdits, TrackCameras trackCameras)
        {
            this.sessionData = sessionData;
            this.commentaryMessages = commentaryMessages;
            this.removalEdits = removalEdits;

            randomDriverNumber = new Random();
            randomPreferredDriver = new Random();

            normalCameraAngles = new List<CameraAngle>
            {
                CameraAngle.LookingInfrontOfCar, 
                CameraAngle.LookingAtCar,
                CameraAngle.LookingAtTrack
            };

            IEnumerable<string> preferredDriverNames = Settings.Default.PreferredDriverNames.Split(new char[] { ',', ';' }).Select(name => name.Trim());
            preferredCars = sessionData.DriverInfo.Drivers.Where(x => preferredDriverNames.Contains(x.UserName)).ToList();

            cameras = trackCameras.Where(tc => tc.TrackName == sessionData.WeekendInfo.TrackDisplayName).ToArray();

            Trace.WriteLineIf(cameras.Count() <= 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName), "INFO");
            Debug.Assert(cameras.Count() > 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName));

            foreach (var tc in cameras)
                tc.CameraNumber = (short)sessionData.CameraInfo.Groups.First(g => g.GroupName.ToLower() == tc.CameraName.ToLower()).GroupNum;

            TV2 = cameras.First(tc => tc.CameraName == "TV2");
            TV3 = cameras.First(tc => tc.CameraName == "TV3");

            iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);

            cameraControl = new CameraControl(cameras);

            directionRules = new IDirectionRule[] { 
                new RuleLastSectors(cameras, removalEdits),
                new RuleFirstSectors(cameras, removalEdits),
                new RuleIncident(cameras, removalEdits, incidents),
                new RuleBattle(cameraControl, removalEdits, Settings.Default.MaxTimeBetweenCameraChanges, Settings.Default.MaxTimeForInterestingEvent)
            };

            currentRule = directionRules[0];
        }

        IDirectionRule currentRule;

        public void Process(DataSample data)
        {
            if (ActiveRule(currentRule, data))
                return;

            foreach (var rule in directionRules)
                if (ActiveRule(rule, data))
                    return;

            TrackCamera camera;
            SessionData._DriverInfo._Drivers car;

            if (jumptToNextRandomCarAt > data.Telemetry.SessionTimeSpan)
                return;

            jumptToNextRandomCarAt = data.Telemetry.SessionTimeSpan + Settings.Default.MaxTimeBetweenCameraChanges;

            if (preferredCars.Count() == 0)
            {
                car = FindARandomDriver(data);
                camera = cameraControl.FindACamera(normalCameraAngles);
                Trace.WriteLine("{0} Changing camera to random driver number {1}, using camera {2}".F(data.Telemetry.SessionTimeSpan, car.CarNumber, camera.CameraName), "INFO");
            }
            else
            {
                car = FindAPreferredDriver();
                camera = cameraControl.FindACamera(normalCameraAngles);
                Trace.WriteLine("{0} Changing camera to preferred driver number {1}, using camera {2}".F(data.Telemetry.SessionTimeSpan, car.CarNumber, camera.CameraName), "INFO");
            }

            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        bool ActiveRule(IDirectionRule rule, DataSample data)
        {
            if (rule.IsActive(data))
            {
                currentRule = rule;
                rule.Direct(data);
                return true;
            }

            return false;
        }
        SessionData._DriverInfo._Drivers FindARandomDriver(DataSample data)
        {
            var activeDrivers = data.Telemetry.Cars
                    .Where(c => !c.IsPaceCar)
                    .Where(c => c.HasData)
                    .ToList();

            var next = randomDriverNumber.Next(activeDrivers.Count);

            return sessionData.DriverInfo.Drivers[activeDrivers[next].CarIdx];
        }

        SessionData._DriverInfo._Drivers FindAPreferredDriver()
        {
            var next = randomPreferredDriver.Next(preferredCars.Count());

            return sessionData.DriverInfo.Drivers[preferredCars[next].CarIdx];
        }
    }
}
