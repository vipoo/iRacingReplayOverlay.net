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
    public class CaptureLeaderBoard
    {
        readonly String workingFolder;
        readonly OverlayData overlayData;
        readonly CommentaryMessages commentaryMessages;
        readonly RemovalEdits removalEdits;

        OverlayData.LeaderBoard leaderBoard;
        OverlayData.Driver[] lastDrivers;
        int leaderBoardUpdateRate = 0;
        double raceStartTimeOffset = 0;
        bool[] haveNotedCheckerdFlag = new bool[64];

        public CaptureLeaderBoard(OverlayData overlayData, CommentaryMessages commentaryMessages, RemovalEdits removalEdits, string workingFolder)
        {
            this.overlayData = overlayData;
            this.workingFolder = workingFolder;
            this.commentaryMessages = commentaryMessages;
            this.removalEdits = removalEdits;
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            if (ProcessForLastLap(data, relativeTime))
                return;

            if (ProcessForStarting(data, relativeTime))
                return;

            if (leaderBoardUpdateRate == 0 || leaderBoard == null)
                leaderBoard = CreateLeaderBoard(data, relativeTime, ProcessLatestRunningOrder(data, relativeTime));
            else
                leaderBoard = CreateLeaderBoard(data, relativeTime, leaderBoard.Drivers);

            leaderBoardUpdateRate++;
            leaderBoardUpdateRate = leaderBoardUpdateRate % 8;

            overlayData.LeaderBoards.Add(leaderBoard);
        }

        private bool ProcessForStarting(DataSample data, TimeSpan relativeTime)
        {
            if (data.Telemetry.RaceDistance > 1.10)
                return false;

            leaderBoard = CreateLeaderBoard(data, relativeTime, new OverlayData.Driver[0]);

            leaderBoardUpdateRate = 0;

            overlayData.LeaderBoards.Add(leaderBoard);

            return true;
        }

        OverlayData.Driver[] ProcessLatestRunningOrder(DataSample data, TimeSpan relativeTime)
        {
            var drivers = data.Telemetry.Cars.Where(c => !c.IsPaceCar ).Select(c => new OverlayData.Driver
            {
                UserName = c.UserName,
                CarNumber = c.CarNumber,
                Position = c.Position,
                CarIdx = c.CarIdx
            })
            .OrderBy( c => c.Position)
            .ToArray();

            if (lastDrivers != null)
                foreach (var d in drivers.OrderBy(d => d.Position))
                {
                    var lastPosition = lastDrivers.FirstOrDefault(lp => lp.CarIdx == d.CarIdx);
                    if (lastPosition != null && lastPosition.Position != d.Position)
                    {
                        removalEdits.InterestingThingHappend(data);
                        var msg = "{0} in {1}{2}".F(d.UserName, d.Position, d.Position.Ordinal());
                        Trace.WriteLine("{0} {1}".F(data.Telemetry.SessionTimeSpan, msg), "INFO");
                        commentaryMessages.Add(msg, relativeTime.TotalSeconds);
                    }
                }

            lastDrivers = drivers;

            return drivers;
        }

        OverlayData.LeaderBoard CreateLeaderBoard(DataSample data, TimeSpan relativeTime, OverlayData.Driver[] drivers)
        {
            if( raceStartTimeOffset == 0 && data.Telemetry.SessionState == SessionState.Racing)
                raceStartTimeOffset = data.Telemetry.SessionTime;

            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            var timespan = raceStartTimeOffset == 0 ? TimeSpan.FromSeconds(session._SessionTime) : TimeSpan.FromSeconds(data.Telemetry.SessionTimeRemain + raceStartTimeOffset);
            var raceLapsPosition = string.Format("Lap {0}/{1}", data.Telemetry.RaceLaps, session.ResultsLapsComplete);
            var raceTimePosition = (timespan.TotalSeconds <= 0 ? TimeSpan.FromSeconds(0) : timespan).ToString(@"mm\:ss");
            var raceLapCounter = string.Format("Lap {0}", data.Telemetry.RaceLaps);

            OverrideRacePositionDetails(data, session, ref raceLapsPosition, ref raceLapCounter);

            return new OverlayData.LeaderBoard
            {
                StartTime = relativeTime.TotalSeconds,
                Drivers = drivers,
                RacePosition = session.IsLimitedSessionLaps ? raceLapsPosition : raceTimePosition,
                LapCounter = session.IsLimitedSessionLaps ? null : raceLapCounter
            };
        }

        static void OverrideRacePositionDetails(DataSample data, SessionData._SessionInfo._Sessions session, ref string raceLapsPosition, ref string raceLapCounter)
        {
            if (data.Telemetry.RaceLaps <= 0)
            {
                raceLapCounter = null;
                raceLapsPosition = "";
                return;
            }
            
            if (data.Telemetry.RaceLaps < session.ResultsLapsComplete)
                return;

            if (data.Telemetry.RaceLaps == session.ResultsLapsComplete)
            {
                raceLapsPosition = raceLapCounter = "Final Lap";
                return;
            }
         
            raceLapsPosition = raceLapCounter = "Results";
        }

        bool ProcessForLastLap(DataSample data, TimeSpan relativeTime)
        {
            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            if (!data.Telemetry.LeaderHasFinished)
                return false;

            if (data.LastSample == null)
                return false;

            removalEdits.InterestingThingHappend(data);

            leaderBoard = CreateLeaderBoard(data, relativeTime, leaderBoard.Drivers);

            for (int i = 1; i < data.SessionData.DriverInfo.Drivers.Length; i++)
                AnnounceIfDriverHasFinished(data, relativeTime, session, i);

            overlayData.LeaderBoards.Add(leaderBoard);

            return true;
        }

        private void AnnounceIfDriverHasFinished(DataSample data, TimeSpan relativeTime, SessionData._SessionInfo._Sessions session, int i)
        {
            if (!data.LastSample.Telemetry.Cars[i].HasSeenCheckeredFlag || haveNotedCheckerdFlag[i])
                return;

            haveNotedCheckerdFlag[i] = true;

            var driver = data.SessionData.DriverInfo.Drivers[i];
            var position = (int)session.ResultsPositions.First(r => r.CarIdx == i).Position;

            var drivers = leaderBoard.Drivers.Where(d => d.CarIdx != i)
                .Select(d => d.Clone())
                .ToList();

            drivers.Insert((int)position - 1, new OverlayData.Driver
            {
                CarNumber = (int)driver.CarNumber,
                UserName = driver.UserName,
                Position = position,
                CarIdx = i
            });

            var p = 1;
            foreach (var d in drivers)
                d.Position = p++;

            leaderBoard = CreateLeaderBoard(data, relativeTime, drivers.ToArray());

            var msg = string.Format("{0} finished in {1}{2}", driver.UserName, position, position.Ordinal());
            Trace.WriteLine("{0} {1}".F(data.Telemetry.SessionTimeSpan, msg), "INFO");
            commentaryMessages.Add(msg, relativeTime.TotalSeconds);
        }
    }
}
