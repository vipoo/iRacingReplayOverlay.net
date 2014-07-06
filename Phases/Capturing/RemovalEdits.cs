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
    public class InterestLevel
    {
        public TimeSpan Time;
        public short Interest;

        internal bool Interesting;
        internal int Index;

        public const short FIRST_LAP = 100;
        public const short LAST_LAP = 100;
        public const short INCIDENT = 80;
        public const short BATTLE = 60;
        public const short OVERTAKE = 55;
        public const short PITSTOP = 50;
    }

    public class RemovalEdits
    {
        readonly List<OverlayData.BoringBit> boringBits;

        short lastInterest = 0;
        List<InterestLevel> interestLevels = new List<InterestLevel>();

        public RemovalEdits(OverlayData overlayData)
        {
            this.boringBits = overlayData.EditCuts;
        }

        public void InterestingThingHappend(short interest)
        {
            lastInterest = Math.Max(interest, lastInterest);
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            interestLevels.Add(new InterestLevel { Time = relativeTime, Interest = lastInterest, Index = interestLevels.Count });
            lastInterest = 0;
        }
        
        public void Stop()
        {
            const int TenMinutes = 36000;

            var startTime = interestLevels.First().Time;
            var endTime = interestLevels.Last().Time;

            foreach (var il in interestLevels.OrderByDescending(i => i.Interest).ThenByDescending(i => i.Time.TotalSeconds).Take(TenMinutes))
                il.Interesting = true;

            OverlayData.BoringBit boringBit = null;
            int lastIndex = -1;

            foreach (var il in interestLevels.OrderBy(i => i.Index).Where(i => !i.Interesting))
            {
                if( boringBit != null && lastIndex + 1 != il.Index )
                    AddBoringBit(ref boringBit);

                if (boringBit == null)
                    boringBit = new OverlayData.BoringBit { StartTime = il.Time.TotalSeconds };
                else
                    boringBit.EndTime = il.Time.TotalSeconds;
                
                lastIndex = il.Index;
            }

            if (boringBit != null)
                AddBoringBit(ref boringBit);

            Trace.WriteLine("Total edited out time is {0}".F(TimeSpan.FromSeconds(boringBits.Sum(b => b.Duration))), "INFO");
        }

        private void AddBoringBit(ref OverlayData.BoringBit boringBit)
        {
            if (boringBit.Duration < 10d)
                Trace.WriteLine("Not applying edit of less then 10 seconds. From {0} for {1}".F(boringBit.StartTimeSpan, boringBit.DurationSpan));
            else
            {
                this.boringBits.Add(boringBit);
                Trace.WriteLine("Applying edit: from {0} for {1}".F(boringBit.StartTimeSpan, boringBit.DurationSpan));
            }
            boringBit = null;
        }
    }
}
