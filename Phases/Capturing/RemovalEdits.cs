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
using iRacingSDK.Support;
using System;
using System.Collections.Generic;

namespace iRacingReplayOverlay.Phases.Capturing
{
    public enum InterestState { None = 0, FirstLap = 1, LastLap = 2, Incident = 3, Restart = 4, Battle = 5 /*, Overtake = 5, Pitstp = 6 */};

    public class RemovalEdits
    {
        readonly List<OverlayData.RaceEvent> raceEvents;

        Action<DataSample, TimeSpan> nextAction = (d, t) => { };
        TimeSpan lastRelativeTime;
        TimeSpan lastStartTime;

        readonly Stack<Tuple<InterestState, int>> events = new Stack<Tuple<InterestState, int>>();

        public RemovalEdits(List<OverlayData.RaceEvent> raceEvents)
        {
            this.raceEvents = raceEvents;
        }

        public void InterestingThingStarted(InterestState interest, long carIdx)
        {
            InterestingThingStarted(interest, (int)carIdx);
        }

        public void InterestingThingStarted(InterestState interest, int carIdx)
        {
            nextAction = (d, t) =>
            {
                if (events.Count > 0)
                {
                    var le = events.Peek();

                    AddEvent(le.Item1, le.Item2, d, t);
                }
                events.Push(Tuple.Create(interest, carIdx));

                lastStartTime = t;
                TraceInfo.WriteLine("{0} Starting {1}", d.Telemetry.SessionTimeSpan, interest.ToString());
            };
        }

        public void InterestingThingStopped(InterestState interest, long carIdx)
        {
            InterestingThingStopped(interest, (int)carIdx);
        }
        
        public void InterestingThingStopped(InterestState interest, int carIdx)
        {
            nextAction = (d, t) =>
            {
                var le = events.Pop();
                if (le.Item1 != interest)
                    throw new Exception("RaceEvent mismatched.  Attempted to close {0}, when expecting {1}".F(interest.ToString(), le.Item1.ToString()));

                if( le.Item2 != carIdx)
                    throw new Exception("RaceEvent mismatched.  Attempted to close carIdx: {0}, when expecting carIdx: {1}".F(carIdx, le.Item2));

                t = AddEvent(interest, carIdx, d, t);
            };
        }

        private TimeSpan AddEvent(InterestState interest, int carIdx, DataSample d, TimeSpan t)
        {
            raceEvents.Add(new OverlayData.RaceEvent { CarIdx = carIdx, Interest = interest, StartTime = lastStartTime.TotalSeconds, EndTime = t.TotalSeconds });
            lastStartTime = t;

            TraceInfo.WriteLine("{0} Stopping {1}", d.Telemetry.SessionTimeSpan, interest.ToString());
            return t;
        }
        
        public void Process(DataSample data, TimeSpan relativeTime)
        {
            nextAction(data, relativeTime);
            nextAction = (d, t) => { };
            lastRelativeTime = relativeTime;
        }

        public void Stop()
        {
            raceEvents.Add(new OverlayData.RaceEvent 
            {
                CarIdx = -1, 
                Interest = InterestState.LastLap, 
                StartTime = lastStartTime.TotalSeconds, 
                EndTime = lastRelativeTime.TotalSeconds
            });
        }
    }
}
