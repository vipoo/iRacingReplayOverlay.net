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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Support;
using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    public static class RaceEventExtension
    {
        public static List<VideoEdit> GetRaceEdits(this IEnumerable<OverlayData.RaceEvent> raceEvents)
        {
            var edits = raceEvents._GetRaceEdits().ToList();

            foreach (var e in edits)
                TraceInfo.WriteLine("Editing from {0} to {1}", e.StartTimeSpan, e.EndTimeSpan);

            TraceInfo.WriteLine("Total Edits time {0}", edits.Sum(e => e.Duration).Seconds());

            return edits;
        }
        
        static IEnumerable<VideoEdit> _GetRaceEdits(this IEnumerable<OverlayData.RaceEvent> raceEvents)
        {
            var totalRaceEvents = GetInterestingRaceEvents(raceEvents);

            var previousEvent = totalRaceEvents.First();
            foreach (var re in totalRaceEvents.Skip(1))
            {
                if (re.StartTime - previousEvent.EndTime >= 10d)
                    yield return new VideoEdit { StartTime = previousEvent.EndTime, EndTime = re.StartTime };

                previousEvent = re;
            }
        }

        private static IOrderedEnumerable<OverlayData.RaceEvent> GetInterestingRaceEvents(IEnumerable<OverlayData.RaceEvent> raceEvents)
        {
            var middleTime = (raceEvents.Last().EndTime - raceEvents.First().StartTime) /2;

            var firstAndLastLapRaceEvents = raceEvents
                .Where(re => re.Interest == InterestState.FirstLap || re.Interest == InterestState.LastLap)
                .ToList();

            var totalTime = firstAndLastLapRaceEvents.Sum(re => re.Duration);
            var timeRemaing = (10.Minutes() - totalTime.Seconds()).TotalSeconds / 2;

            var incidentRaceEvents = GetRaceEvents(raceEvents, middleTime, timeRemaing, InterestState.Incident);
            totalTime = totalTime + incidentRaceEvents.Sum(re => re.Duration);
            timeRemaing = (10.Minutes() - totalTime.Seconds()).TotalSeconds;

            var restartRaceEvents = GetRaceEvents(raceEvents, middleTime, timeRemaing, InterestState.Restart);
            totalTime = totalTime + incidentRaceEvents.Sum(re => re.Duration);
            timeRemaing = (10.Minutes() - totalTime.Seconds()).TotalSeconds;

            var battleIncidents = GetRaceEvents(raceEvents, middleTime, timeRemaing, InterestState.Battle);

            return firstAndLastLapRaceEvents.Concat(incidentRaceEvents).Concat(restartRaceEvents).Concat(battleIncidents).OrderBy(re => re.StartTime);
        }

        static List<OverlayData.RaceEvent> GetRaceEvents(IEnumerable<OverlayData.RaceEvent> raceEvents, double middleTime, double timeRemaing, InterestState interest)
        {
            return raceEvents
                .Where(re => re.Interest == interest)
                .OrderByDescending(re => Math.Abs(re.StartTime - middleTime))
                .Accumulate(0d, (a, re) => a + re.Duration)
                .TakeWhile(re => re.Accumulation < timeRemaing)
                .Select(re => re.Value)
                .ToList();
        }

        struct _Accumulation<T1, T2>
        {
            public T1 Value;
            public T2 Accumulation;
        }

        static IEnumerable<_Accumulation<T1, T2>> Accumulate<T1, T2>(this IEnumerable<T1> list, T2 seed, Func<T2, T1, T2> fn)
        {
            T2 accumulator = seed;
            foreach (var i in list)
            {
                accumulator = fn(accumulator, i);
                yield return new _Accumulation<T1, T2> { Value = i, Accumulation = accumulator };
            }
        }
    }
}
