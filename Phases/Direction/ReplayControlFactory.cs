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

namespace iRacingReplayOverlay.Phases.Direction
{
	public static class ReplayControlFactory
	{
        public static ReplayControl CreateFrom(Incidents incidents, GapsToLeader gapsToLeader, PositionChanges positionChanges, LapsToFrameNumbers lapsToFrameNumbers)
        {
            var replayControl = new ReplayControl(iRacing.GetDataFeed().First().SessionData);

            var firstCarIdx = positionChanges.First().DeltaDetails.First().CarIdx;
            replayControl.AddCarChange(lapsToFrameNumbers[1].sessionTime, firstCarIdx, "TV3", "is leader");
            foreach (var lap in lapsToFrameNumbers.Skip(2))
            {
                var change = positionChanges[lap.LapNumber].DeltaDetails.FirstOrDefault(d => d.Delta > 0);
                if (change != null)
                {
                    Trace.WriteLine("Switching to {0} for overtake on lap {1}".F( change.CarIdx, lap.LapNumber));

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];
                    var camera = (new System.Random().Next() % 2) == 1 ? "TV1" : "TV2";
                    replayControl.AddCarChange(frameNumber.sessionTime, change.CarIdx, camera, "is overtaking");
                }
                else
                {
                    var gaps = gapsToLeader[lap.LapNumber];
                    var carIdx = gaps.GapsByCarIndex.OrderBy(g => g.Value).First().Key;

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];

                    var camera = (new System.Random().Next() % 2) == 1 ? "Nose" : "Rollbar";

                    replayControl.AddCarChange(frameNumber.sessionTime, carIdx, camera, "is close");

                    Trace.WriteLine("Switching to {0} on lap {1}".F( carIdx, lap.LapNumber));
                }
            }

            foreach (var ic in incidents)
                replayControl.AddShortCarChange(ic.StartSessionTime, ic.EndSessionTime, ic.CarIdx, "TV2", "incident");

            return replayControl;
        }
    }
}

