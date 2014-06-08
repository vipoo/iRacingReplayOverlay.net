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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class RuleRandomDriver : IDirectionRule
    {
        readonly RemovalEdits removalEdits;
        readonly TrackCamera TV3;

        DateTime reselectLeaderAt = DateTime.Now;

        public RuleRandomDriver(TrackCamera[] cameras, RemovalEdits removalEdits)
        {
            this.removalEdits = removalEdits;
            TV3 = cameras.First(tc => tc.CameraName == "TV3");
        }

        public bool IsActive(DataSample data)
        {
            return false;
        }

        public void Direct(DataSample data)
        {
        }
    }
}
