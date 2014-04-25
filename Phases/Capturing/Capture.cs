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

using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingReplayOverlay.Support;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class Capture
    {
        string tempFileName;
        FileSystemWatcher[] fileWatchers;
        string latestCreatedVideoFile;
        DateTime startTime;
        string workingFolder;
        TimingSamples timingSamples;

        public void Start(string workingFolder)
        {
            this.workingFolder = workingFolder;
            tempFileName = Path.GetTempFileName();
            Trace.WriteLine("Creating temp file for game data {0}".F(tempFileName));

            startTime = DateTime.Now;
            timingSamples = new TimingSamples();

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

        public void Process(DataSample data)
        {
            WriteNewLeaderBoardRow(data);
        }

        public void Stop(out string latestCreatedVideoFile, out string errors)
        {
            errors = null;
            latestCreatedVideoFile = this.latestCreatedVideoFile;


            if (latestCreatedVideoFile != null)
                timingSamples.SaveTo(Path.ChangeExtension(latestCreatedVideoFile, ".xml"));
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
                        timingSamples.SaveTo(Path.ChangeExtension(latestCreatedVideoFile, ".xml"));
                        return;
                    }
                }
             
                errors = "Unable to find captured video file in " + workingFolder;
            }
        }

        DateTime lastTime;
        void WriteNewLeaderBoardRow( DataSample data)
        {
            var timeNow = DateTime.Now - startTime;

            if( (DateTime.Now - lastTime).TotalSeconds < 4)
                return;

            lastTime = DateTime.Now;

            var numberOfDrivers = data.SessionData.DriverInfo.Drivers.Length;

            var positions = data.Telemetry.Cars
                .Take(numberOfDrivers)
                .Where(c => c.Index != 0)
                .OrderByDescending(c => c.Lap + c.DistancePercentage)
                .ToArray();

            var session = data.SessionData.SessionInfo.Sessions.First(s => s.SessionNum == data.Telemetry.SessionNum);

            var timespan = TimeSpan.FromSeconds(data.Telemetry.SessionTimeRemain);
            var raceLapsPosition = string.Format("Lap {0}/{1}", session._SessionLaps - data.Telemetry.SessionLapsRemain, session.SessionLaps);
            var raceTimePosition = string.Format("{0:00}:{1:00}", timespan.Minutes, timespan.Seconds);

            var timingSample = new TimingSample
            {
                StartTime = (int)timeNow.TotalSeconds,
                Drivers = positions.Select((c,p) => new TimingSample.Driver { Name = c.Driver.UserName, CarNumber = (int)c.Driver.CarNumber, Position = p+1 }).ToArray(),
                RacePosition = session.IsLimitedSessionLaps ? raceLapsPosition : raceTimePosition,
                CurrentDriver = GetCurrentDriverDetails(data, positions)
            };

            timingSamples.Add(timingSample);
        }

        static TimingSample.Driver GetCurrentDriverDetails(DataSample data, Car[] positions)
        {
            var position = positions
                .Select((p, i) => new { Position = i + 1, Details = p })
                .FirstOrDefault(p => p.Details.Index == data.Telemetry.CamCarIdx);

            if (position == null)
                return new TimingSample.Driver();

            return new TimingSample.Driver
            {
                Indicator = GetOrdinal(position.Position),
                Position = position.Position,
                CarNumber = (int)data.Telemetry.CamCar.Driver.CarNumber,
                Name = data.Telemetry.CamCar.Driver.UserName
            };
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
