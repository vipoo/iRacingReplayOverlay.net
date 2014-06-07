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
        public enum ViewType
        {
            Incident,
            CloseBattle,
            RandomCar,
            FirstLap,
            LastLap
        }

        readonly SessionData sessionData;
        readonly Random random;
        readonly TrackCamera[] cameras;
        readonly TrackCamera TV2;
        readonly TrackCamera TV3;
        readonly Random randomDriverNumber;
        readonly Random randomPreferredDriver;
        readonly IEnumerator<Incidents.Incident> nextIncident;
        readonly CommentaryMessages commentaryMessages;
        readonly RemovalEdits removalEdits;
        readonly IList<SessionData._DriverInfo._Drivers> preferredCars;
        readonly double maxTimeForInterestingEvent;
        readonly double maxTimeBetweenCameraChanges;
        readonly List<CameraAngle> forwardCameraAngles;
        readonly List<CameraAngle> battleCameraAngles;
        readonly List<CameraAngle> normalCameraAngles;

        ViewType currentlyViewing;
        double lastTimeStamp = 0;
        bool isShowingIncident;
        double incidentPitBoxStartTime = 0;
        DateTime timeOfFinisher = DateTime.Now;
        int lastFinisherCarIdx = -1;
        DateTime lastTimeLeaderWasSelected = DateTime.Now;
        long previousCarIdx = -1;
        long previousCarPosition = -1;

        public ReplayControl(SessionData sessionData, Incidents incidents, CommentaryMessages commentaryMessages, RemovalEdits removalEdits, TrackCameras trackCameras)
        {
            this.sessionData = sessionData;
            this.commentaryMessages = commentaryMessages;
            this.removalEdits = removalEdits;

            random = new System.Random();
            randomDriverNumber = new Random();
            randomPreferredDriver = new Random();

            forwardCameraAngles = new List<CameraAngle>
            {
                CameraAngle.LookingInfrontOfCar, 
            };
            battleCameraAngles = new List<CameraAngle>
            {
                CameraAngle.LookingInfrontOfCar, 
                CameraAngle.LookingBehindCar,
                CameraAngle.LookingAtCar
            };
            normalCameraAngles = new List<CameraAngle>
            {
                CameraAngle.LookingInfrontOfCar, 
                CameraAngle.LookingAtCar,
                CameraAngle.LookingAtTrack
            };

            IEnumerable<string> preferredDriverNames = Settings.Default.PreferredDriverNames.Split(new char[] { ',', ';' }).Select(name => name.Trim());
            preferredCars = sessionData.DriverInfo.Drivers.Where(x => preferredDriverNames.Contains(x.UserName)).ToList();
            maxTimeForInterestingEvent = Settings.Default.MaxTimeForInterestingEvent.TotalSeconds;
            maxTimeBetweenCameraChanges = Settings.Default.MaxTimeBetweenCameraChanges.TotalSeconds;

            cameras = trackCameras.Where(tc => tc.TrackName == sessionData.WeekendInfo.TrackDisplayName).ToArray();

            Trace.WriteLineIf(cameras.Count() <= 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName), "INFO");
            Debug.Assert(cameras.Count() > 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName));

            foreach (var tc in cameras)
                tc.CameraNumber = (short)sessionData.CameraInfo.Groups.First(g => g.GroupName.ToLower() == tc.CameraName.ToLower()).GroupNum;

            TV2 = cameras.First(tc => tc.CameraName == "TV2");
            TV3 = cameras.First(tc => tc.CameraName == "TV3");

            iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);

            nextIncident = incidents.GetEnumerator();
            nextIncident.MoveNext();
        }

        public void Process(DataSample data)
        {
            if (OnLastLap(data))
            {
                currentlyViewing = ViewType.LastLap;
                SwitchToFinishingDrivers(data);
                return;
            }

            if (IsBeforeFirstLapSector2(data))
            {
                currentlyViewing = ViewType.FirstLap;
                return;
            }

            if (IsShowingIncident(data))
            {
                currentlyViewing = ViewType.Incident;
                return;
            }

            var finishedShowingIncident = IsFinishedShowingIncident(data);

            SkipOverlappingIncidents(data);

            if (SwitchToIncident(data))
            {
                currentlyViewing = ViewType.Incident;
                return;
            }

            TrackCamera camera;
            SessionData._DriverInfo._Drivers car;
            bool currentCarOvertake = false;

            // Find car of previous iteration
            Car previousCar = data.Telemetry.Cars.FirstOrDefault(x => x.CarIdx == previousCarIdx);
            if (previousCar != null)
            {
                // Detect if car has been overtaken
                long currentPositionOfPreviousCar = data.Telemetry.Cars.FirstOrDefault(x => x.CarIdx == previousCarIdx).Position;
                if (currentPositionOfPreviousCar > previousCarPosition) currentCarOvertake = true;
            }

            // Continue only if incident is finished or camera has not changed for 20s or car has been overtaken
            if (!finishedShowingIncident && !CameraChanged(data) && !currentCarOvertake)
            {
                if (currentlyViewing != ViewType.RandomCar)
                    removalEdits.InterestingThingHappend(data);
                return;
            }

            lastTimeStamp = data.Telemetry.SessionTime;

            car = FindCarWithinRange(data, maxTimeForInterestingEvent);
            if (car != null)
            {
                currentlyViewing = ViewType.CloseBattle;

                // In a battle use only battle cameraAngles
                camera = FindACamera(battleCameraAngles);
                car = ChangeCarForCamera(data, camera, car);
                removalEdits.InterestingThingHappend(data);
                Trace.WriteLine("{0} Changing camera to driver number {1}, using camera {2} - within {3} seconds".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName, maxTimeForInterestingEvent), "INFO");
            }
            else
            {
                currentlyViewing = ViewType.RandomCar;

                if (preferredCars.Count() == 0)
                {
                    car = FindARandomDriver(data);
                    camera = FindACamera(normalCameraAngles);
                    Trace.WriteLine("{0} Changing camera to random driver number {1}, using camera {2}".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
                }
                else
                {
                    car = FindAPreferredDriver();
                    camera = FindACamera(normalCameraAngles);
                    Trace.WriteLine("{0} Changing camera to preferred driver number {1}, using camera {2}".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
                }
            }

            long currentCarPosition = data.Telemetry.Cars.FirstOrDefault(x => x.CarIdx == car.CarIdx).Position;

            if (car.CarIdx == previousCarIdx)
            {
                if (currentCarPosition > previousCarPosition)
                {
                    // After overtake switch to forward cameraAngles
                    camera = FindACamera(forwardCameraAngles);
                    Trace.WriteLine("{0} Driver number {1} has been passed, switching to forward camera {2}".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
                }
            }

            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);

            previousCarIdx = car.CarIdx;
            previousCarPosition = currentCarPosition;
        }

        bool IsShowingIncident(DataSample data)
        {
            if (!(isShowingIncident && nextIncident.Current.EndSessionTime >= data.Telemetry.SessionTime))
                return false;

            if (data.Telemetry.CamCar.TrackSurface == TrackLocation.InPitStall && incidentPitBoxStartTime == 0)
            {
                Trace.WriteLine("Incident car is in pit stall {0}".F(TimeSpan.FromSeconds(data.Telemetry.SessionTime)));
                incidentPitBoxStartTime = data.Telemetry.SessionTime;
            }

            if (data.Telemetry.CamCar.TrackSurface == TrackLocation.InPitStall && incidentPitBoxStartTime + 2 < data.Telemetry.SessionTime)
            {
                Trace.WriteLine("Finishing showing incident as car is in pit stall {0}".F(TimeSpan.FromSeconds(data.Telemetry.SessionTime)));
                return false;
            }

            removalEdits.InterestingThingHappend(data);

            return true;
        }

        bool IsFinishedShowingIncident(DataSample data)
        {
            if (!isShowingIncident)
                return false;

            removalEdits.InterestingThingHappend(data);
            Trace.WriteLine("Finishing incident from {0}".F(TimeSpan.FromSeconds(nextIncident.Current.StartSessionTime)), "INFO");

            isShowingIncident = false;
            nextIncident.MoveNext();
            return true;
        }

        bool SwitchToIncident(DataSample data)
        {
            if (nextIncident.Current != null && (nextIncident.Current.StartSessionTime) < data.Telemetry.SessionTime)
            {
                isShowingIncident = true;
                incidentPitBoxStartTime = 0;

                var incidentCar = sessionData.DriverInfo.Drivers[nextIncident.Current.CarIdx];

                Trace.WriteLine("{0} Showing incident with {1}".F(data.Telemetry.SessionTimeSpan, incidentCar.UserName), "INFO");

                removalEdits.InterestingThingHappend(data);
                iRacing.Replay.CameraOnDriver((short)incidentCar.CarNumber, TV2.CameraNumber);
                return true;
            }

            return false;
        }

        void SkipOverlappingIncidents(DataSample data)
        {
            while (nextIncident.Current != null && nextIncident.Current.StartSessionTime + 1 < data.Telemetry.SessionTime)
            {
                Trace.WriteLine("Skipping incident at time {0}".F(TimeSpan.FromSeconds(nextIncident.Current.StartSessionTime)), "INFO");
                nextIncident.MoveNext();
            }
        }

        void SwitchToFinishingDrivers(DataSample data)
        {
            removalEdits.InterestingThingHappend(data);

            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            if (lastFinisherCarIdx != -1 && !data.Telemetry.Cars[lastFinisherCarIdx].HasSeenCheckeredFlag)
            {
                timeOfFinisher = DateTime.Now.AddSeconds(2);
                return;
            }

            if (timeOfFinisher > DateTime.Now)
                return;

            Car nextFinisher;

            if (!data.Telemetry.LeaderHasFinished)
                nextFinisher = data.Telemetry.Cars.First(c => c.Position == 1);
            else
                nextFinisher = data.Telemetry.Cars
                        .Where(c => c.TotalDistance > 0)
                        .Where(c => !c.HasSeenCheckeredFlag)
                        .Where(c => !c.IsPaceCar)
                        .Where(c => c.HasData)
                        .OrderByDescending(c => c.DistancePercentage)
                        .FirstOrDefault();

            if (nextFinisher == null)
                return;

            Trace.WriteLine("{0} Found {1} in position {2}".F(data.Telemetry.SessionTimeSpan, nextFinisher.UserName, nextFinisher.Position), "DEBUG");

            timeOfFinisher = DateTime.Now;
            lastFinisherCarIdx = nextFinisher.CarIdx;

            Trace.WriteLine("{0} Switching camera to {1} as they cross finishing line in position {2}".F(data.Telemetry.SessionTimeSpan, nextFinisher.UserName, nextFinisher.Position), "INFO");

            iRacing.Replay.CameraOnDriver(nextFinisher.CarNumber, TV2.CameraNumber);

            return;
        }

        bool CameraChanged(DataSample data)
        {
            return lastTimeStamp + maxTimeBetweenCameraChanges <= data.Telemetry.SessionTime;
        }

        bool IsBeforeFirstLapSector2(DataSample data)
        {
            var result = data.Telemetry.RaceLapSector.LapNumber < 1 || (data.Telemetry.RaceLapSector.LapNumber == 1 && data.Telemetry.RaceLapSector.Sector < 2);
            if (result)
            {
                removalEdits.InterestingThingHappend(data);

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

        SessionData._DriverInfo._Drivers FindAPreferredDriver()
        {
            var next = randomPreferredDriver.Next(preferredCars.Count());

            return sessionData.DriverInfo.Drivers[preferredCars[next].CarIdx];
        }

        TrackCamera FindACamera(IList<CameraAngle> cameraAngles)
        {
            var rand = 0;
            var offset = 0;
            var camera = TV2;

            // Filter cameras to take only those having the specified TrackCameraAngles
            IEnumerable<TrackCamera> selectableCameras = cameras.Where(x => cameraAngles.Contains(x.CameraAngle));
            int total = selectableCameras.Sum(x => x.Ratio);

            // If no camera within specified cameraAngles has non zero ratio select among all
            if (total == 0)
            {
                selectableCameras = cameras;
                rand = random.Next(100);
            }
            else
            {
                rand = random.Next(total);
            }

            foreach (var tc in selectableCameras)
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

        static SessionData._DriverInfo._Drivers FindCarWithinRange(DataSample data, double range)
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
                    .Where(d => d.Time <= range)
                    .OrderBy(d => d.Position);

            var closest = timeGap.FirstOrDefault();

            if (closest != null)
                return data.SessionData.DriverInfo.Drivers[closest.CarIdx];

            return null;
        }

        static SessionData._DriverInfo._Drivers ChangeCarForCamera(DataSample data, TrackCamera camera, SessionData._DriverInfo._Drivers driver)
        {
            if (driver == null)
                return null;

            var car = data.Telemetry.Cars[driver.CarIdx];

            if (camera.CameraAngle == CameraAngle.LookingBehindCar)
            {
                Trace.WriteLine("Changing to forward car, with reverse camera");
                car = data.Telemetry.Cars.First(c => c.Position == car.Position - 1);
                return data.SessionData.DriverInfo.Drivers[car.CarIdx];
            }

            return driver;
        }
    }
}
