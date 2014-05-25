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
    public class CaptureLeaderBoardMiddleLaps
    {
        readonly OverlayData overlayData;
        readonly CaptureLeaderBoard captureLeaderBoard;
        readonly RemovalEdits removalEdits;
        readonly CommentaryMessages commentaryMessages;

        int leaderBoardUpdateRate = 0;
        OverlayData.Driver[] lastDrivers;

        public CaptureLeaderBoardMiddleLaps(CaptureLeaderBoard captureLeaderBoard, OverlayData overlayData, RemovalEdits removalEdits, CommentaryMessages commentaryMessages)
        {
            this.captureLeaderBoard = captureLeaderBoard;
            this.overlayData = overlayData;
            this.removalEdits = removalEdits;
            this.commentaryMessages = commentaryMessages;
        }

        public void Process(DataSample data, TimeSpan relativeTime, ref OverlayData.LeaderBoard leaderBoard)
        {
            if (leaderBoardUpdateRate == 0 || leaderBoard == null)
                leaderBoard = captureLeaderBoard.CreateLeaderBoard(data, relativeTime, LatestRunningOrder(data, relativeTime));
            else
                leaderBoard = captureLeaderBoard.CreateLeaderBoard(data, relativeTime, leaderBoard.Drivers);

            leaderBoardUpdateRate++;
            leaderBoardUpdateRate = leaderBoardUpdateRate % 8;

            overlayData.LeaderBoards.Add(leaderBoard);
        }

        OverlayData.Driver[] LatestRunningOrder(DataSample data, TimeSpan relativeTime)
        {
            var drivers = data.Telemetry.Cars.Where(c => !c.IsPaceCar).Select(c => new OverlayData.Driver
            {
                UserName = c.UserName,
                CarNumber = c.CarNumber,
                Position = c.Position,
                CarIdx = c.CarIdx
            })
            .OrderBy(c => c.Position)
            .ToArray();

            if (lastDrivers != null)
                foreach (var d in drivers.OrderBy(d => d.Position))
                {
                    var lastPosition = lastDrivers.FirstOrDefault(lp => lp.CarIdx == d.CarIdx);
                    if (lastPosition != null && lastPosition.Position != d.Position)
                    {
                        removalEdits.InterestingThingHappend(data);
                        var position = d.Position != null ? d.Position.Value.ToString() : "";
                        var indicator = d.Position != null ? d.Position.Value.Ordinal() : "";
                        var msg = "{0} in {1}{2}".F(d.UserName, position, indicator);
                        Trace.WriteLine("{0} {1}".F(data.Telemetry.SessionTimeSpan, msg), "INFO");
                        commentaryMessages.Add(msg, relativeTime.TotalSeconds);
                    }
                }

            lastDrivers = drivers;

            return drivers;
        }
    }
}
