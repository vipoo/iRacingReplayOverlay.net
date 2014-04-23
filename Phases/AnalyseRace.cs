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
using iRacingSDK;
using System;
using System.Linq;
using System.Threading;

namespace IRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        ReplayControl replayControl;

        public void _AnalyseRace(bool? requestAbort)
        {
            var incidents = new Incidents();
            foreach( var data in iRacing.GetDataFeed().RaceIncidents() )
                incidents.Process(data);

            var gapsToLeader = new GapsToLeader();
            var positionChanges = new PositionChanges();
            var lapsToFrameNumbers = new LapsToFrameNumbers();

            foreach (var data in iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(16)
                .RaceOnly()
                .TakeWhile(d => d.Telemetry.RaceLaps < 4))
            {
                gapsToLeader.Process(data);
                positionChanges.Process(data);
                lapsToFrameNumbers.Process(data);
            }

            replayControl = ReplayControlFactory.CreateFrom(incidents, gapsToLeader, positionChanges, lapsToFrameNumbers);
        }
    }
}
