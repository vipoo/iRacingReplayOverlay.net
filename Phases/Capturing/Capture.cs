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
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class Capture
    {
        readonly String workingFolder;
        readonly OverlayData overlayData;
        readonly FileSystemWatcher[] fileWatchers;
        readonly CommentaryMessages commentaryMessages;

        string latestCreatedVideoFile;
        DateTime lastTime;
        OverlayData.TimingSample timingSample;
        int[] lastLaps = new int[64];
        OverlayData.Driver[] lastDrivers;
        int leaderBoardUpdateRate = 0;
        
        public Capture(OverlayData overlayData, string workingFolder)
        {
            this.overlayData = overlayData;
            this.workingFolder = workingFolder;

            this.commentaryMessages = new CommentaryMessages();
            latestCreatedVideoFile = null;
            fileWatchers = new FileSystemWatcher[2];
            fileWatchers[0] = new FileSystemWatcher(workingFolder, "*.mp4");
            fileWatchers[1] = new FileSystemWatcher(workingFolder, "*.avi");
            foreach(var fileWatcher in fileWatchers)
            {
                fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime;
                fileWatcher.Created += OnCreated;
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            if ((DateTime.Now - lastTime).TotalSeconds < 0.5)
                return;

            lastTime = DateTime.Now;

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
            var positions = data.Telemetry.Cars
                .Where(c => c.Index != 0)
                .OrderByDescending(c => c.Lap + c.DistancePercentage);

            var drivers = positions.Select((c, p) => new OverlayData.Driver
            {
                Name = c.Driver.UserName,
                CarNumber = (int)c.Driver.CarNumber,
                Position = p + 1,
                CarIdx = (int)c.Driver.CarIdx
            }).ToArray();

            timingSample = CreateTimingSample(data, relativeTime, drivers);

            if (lastDrivers != null)
                foreach (var d in drivers.OrderByDescending(d => d.Position))
                {
                    var lastPosition = lastDrivers.FirstOrDefault(lp => lp.CarIdx == d.CarIdx);
                    if (lastPosition != null && lastPosition.Position != d.Position)
                    {
                        var msg = "Driver {0} now in {1}{2}".F(d.Name, d.Position, GetOrdinal(d.Position));
                        Trace.WriteLine("Adding Message {0}".F(msg));
                        commentaryMessages.Add(msg, relativeTime.TotalSeconds);
                    }
                }

            lastDrivers = drivers;
        }

        OverlayData.TimingSample CreateTimingSample(DataSample data, TimeSpan relativeTime, OverlayData.Driver[] drivers)
        {
            var session = data.SessionData.SessionInfo.Sessions.First(s => s.SessionNum == data.Telemetry.SessionNum);

            var timespan = TimeSpan.FromSeconds(data.Telemetry.SessionTimeRemain);
            var raceLapsPosition = string.Format("Lap {0}/{1}", data.Telemetry.RaceLaps, session.ResultsLapsComplete);
            var raceTimePosition = string.Format("{0:00}:{1:00}", timespan.Minutes, timespan.Seconds);
            var raceLapCounter = string.Format("Lap {0}", data.Telemetry.RaceLaps);

            if (data.Telemetry.RaceLaps == session.ResultsLapsComplete)
            {
                raceLapsPosition = "Final Lap";
                raceLapCounter = "Final Lap";
            }
            if (data.Telemetry.RaceLaps > session.ResultsLapsComplete)
            {
                raceLapsPosition = "Results";
                raceLapCounter = null;
            }

            return new OverlayData.TimingSample
            {
                MessageState = commentaryMessages.Messages(relativeTime.TotalSeconds),
                StartTime = (long)relativeTime.TotalSeconds,
                Drivers = drivers,
                RacePosition = session.IsLimitedSessionLaps ? raceLapsPosition : raceTimePosition,
                CurrentDriver = GetCurrentDriverDetails(data, drivers),
                LapCounter = session.IsLimitedSessionLaps ? null : raceLapCounter
            };
        }

        bool ProcessForLastLap(DataSample data, TimeSpan relativeTime)
        {
            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            if (data.Telemetry.RaceLaps <= session.ResultsLapsComplete)
            {
                for (int i = 0; i < data.SessionData.DriverInfo.Drivers.Length; i++)
                    lastLaps[i] = data.Telemetry.CarIdxLap[i];

                return false;
            }

            for (int i = 0; i < data.SessionData.DriverInfo.Drivers.Length; i++)
            {
                if (lastLaps[i] != data.Telemetry.CarIdxLap[i])
                {
                    lastLaps[i] = data.Telemetry.CarIdxLap[i];
                    var driver = data.SessionData.DriverInfo.Drivers[i];
                    var position = (int)session.ResultsPositions.First(r => r.CarIdx == i).Position;

                    var drivers = timingSample.Drivers.Where(d => d.CarIdx != driver.CarIdx)
                        .Select(d => d.Clone())
                        .ToList();

                    drivers.Insert((int)position-1, new OverlayData.Driver 
                    {
                        CarNumber = (int)driver.CarNumber,
                        Name = driver.UserName,
                        Position = position,
                        CarIdx = (int)driver.CarIdx
                    });

                    var p = 1;
                    foreach( var d in drivers)
                        d.Position = p++;

                    timingSample = CreateTimingSample(data, relativeTime, drivers.ToArray());

                    Trace.WriteLine(string.Format("Driver {0} Cross line in position {1}", driver.UserName, position));
                }
            }

            overlayData.TimingSamples.Add(timingSample);

            return true;
        }

        public void Stop(out string latestCreatedVideoFile, out string errors)
        {
            errors = null;
            latestCreatedVideoFile = this.latestCreatedVideoFile;

            if (latestCreatedVideoFile != null)
                overlayData.SaveTo(Path.ChangeExtension(latestCreatedVideoFile, ".xml"));
            else
            {
                Trace.WriteLine("No mp4/avi video file was detected during capturing.", "Critical");
                errors = "No mp4/avi video file was detected during capturing -- Assuming last created file";
                var guessedFileName = Directory.GetFiles(workingFolder, "*.avi")
                    .Select(fn => new { FileName = fn, CreationTime = File.GetCreationTime(fn) })
                    .OrderByDescending(f => f.CreationTime)
                    .FirstOrDefault();

                if (guessedFileName != null)
                {
                    latestCreatedVideoFile = guessedFileName.FileName;

                    if (!File.Exists(Path.ChangeExtension(latestCreatedVideoFile, ".xml")))
                    {
                        overlayData.SaveTo(Path.ChangeExtension(latestCreatedVideoFile, ".xml"));
                        return;
                    }
                }
             
                errors = "Unable to find captured video file in " + workingFolder;
            }
        }

        static OverlayData.Driver GetCurrentDriverDetails(DataSample data, OverlayData.Driver[] drivers)
        {
            var driver = drivers.FirstOrDefault(d => d.CarIdx == data.Telemetry.CamCarIdx);
            if (driver == null)
                return new OverlayData.Driver();

            driver.Indicator = GetOrdinal(driver.Position);

            return driver;
        }

        static string GetOrdinal(int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }

            switch (num % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        void OnCreated(object sender, FileSystemEventArgs e)
        {
            latestCreatedVideoFile = e.FullPath;
        }
    }
}
