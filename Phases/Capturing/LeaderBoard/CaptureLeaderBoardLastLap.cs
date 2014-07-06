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
using System.Linq;

namespace iRacingReplayOverlay.Phases.Capturing.LeaderBoard
{
    public class CaptureLeaderBoardLastLap
    {
        readonly OverlayData overlayData;
        readonly CaptureLeaderBoard captureLeaderBoard;
        readonly CommentaryMessages commentaryMessages;

        bool[] haveNotedCheckerdFlag = new bool[64];

        public CaptureLeaderBoardLastLap(CaptureLeaderBoard captureLeaderBoard, OverlayData overlayData, CommentaryMessages commentaryMessages)
        {
            this.captureLeaderBoard = captureLeaderBoard;
            this.overlayData = overlayData;
            this.commentaryMessages = commentaryMessages;
        }

        public void Process(DataSample data, TimeSpan relativeTime, ref OverlayData.LeaderBoard leaderBoard)
        {
            var session = data.SessionData.SessionInfo.Sessions[data.Telemetry.SessionNum];

            leaderBoard = captureLeaderBoard.CreateLeaderBoard(data, relativeTime, leaderBoard.Drivers);

            for (int i = 1; i < data.SessionData.DriverInfo.Drivers.Length; i++)
                AnnounceIfDriverHasFinished(data, relativeTime, session, i, ref leaderBoard);

            overlayData.LeaderBoards.Add(leaderBoard);
        }

        void AnnounceIfDriverHasFinished(DataSample data, TimeSpan relativeTime, SessionData._SessionInfo._Sessions session, int i, ref OverlayData.LeaderBoard leaderBoard)
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

            leaderBoard = captureLeaderBoard.CreateLeaderBoard(data, relativeTime, drivers.ToArray());

            var msg = string.Format("{0} finished in {1}{2}", driver.UserName, position, position.Ordinal());
            Trace.WriteLine("{0} {1}".F(data.Telemetry.SessionTimeSpan, msg), "INFO");
            commentaryMessages.Add(msg, relativeTime.TotalSeconds);
        }
    }
}
