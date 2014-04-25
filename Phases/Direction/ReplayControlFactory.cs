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
using IRacingReplayOverlay;
using iRacingSDK;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
	public static class ReplayControlFactory
	{
        public static ReplayControl CreateFrom(Incidents incidents, GapsToLeader gapsToLeader, PositionChanges positionChanges, LapsToFrameNumbers lapsToFrameNumbers)
        {
            var sessionData = iRacing.GetDataFeed().First().SessionData;
            var replayControl = new ReplayControl(sessionData);

            var firstLapTime = lapsToFrameNumbers[1].sessionTime;

            var firstCarIdx = positionChanges.First().DeltaDetails.First().CarIdx;
            replayControl.AddCarChange(0, firstCarIdx, "TV3", "is leader");

            var trackCameras = Settings.Default.trackCameras.Where( tc => tc.TrackName == sessionData.WeekendInfo.TrackDisplayName);

            var random = new System.Random();

            foreach(var gaps in gapsToLeader.Where(g => g.TimeStamp > firstLapTime))
            {
                var rand = random.Next(100);
                var offset = 0;
                var camera = "TV2";
                
                foreach( var tc in trackCameras)
                {
                    if(rand < tc.Ratio + offset)
                    {
                        camera = tc.CameraName;
                        break;
                    }
                    offset += tc.Ratio;
                }

                replayControl.AddCarChange(gaps.TimeStamp, gaps.CarIdx, camera, "close by {0}".F(gaps.Gap));
            }
            /*
            foreach (var lap in lapsToFrameNumbers.Skip(2))
            {
                var change = positionChanges[lap.LapNumber].DeltaDetails.FirstOrDefault(d => d.Delta > 0);
                if (false) //(change != null)
                {
                    Trace.WriteLine("Switching to {0} for overtake on lap {1}".F( change.CarIdx, lap.LapNumber));

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];
                    var camera = (new System.Random().Next() % 2) == 1 ? "TV1" : "TV2";
                    replayControl.AddCarChange(frameNumber.sessionTime, change.CarIdx, camera, "is overtaking");
                }
                else
                {
                    var gaps = gapsToLeader[LapSector.ForLap(lap.LapNumber)];
                    var carIdx = gaps.GapsByCarIndex.OrderBy(g => g.Value).Skip(1).First().Key;

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];

                    var camera = (new System.Random().Next() % 2) == 1 ? "Nose" : "Roll Bar";

                    replayControl.AddCarChange(frameNumber.sessionTime, carIdx, "TV1", "is close");

                    Trace.WriteLine("Switching to {0} on lap {1}".F( carIdx, lap.LapNumber));
                }
            }
            */
            //foreach (var ic in incidents)
            //    replayControl.AddShortCarChange(ic.StartSessionTime, ic.EndSessionTime, ic.CarIdx, "TV2", "incident");

            return replayControl;
        }
    }
}

