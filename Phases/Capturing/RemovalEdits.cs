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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using iRacingReplayOverlay.Support;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class RemovalEdits
    {
        readonly List<OverlayData.BoringBit> boringBits;

        public RemovalEdits(OverlayData overlayData)
        {
            this.boringBits = overlayData.BoringLap;
        }

        public void InterestingThingHappend()
        {
            Trace.WriteLineIf(!somethingInterestingHappened, "Marking {0} as interesting".F(sectorStartTime), "INFO");
            somethingInterestingHappened = true;
        }

        bool somethingInterestingHappened = false;
        TimeSpan sectorStartTime;
        OverlayData.BoringBit lastBoringBit = null;

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            if (data.LastSample == null)
                return;

            if (data.LastSample.Telemetry.RaceLapSector == data.Telemetry.RaceLapSector)
                return;

            Trace.WriteLine("{0} {1} {2}".F(relativeTime, data.Telemetry.RaceLapSector.LapNumber, data.Telemetry.RaceLapSector.Sector), "DEBUG");

            if (!somethingInterestingHappened)
                AddBoringBitsToEdits(relativeTime);
            else
                AddPreviouslyMarkedBoringBit();

            somethingInterestingHappened = false;
            sectorStartTime = relativeTime;
        }

        void AddBoringBitsToEdits(TimeSpan relativeTime)
        {
            if (lastBoringBit == null)
            {
                lastBoringBit = new OverlayData.BoringBit { StartTime = sectorStartTime.TotalSeconds, EndTime = relativeTime.TotalSeconds };
                Trace.WriteLine("Marking from {0} to {1}".F(TimeSpan.FromSeconds(lastBoringBit.StartTime), TimeSpan.FromSeconds(lastBoringBit.EndTime)), "DEBUG");

                return;
            }

            if (Math.Abs(lastBoringBit.EndTime - sectorStartTime.TotalSeconds) > 5)
            {
                AddLastBoringBit();
                lastBoringBit = new OverlayData.BoringBit { StartTime = sectorStartTime.TotalSeconds, EndTime = relativeTime.TotalSeconds };
                Trace.WriteLine("Marking from {0} to {1}".F(TimeSpan.FromSeconds(lastBoringBit.StartTime), TimeSpan.FromSeconds(lastBoringBit.EndTime)), "DEBUG");
                return;
            }

            Trace.WriteLine("Extending cut from {0} to {1}".F(TimeSpan.FromSeconds(lastBoringBit.EndTime), relativeTime), "DEBUG");
            lastBoringBit.EndTime = relativeTime.TotalSeconds;
        }

        void AddPreviouslyMarkedBoringBit()
        {
            if (lastBoringBit == null)
                return;

            AddLastBoringBit();
            lastBoringBit = null;
        }

        private void AddLastBoringBit()
        {
            Trace.WriteLine("Cutting from {0} to {1}".F(TimeSpan.FromSeconds(lastBoringBit.StartTime), TimeSpan.FromSeconds(lastBoringBit.EndTime)), "INFO");
            boringBits.Add(lastBoringBit);
        }

        public void Stop()
        {
            if( lastBoringBit != null)
                boringBits.Add(lastBoringBit);

            var totalTimeCut = boringBits.Sum(b => b.EndTime - b.StartTime);
            Trace.WriteLine("Total cut time is {0}".F(TimeSpan.FromSeconds(totalTimeCut)), "INFO");
        }
    }
}
