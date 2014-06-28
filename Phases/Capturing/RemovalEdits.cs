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
using iRacingSDK.Support;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public class RemovalEdits
    {
        public enum InterestStates
        {
            WaitingForInterestToFade, // + 15sconds
            BeginingPotentialBoringBit, // + 30seconds
            BoringBitActivate //until interesting thing happens - 4 seconds
        }

        readonly List<OverlayData.BoringBit> boringBits;
        OverlayData.BoringBit lastBoringBit = null;
        TimeSpan lastInterestingTime = TimeSpan.FromSeconds(0);
        InterestStates interestState = InterestStates.WaitingForInterestToFade;

        public RemovalEdits(OverlayData overlayData)
        {
            this.boringBits = overlayData.EditCuts;
        }

        public void InterestingThingHappend(DataSample data)
        {
			AddPreviouslyMarkedBoringBit();
         
            lastInterestingTime = data.Telemetry.SessionTimeSpan;
            interestState = InterestStates.WaitingForInterestToFade;
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            if (data.LastSample == null)
                return;

            if (interestState == InterestStates.WaitingForInterestToFade && lastInterestingTime + 7.Seconds() <= data.Telemetry.SessionTimeSpan)
            {
                interestState = InterestStates.BeginingPotentialBoringBit;
                lastBoringBit = new OverlayData.BoringBit { StartTime = relativeTime.TotalSeconds };
                Trace.WriteLine("{0} Marking start of a cut - at video time of {1}".F(data.Telemetry.SessionTimeSpan, relativeTime), "INFO");
            }

            if( interestState == InterestStates.BeginingPotentialBoringBit && lastBoringBit.StartTime + 15 <= relativeTime.TotalSeconds)
                interestState = InterestStates.BoringBitActivate;

            if( interestState == InterestStates.BoringBitActivate)
                lastBoringBit.EndTime = relativeTime.TotalSeconds;
        }

        void AddPreviouslyMarkedBoringBit()
        {
            if (interestState != InterestStates.BoringBitActivate)
                return;

            Trace.WriteLine("Cutting from {0} to {1} (video time)".F(TimeSpan.FromSeconds(lastBoringBit.StartTime), TimeSpan.FromSeconds(lastBoringBit.EndTime)), "INFO");
            
            boringBits.Add(lastBoringBit);
        }

        public void Stop()
        {
            AddPreviouslyMarkedBoringBit();

            var totalTimeCut = boringBits.Sum(b => b.EndTime - b.StartTime);
            Trace.WriteLine("Total cut time is {0}".F(TimeSpan.FromSeconds(totalTimeCut)), "INFO");
        }
    }
}
