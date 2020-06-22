// This file is part of iRacingReplayDirector.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayDirector.net
//
// iRacingReplayDirector is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayDirector is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License333
// along with iRacingReplayDirector.  If not, see <http://www.gnu.org/licenses/>.

using iRacingSDK;

namespace iRacingReplayDirector.Phases.Capturing
{
    public class SessionDataCapture
    {
        readonly OverlayData overlayData;

        public SessionDataCapture(OverlayData overlayData)
        {
            this.overlayData = overlayData;
        }

        public void Process(DataSample data)
        {
            if (overlayData.SessionData == null)
                overlayData.SessionData = data.SessionData;
        }
    }
}
