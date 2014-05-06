﻿// This file is part of iRacingReplayOverlay.
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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using iRacingReplayOverlay.Support;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class RemovalEdits
    {
        readonly List<OverlayData.BoringBit> boringBits;
        OverlayData.BoringBit lastBoringBit = null;
        double lastInterestingTime = 0;

        public RemovalEdits(OverlayData overlayData)
        {
            this.boringBits = overlayData.EditCuts;
        }

        public void InterestingThingHappend(DataSample data)
        {
            AddPreviouslyMarkedBoringBit();
         
            lastInterestingTime = data.Telemetry.SessionTime;
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            if (data.LastSample == null)
                return;

            if (lastInterestingTime + 30 > data.Telemetry.SessionTime)
                return;

            if (lastBoringBit == null)
            {
                Trace.WriteLine("{0} Marking start of a cut".F(TimeSpan.FromSeconds(data.Telemetry.SessionTime)));
                lastBoringBit = new OverlayData.BoringBit { StartTime = data.Telemetry.SessionTime };
            }

            lastBoringBit.EndTime = data.Telemetry.SessionTime;
        }

        void AddPreviouslyMarkedBoringBit()
        {
            AddLastBoringBit();

            lastBoringBit = null;
        }

        private void AddLastBoringBit()
        {
            if (lastBoringBit == null)
                return;

            if (lastBoringBit.Duration < 30)
                return;

            Trace.WriteLine("Cutting from {0} to {1}".F(TimeSpan.FromSeconds(lastBoringBit.StartTime), TimeSpan.FromSeconds(lastBoringBit.EndTime)), "INFO");
            boringBits.Add(lastBoringBit);
        }

        public void Stop()
        {
            boringBits.Add(lastBoringBit);

            var totalTimeCut = boringBits.Sum(b => b.EndTime - b.StartTime);
            Trace.WriteLine("Total cut time is {0}".F(TimeSpan.FromSeconds(totalTimeCut)), "INFO");
        }
    }
}