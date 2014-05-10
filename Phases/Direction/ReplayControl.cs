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
using iRacingReplayOverlay;
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
        readonly RemovalEdits removalEdits;

        double lastTimeStamp = 0;
        bool isShowingIncident;

        public ReplayControl(SessionData sessionData, Incidents incidents, CommentaryMessages commentaryMessages, RemovalEdits removalEdits)
        {
            this.sessionData = sessionData;
            this.commentaryMessages = commentaryMessages;
            this.removalEdits = removalEdits;

            random = new System.Random();
            randomDriverNumber = new Random();

            trackCameras = Settings.Default.trackCameras.Where(tc => tc.TrackName == sessionData.WeekendInfo.TrackDisplayName).ToArray();

            Trace.WriteLineIf(trackCameras.Count() <= 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName), "INFO");
            Debug.Assert(trackCameras.Count() > 0, "Track Cameras not defined for {0}".F(sessionData.WeekendInfo.TrackDisplayName));

            foreach (var tc in trackCameras)
                tc.CameraNumber = (short)sessionData.CameraInfo.Groups.First(g => g.GroupName.ToLower() == tc.CameraName.ToLower()).GroupNum;

            TV2 = trackCameras.First(tc => tc.CameraName == "TV2");
            TV3 = trackCameras.First(tc => tc.CameraName == "TV3");

            iRacing.Replay.CameraOnPositon(1, TV3.CameraNumber);

            nextIncident = incidents.GetEnumerator();
            nextIncident.MoveNext();
        }

        double incidentPitBoxStartTime = 0;

        bool IsShowingIncident(DataSample data)
        {
            if( !(isShowingIncident && nextIncident.Current.EndSessionTime >= data.Telemetry.SessionTime))
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

        private bool IsFinishedShowingIncident(DataSample data)
        {
            if (!isShowingIncident)
                return false;

            removalEdits.InterestingThingHappend(data);
            Trace.WriteLine("Finishing incident from {0}".F(TimeSpan.FromSeconds(nextIncident.Current.StartSessionTime)), "INFO");

            isShowingIncident = false;
            nextIncident.MoveNext();
            return true;
        }

		public enum ViewType
		{
			Incident,
			CloseBattle,
			RandomCar,
			FirstLap,
			LastLap
		}

		ViewType currentlyViewing;
        public bool Process(DataSample data)
        {
			if(OnLastLap(data))
			{
				currentlyViewing = ViewType.LastLap;
				return SwitchToFinishingDrivers(data);
			}

			if(IsBeforeFirstLapSector2(data))
			{
				currentlyViewing = ViewType.FirstLap;
				return false;
			}

            if (IsShowingIncident(data))
			{
				currentlyViewing = ViewType.Incident;
                return false;
			}
            
            var finishedShowingIncident = IsFinishedShowingIncident(data);

            SkipOverlappingIncidents(data);

            if (SwitchToIncident(data))
			{
				currentlyViewing = ViewType.Incident;
                return false;
			}

            TrackCamera camera;
            SessionData._DriverInfo._Drivers car;

            /*if( !data.Telemetry.CamCar.HasData)
            {
                camera = FindACamera();
                car = FindARandomDriver(data);
                Trace.WriteLine("{0} Changing camera to random driver number {1}, using camera number {2} as previous car has drop out".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
                iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
                return false;
            }*/

			if(!finishedShowingIncident && !TwentySecondsAfterLastCameraChange(data))
			{
				if( currentlyViewing != ViewType.RandomCar)
					removalEdits.InterestingThingHappend(data);
				return false;
			}

            lastTimeStamp = data.Telemetry.SessionTime;

            car = FindCarWithin1Second(data);
            camera = FindACamera();
            car = ChangeCarForCamera(data, camera, car);
            if (car != null)
            {
				currentlyViewing= ViewType.CloseBattle;
                removalEdits.InterestingThingHappend(data);
                Trace.WriteLine("{0} Changing camera to driver number {1}, using camera number {2} - within 1 second".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
            }
            else
            {
				currentlyViewing = ViewType.RandomCar;
                car = FindARandomDriver(data);
                Trace.WriteLine("{0} Changing camera to random driver number {1}, using camera number {2}".F(TimeSpan.FromSeconds(lastTimeStamp), car.CarNumber, camera.CameraName), "INFO");
            }

            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);

            return false;
        }

        private bool SwitchToIncident(DataSample data)
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

        private void SkipOverlappingIncidents(DataSample data)
        {
            while (nextIncident.Current != null && nextIncident.Current.StartSessionTime + 1 < data.Telemetry.SessionTime)
            {
                Trace.WriteLine("Skipping incident at time {0}".F(TimeSpan.FromSeconds(nextIncident.Current.StartSessionTime)), "INFO");
                nextIncident.MoveNext();
            }
        }

        DateTime timeOfFinisher = DateTime.Now;
        int lastFinisherCarIdx = -1;

        private bool SwitchToFinishingDrivers(DataSample data)
        {
            removalEdits.InterestingThingHappend(data);

            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            if (lastFinisherCarIdx != -1 && !data.Telemetry.Cars[lastFinisherCarIdx].HasSeenCheckeredFlag)
            {
                timeOfFinisher = DateTime.Now.AddSeconds(2);
                return false;
            }

            if (timeOfFinisher > DateTime.Now)
                return false;

            Car nextFinisher;

            if( !data.Telemetry.LeaderHasFinished)
                nextFinisher = data.Telemetry.Cars.First( c=> c.Position == 1);
            else
                nextFinisher = data.Telemetry.Cars
                    .Where(c => c.TotalDistance > 0)
                    .Where( c=> !c.HasSeenCheckeredFlag)
                    .Where( c => !c.IsPaceCar)
                    .Where( c => c.HasData)
                    .OrderByDescending( c=> c.DistancePercentage)
                    .FirstOrDefault();

            if (nextFinisher == null)
                return true;

            Trace.WriteLine("{0} Found {1} in position {2}".F(data.Telemetry.SessionTimeSpan, nextFinisher.UserName, nextFinisher.Position), "DEBUG");

            timeOfFinisher = DateTime.Now;
            lastFinisherCarIdx = nextFinisher.CarIdx;

            Trace.WriteLine("{0} Switching camera to {1} as they cross finishing line in position {2}".F(data.Telemetry.SessionTimeSpan, nextFinisher.UserName, nextFinisher.Position), "INFO");

            iRacing.Replay.CameraOnDriver(nextFinisher.CarNumber, TV2.CameraNumber);
            
            return false;
        }

        bool TwentySecondsAfterLastCameraChange(DataSample data)
        {
            return lastTimeStamp + 20.0 <= data.Telemetry.SessionTime;
        }

        DateTime lastTimeLeaderWasSelected = DateTime.Now;

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

        SessionData._DriverInfo._Drivers ChangeCarForCamera(DataSample data, TrackCamera camera, SessionData._DriverInfo._Drivers driver)
        {
            if (driver == null)
                return null;

            var car = data.Telemetry.Cars[driver.CarIdx];

            if (camera.CameraName == "Gearbox" )
            {
                Trace.WriteLine("Changing to forward car, with reverse camera");
                car =  data.Telemetry.Cars.First(c => c.Position == car.Position - 1);
                return data.SessionData.DriverInfo.Drivers[car.CarIdx];
            }
                   

            return driver;
        }
    }
}
