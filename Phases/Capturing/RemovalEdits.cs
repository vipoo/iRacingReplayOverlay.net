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
    public enum InterestState { None = 0, FirstLap = 1, LastLap = 2, Incident = 3, Restart = 4, Battle = 5 /*, Overtake = 5, Pitstp = 6 */};
    public class InterestLevel
    {
        public TimeSpan Time;
        public InterestState Interest;
        public int CarIdx;
    }

    public class RemovalEdits
    {
        readonly List<OverlayData.RaceEvent> raceEvents;
        readonly List<InterestLevel> interestLevels = new List<InterestLevel>();

        InterestState lastInterest = 0;
        int carIdx = -1;

        public RemovalEdits(OverlayData overlayData)
        {
            this.raceEvents = overlayData.RaceEvents;
        }

        public void InterestingThingHappend(InterestState interest, long carIdx)
        {
            InterestingThingHappend(interest, (int)carIdx);
        }

        public void InterestingThingHappend(InterestState interest, int carIdx)
        {
            if (interest > lastInterest)
            {
                lastInterest = interest;
                this.carIdx = carIdx;
            }
        }

        public void Process(DataSample data, TimeSpan relativeTime)
        {
            interestLevels.Add(new InterestLevel { Time = relativeTime, Interest = lastInterest, CarIdx = carIdx });
            lastInterest = 0;
        }

        public void Stop()
        {
            foreach (var re in GetRaceEvents())
                raceEvents.Add(re);
        }

        IEnumerable<OverlayData.RaceEvent> GetRaceEvents()
        {
            InterestState lastInterest = InterestState.None;
            int lastCarIdx = -2;
            TimeSpan lastTime = new TimeSpan();
            OverlayData.RaceEvent raceEvent = null;
            
            foreach (var il in interestLevels)
            {
                lastTime = il.Time;

                if (lastInterest != il.Interest || lastCarIdx != il.CarIdx)
                {
                    if (raceEvent != null)
                    {
                        raceEvent.EndTime = il.Time.TotalSeconds;
                        yield return raceEvent;
                    }
                    raceEvent = new OverlayData.RaceEvent { StartTime = il.Time.TotalSeconds, Interest = il.Interest, CarIdx = il.CarIdx };
                }

                lastInterest = il.Interest;
                lastCarIdx = il.CarIdx;
            }

            raceEvent.EndTime = lastTime.TotalSeconds;
            yield return raceEvent;
        }
    }
}
