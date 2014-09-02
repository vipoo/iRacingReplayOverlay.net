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

using iRacingReplayOverlay.Support;
using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Diagnostics;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class RecordFastestLaps
    {
        readonly OverlayData overlayData;
        FastLap lastFastestLap = null;
        double? timeToNoteFastestLap = null;

        public RecordFastestLaps(OverlayData overlayData)
        {
            this.overlayData = overlayData;
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            ShowAnyPendingFastestLap(data, relativeTime);

            if (lastFastestLap != data.Telemetry.FastestLap)
                NoteNewFastestLap(data, relativeTime);
        }

        void NoteNewFastestLap(DataSample data, TimeSpan relativeTime)
        {
            if (timeToNoteFastestLap == null)
                timeToNoteFastestLap = data.Telemetry.SessionTime + 20;

            lastFastestLap = data.Telemetry.FastestLap;

            TraceInfo.WriteLine("{0} Driver {1} recorded a new fast lap of {2:0.00}", data.Telemetry.SessionTimeSpan, lastFastestLap.Driver.UserName, lastFastestLap.Time.TotalSeconds);
        }

        void ShowAnyPendingFastestLap(DataSample data, TimeSpan relativeTime)
        {
            if (timeToNoteFastestLap == null || timeToNoteFastestLap.Value >= data.Telemetry.SessionTime)
                return;

            var fastLap = new OverlayData.FastLap
            {
                StartTime = (int)relativeTime.TotalSeconds,
                Time = lastFastestLap.Time.TotalSeconds,
                Driver = new OverlayData.Driver
                {
                    UserName = lastFastestLap.Driver.UserName,
                    CarNumber = (int)lastFastestLap.Driver.CarNumber
                }
            };

            TraceInfo.WriteLine("{0} Showing Driver {1} recorded a new fast lap of {2:0.00}", data.Telemetry.SessionTimeSpan, lastFastestLap.Driver.UserName, lastFastestLap.Time.TotalSeconds);
            overlayData.FastestLaps.Add(fastLap);
            timeToNoteFastestLap = null;
        }
    }
}
