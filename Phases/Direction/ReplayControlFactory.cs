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
//
using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Direction;
using iRacingReplayOverlay.Support;
using iRacingSDK;
using System.Diagnostics;
using System.Linq;

namespace IRacingReplayOverlay.Phases.Direction
{
	public static class ReplayControlFactory
	{
        public static ReplayControl CreateFrom(GapsToLeader gapsToLeader, PositionChanges positionChanges, LapsToFrameNumbers lapsToFrameNumbers)
        {
            var replayControl = new ReplayControl(iRacing.GetDataFeed().First().SessionData);

            var firstCarIdx = positionChanges.First().DeltaDetails.First().CarIdx;
            replayControl.AddCarChange(lapsToFrameNumbers[1], firstCarIdx, "TV3");
            foreach (var lap in lapsToFrameNumbers.Skip(2))
            {
                var change = positionChanges[lap.LapNumber].DeltaDetails.FirstOrDefault(d => d.Delta > 0);
                if (change != null)
                {
                    Trace.WriteLine("Switching to {0} for overtake on lap {1}".F( change.CarIdx, lap.LapNumber));

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];
                    replayControl.AddCarChange(frameNumber, change.CarIdx, "TV1");
                }
                else
                {
                    var gaps = gapsToLeader[lap.LapNumber];
                    var carIdx = gaps.GapsByCarIndex.OrderBy(g => g.Value).First().Key;

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];

                    replayControl.AddCarChange(frameNumber, change.CarIdx, "TV1");

                    Trace.WriteLine("Switching to {0} on lap {1}".F( carIdx, lap.LapNumber));
                }
            }

            return replayControl;
        }
    }
}

