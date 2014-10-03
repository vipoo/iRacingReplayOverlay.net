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

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class EditMarker
    {
        readonly RemovalEdits removalEdits;
        readonly InterestState interest;

        int? lastCarIdx = null;

        public EditMarker(RemovalEdits removalEdits, InterestState interest)
        {
            this.removalEdits = removalEdits;
            this.interest = interest;
        }

        internal void Start(int carIdx = -1)
        {
            if (lastCarIdx != null )
                removalEdits.InterestingThingStopped(interest, lastCarIdx.Value);

            lastCarIdx = carIdx;
            removalEdits.InterestingThingStarted(interest, carIdx);
        }

        internal void Stop(int carIdx = -1)
        {
            removalEdits.InterestingThingStopped(interest, carIdx);
            lastCarIdx = null;
        }
    }
}
