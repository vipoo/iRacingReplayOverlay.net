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
using IRacingReplayOverlay;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class ReplayControl
    {
        readonly SessionData sessionData;
        readonly Random random;
        readonly TrackCamera[] trackCameras;
        readonly TrackCamera TV2;
        readonly TrackCamera TV3;
        readonly Random randomDriverNumber;
        readonly IEnumerator<Incidents.Incident> nextIncident;
        readonly CommentaryMessages commentaryMessages;

        double lastTimeStamp = 0;
        bool isShowingIncident;

        public ReplayControl(SessionData sessionData, Incidents incidents, CommentaryMessages commentaryMessages)
        {
            this.sessionData = sessionData;
            this.commentaryMessages = commentaryMessages;

            random = new System.Random();
            randomDriverNumber = new Random();

            trackCameras = Settings.Default.trackCameras.Where(tc => tc.TrackName == sessionData.WeekendInfo.TrackDisplayName).ToArray();

            Trace.WriteLineIf(trackCameras.Count() > 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName), "INFO");
            Debug.Assert(trackCameras.Count() > 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName));

            foreach (var tc in trackCameras)
                tc.CameraNumber = (short)sessionData.CameraInfo.Groups.First(g => g.GroupName.ToLower() == tc.CameraName.ToLower()).GroupNum;

            TV2 = trackCameras.First(tc => tc.CameraName == "TV2");
            TV3 = trackCameras.First(tc => tc.CameraName == "TV3");

            iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);

            nextIncident = incidents.GetEnumerator();
            nextIncident.MoveNext();
        }

        public bool Process(DataSample data)
        {
            if( OnLastLap(data) )
                return SwitchToFinishingDrivers(data);

            if (isShowingIncident)
            {
                if (nextIncident.Current.EndSessionTime >= data.Telemetry.SessionTime)
                    return false;

                Trace.WriteLine("Finishing incident from {0}".F(nextIncident.Current.StartSessionTime), "INFO");
   
                isShowingIncident = false;
                nextIncident.MoveNext();
            }

            if (IsBeforeFirstLapSector2(data))
                return false;

            while (nextIncident.Current != null && nextIncident.Current.StartSessionTime + 1 < data.Telemetry.SessionTime)
            {
                Trace.WriteLine("Skipping incident at time {0}".F(TimeSpan.FromSeconds(nextIncident.Current.StartSessionTime)), "INFO");
                nextIncident.MoveNext();
            }

            if( nextIncident.Current != null && (nextIncident.Current.StartSessionTime) < data.Telemetry.SessionTime)
            {
                isShowingIncident = true;

                var incidentCar = sessionData.DriverInfo.Drivers[nextIncident.Current.CarIdx];

                iRacing.Replay.CameraOnDriver((short)incidentCar.CarNumber, TV2.CameraNumber);
                return false;
            }

            TrackCamera camera;
            SessionData._DriverInfo._Drivers car;

            /*if( !data.Telemetry.CamCar.HasData)
            {
                camera = FindACamera();
                car = FindARandomDriver(data);
                Trace.WriteLine("{0} - Changing camera to random driver number {1}, using camera number {2} as previous car has drop out".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
                iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
                return false;
            }*/

            if (TwentySecondsAfterLastCameraChange(data))
                return false;

            lastTimeStamp = data.Telemetry.SessionTime;

            camera = FindACamera();

            car = FindCarWithin1Second(data);
            if (car != null)
            {
                Trace.WriteLine("{0} - Changing camera to driver number {1}, using camera number {2} - within 1 second".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
            }
            else
            {
                car = FindARandomDriver(data);
                Trace.WriteLine("{0} - Changing camera to random driver number {1}, using camera number {2}".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
            }

            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);

            return false;
        }

        double timeOfFinisher = 0;
        int lastFinisherCarIdx = -1;

        private bool SwitchToFinishingDrivers(DataSample data)
        {
            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            if (lastFinisherCarIdx != -1 && !data.Telemetry.Cars[lastFinisherCarIdx].HasSeenCheckeredFlag)
            {
                timeOfFinisher = data.Telemetry.SessionTime + 2;
                return false;
            }

            if (timeOfFinisher > data.Telemetry.SessionTime)
                return false;

            Car nextFinisher;

            if( !data.Telemetry.LeaderHasFinished)
                nextFinisher = data.Telemetry.Cars.First( c=> c.Position == 1);
            else
                nextFinisher = data.Telemetry.Cars
                    .Where(c => c.TotalDistance > 0)
                    .Where( c=> !c.HasSeenCheckeredFlag)
                    .Where( c => !c.IsPaceCar)
                    .OrderByDescending( c=> c.DistancePercentage)
                    .FirstOrDefault();

            if (nextFinisher == null)
                return true;

            Trace.WriteLine("Found {0} in position {1}".F(nextFinisher.UserName, nextFinisher.Position));

            timeOfFinisher = data.Telemetry.SessionTime;
            lastFinisherCarIdx = nextFinisher.CarIdx;

            Trace.WriteLine("Switching camera to {0} as they cross finishing line in position {1}.".F(nextFinisher.UserName, nextFinisher.Position));

            iRacing.Replay.CameraOnDriver(nextFinisher.CarNumber, TV2.CameraNumber);
            
            return false;
        }

        bool TwentySecondsAfterLastCameraChange(DataSample data)
        {
            return lastTimeStamp + 20.0 > data.Telemetry.SessionTime;
        }

        DateTime lastTimeLeaderWasSelected = DateTime.Now;

        bool IsBeforeFirstLapSector2(DataSample data)
        {
            var result = data.Telemetry.RaceLapSector.LapNumber < 1 || (data.Telemetry.RaceLapSector.LapNumber == 1 && data.Telemetry.RaceLapSector.Sector < 2);
            if (result)
            {
                if ((DateTime.Now - lastTimeLeaderWasSelected).TotalSeconds > 5)
                {
                    iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);

                    lastTimeLeaderWasSelected = DateTime.Now;
                }
            }

            return result;
        }

        bool OnLastLap(DataSample data)
        {
            var totalLaps = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum].ResultsLapsComplete;
            return data.Telemetry.RaceLapSector >= new LapSector((int)totalLaps, 1);
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

        SessionData._DriverInfo._Drivers FindCarWithin1Second(DataSample data)
        {
            var distances = data.Telemetry.CarIdxDistance
                .Select((d, i) => new { CarIdx = i, Distance = d })
                .Skip(1)
                .Where(d => d.Distance > 0)
                .OrderByDescending(d => d.Distance)
                .ToList();

            var gap = Enumerable.Range(1, distances.Count - 1)
                .Select(i => new
                {
                    CarIdx = distances[i].CarIdx,
                    Distance = distances[i - 1].Distance - distances[i].Distance,
                    Position = i
                });
            
            var timeGap = gap.Select(g => new
                {
                    CarIdx = g.CarIdx,
                    Time = g.Distance * data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum].ResultsAverageLapTime,
                    Position = g.Position
                })
                .Where( d => d.Time <= 1)
                .OrderBy(d => d.Position);

            var closest = timeGap.FirstOrDefault();

            if (closest != null)
                return sessionData.DriverInfo.Drivers[closest.CarIdx];

            return null;
        }

        TrackCamera FindACamera()
        {
            var rand = random.Next(100);
            var offset = 0;
            var camera = TV2;

            foreach (var tc in trackCameras)
            {
                if (rand < tc.Ratio + offset)
                {
                    camera = tc;
                    break;
                }
                offset += tc.Ratio;
            }
            return camera;
        }
    }
}
