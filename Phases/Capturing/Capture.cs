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
    public class Capture
    {
        readonly String workingFolder;
        readonly OverlayData overlayData;
        readonly CommentaryMessages commentaryMessages;
        readonly RemovalEdits removalEdits;

        TimeSpan lastTime;
        OverlayData.TimingSample timingSample;
        OverlayData.Driver[] lastDrivers;
        int leaderBoardUpdateRate = 0;
        double raceStartTimeOffset = 0;

        public Capture(OverlayData overlayData, CommentaryMessages commentaryMessages, RemovalEdits removalEdits, string workingFolder)
        {
            this.overlayData = overlayData;
            this.workingFolder = workingFolder;
            this.commentaryMessages = commentaryMessages;
            this.removalEdits = removalEdits;
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            if (data.Telemetry.SessionTimeSpan.Subtract( lastTime).TotalSeconds < 0.5)
                return;

            lastTime = data.Telemetry.SessionTimeSpan;

            if (ProcessForLastLap(data, relativeTime))
                return;

            if (leaderBoardUpdateRate <= 8 && timingSample != null)
            {
                timingSample = CreateTimingSample(data, relativeTime, timingSample.Drivers);
                leaderBoardUpdateRate++;
            }
            else
            {
                leaderBoardUpdateRate = 0;
                ProcessLatestRunningOrder(data, relativeTime);
            }

            overlayData.TimingSamples.Add(timingSample);
        }

        void ProcessLatestRunningOrder(DataSample data, TimeSpan relativeTime)
        {
            var drivers = data.Telemetry.Cars.Where(c => !c.IsPaceCar ).Select(c => new OverlayData.Driver
            {
                Name = c.UserName,
                CarNumber = c.CarNumber,
                Position = c.Position,
                CarIdx = c.CarIdx
            })
            .OrderBy( c => c.Position)
            .ToArray();

            timingSample = CreateTimingSample(data, relativeTime, drivers);

            if (lastDrivers != null)
                foreach (var d in drivers.OrderBy(d => d.Position))
                {
                    var lastPosition = lastDrivers.FirstOrDefault(lp => lp.CarIdx == d.CarIdx);
                    if (lastPosition != null && lastPosition.Position != d.Position)
                    {
                        removalEdits.InterestingThingHappend(data);
                        var msg = "{0} in {1}{2}".F(d.Name, d.Position, d.Position.Ordinal());
                        Trace.WriteLine("{0} {1}".F(data.Telemetry.SessionTimeSpan, msg), "INFO");
                        commentaryMessages.Add(msg, relativeTime.TotalSeconds);
                    }
                }

            lastDrivers = drivers;
        }

        OverlayData.TimingSample CreateTimingSample(DataSample data, TimeSpan relativeTime, OverlayData.Driver[] drivers)
        {
            if( raceStartTimeOffset == 0 && data.Telemetry.SessionState == SessionState.Racing)
                raceStartTimeOffset = data.Telemetry.SessionTime;

            var session = data.SessionData.SessionInfo.Sessions.First(s => s.SessionNum == data.Telemetry.SessionNum);

            var timespan = raceStartTimeOffset == 0 ? TimeSpan.FromSeconds(session._SessionTime) : TimeSpan.FromSeconds(data.Telemetry.SessionTimeRemain + raceStartTimeOffset);
            var raceLapsPosition = string.Format("Lap {0}/{1}", data.Telemetry.RaceLaps, session.ResultsLapsComplete);
            var raceTimePosition = (timespan.TotalSeconds <= 0 ? TimeSpan.FromSeconds(0) : timespan).ToString(@"mm\:ss");
            var raceLapCounter = string.Format("Lap {0}", data.Telemetry.RaceLaps);

            if( data.Telemetry.RaceLaps <= 0)
            {
                raceLapCounter = null;
                raceLapsPosition = "";
            } 
            else
                if (data.Telemetry.RaceLaps < session.ResultsLapsComplete)
                {

                }
                else
                    if (data.Telemetry.RaceLaps == session.ResultsLapsComplete)
                    {
                        raceLapsPosition = "Final Lap";
                        raceLapCounter = "Final Lap";
                    }
                    else
                    {
                        raceLapsPosition = "Results";
                        raceLapCounter = "Results";
                    }

            return new OverlayData.TimingSample
            {
                StartTime = relativeTime.TotalSeconds,
                Drivers = drivers,
                RacePosition = session.IsLimitedSessionLaps ? raceLapsPosition : raceTimePosition,
                CurrentDriver = GetCurrentDriverDetails(data, drivers),
                LapCounter = session.IsLimitedSessionLaps ? null : raceLapCounter
            };
        }

        bool[] haveNotedCheckerdFlag = new bool[64];

        bool ProcessForLastLap(DataSample data, TimeSpan relativeTime)
        {
            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            if (!data.Telemetry.LeaderHasFinished)
                return false;

            if (data.LastSample == null)
                return false;

            removalEdits.InterestingThingHappend(data);

            timingSample = CreateTimingSample(data, relativeTime, timingSample.Drivers);

            for (int i = 1; i < data.SessionData.DriverInfo.Drivers.Length; i++)
            {
                if (data.LastSample.Telemetry.Cars[i].HasSeenCheckeredFlag && !haveNotedCheckerdFlag[i])
                {
                    haveNotedCheckerdFlag[i] = true;

                    var driver = data.SessionData.DriverInfo.Drivers[i];
                    var position = (int)session.ResultsPositions.First(r => r.CarIdx == i).Position;

                    var drivers = timingSample.Drivers.Where(d => d.CarIdx != i)
                        .Select(d => d.Clone())
                        .ToList();

                    drivers.Insert((int)position-1, new OverlayData.Driver 
                    {
                        CarNumber = (int)driver.CarNumber,
                        Name = driver.UserName,
                        Position = position,
                        CarIdx = i
                    });

                    var p = 1;
                    foreach( var d in drivers)
                        d.Position = p++;

                    timingSample = CreateTimingSample(data, relativeTime, drivers.ToArray());

                    var msg = string.Format("{0} {1} finished in {2}{3}", data.Telemetry.SessionTimeSpan, driver.UserName, position, position.Ordinal());
                    Trace.WriteLine("{0} {1}".F(data.Telemetry.SessionTimeSpan, msg), "INFO");
                    commentaryMessages.Add(msg, relativeTime.TotalSeconds);
                }
            }

            overlayData.TimingSamples.Add(timingSample);

            return true;
        }

        static OverlayData.Driver GetCurrentDriverDetails(DataSample data, OverlayData.Driver[] drivers)
        {
            var car = data.Telemetry.CamCar;
            if (car == null)
                return null;

            var driver = new OverlayData.Driver
            {
                CarIdx = car.CarIdx,
                CarNumber = car.CarNumber,
                Indicator = car.Position.Ordinal(),
                Name = car.UserName,
                Position = car.Position
            };

            return driver;
        }
    }
}
