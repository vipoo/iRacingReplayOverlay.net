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

using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRacingReplayOverlay.Phases.Capturing
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
