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
using System.Diagnostics;
using System.IO;
using System.Linq;
using iRacingReplayOverlay.Support;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class Capture
    {
        readonly String workingFolder;
        readonly OverlayData overlayData;
        readonly FileSystemWatcher[] fileWatchers;
        string latestCreatedVideoFile;
        DateTime lastTime;

        public Capture(OverlayData overlayData, string workingFolder)
        {
            this.overlayData = overlayData;
            this.workingFolder = workingFolder;

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
            if ((DateTime.Now - lastTime).TotalSeconds < 4)
                return;

            lastTime = DateTime.Now;

            var positions = data.Telemetry.Cars
                .Where(c => c.Index != 0)
                .OrderByDescending(c => c.Lap + c.DistancePercentage)
                .ToArray();

            var session = data.SessionData.SessionInfo.Sessions.First(s => s.SessionNum == data.Telemetry.SessionNum);

            var timespan = TimeSpan.FromSeconds(data.Telemetry.SessionTimeRemain);
            var raceLapsPosition = string.Format("Lap {0}/{1}", session._SessionLaps - data.Telemetry.SessionLapsRemain, session.SessionLaps);
            var raceTimePosition = string.Format("{0:00}:{1:00}", timespan.Minutes, timespan.Seconds);

			var raceLapCounter = string.Format("Lap {0}", data.Telemetry.RaceLaps);

            var drivers = positions.Select((c, p) => new OverlayData.Driver { Name = c.Driver.UserName, CarNumber = (int)c.Driver.CarNumber, Position = p + 1 }).ToArray();

            var timingSample = new OverlayData.TimingSample
            {
                StartTime = (long)relativeTime.TotalSeconds,
                Drivers = drivers,
                RacePosition = session.IsLimitedSessionLaps ? raceLapsPosition : raceTimePosition,
				CurrentDriver = GetCurrentDriverDetails(data, positions),
				LapCounter = session.IsLimitedSessionLaps ? null : raceLapCounter 
            };

            overlayData.TimingSamples.Add(timingSample);
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

        static OverlayData.Driver GetCurrentDriverDetails(DataSample data, Car[] positions)
        {
            var position = positions
                .Select((p, i) => new { Position = i + 1, Details = p })
                .FirstOrDefault(p => p.Details.Index == data.Telemetry.CamCarIdx);

            if (position == null)
                return new OverlayData.Driver();

            return new OverlayData.Driver
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
