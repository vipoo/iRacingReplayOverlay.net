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
        static readonly TimeSpan HighlightVideoDuration = 10.Minutes();

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
            TraceInfo.WriteLine("Highlight Edits: Total Duration Target: {0}", HighlightVideoDuration);

            var middleTime = (raceEvents.Last().EndTime - raceEvents.First().StartTime) /2;

            double totalTime, incidentsRatio, restartsRatio, battlesRatio;
            var firstAndLastLapRaceEvents = GetAllFirstAndLastLapEvents(raceEvents, out totalTime);
            var incidentRaceEvents = GetAllRaceEvents(raceEvents, middleTime, InterestState.Incident, 1.8, out incidentsRatio);
            var restartRaceEvents = GetAllRaceEvents(raceEvents, middleTime, InterestState.Restart, 1.0, out restartsRatio);
            var battleRaceEvents = GetAllRaceEvents(raceEvents, middleTime, InterestState.Battle, 1.4, out battlesRatio);
            
            var totalRatio = incidentsRatio + restartsRatio + battlesRatio;

            var incidentPercentage = incidentsRatio / totalRatio;
            var restartPercentage = restartsRatio / totalRatio;
            var battlePercentage = battlesRatio / totalRatio;

            var incidentsEdited = ExtractEditedEvents(totalTime, incidentPercentage, incidentRaceEvents, InterestState.Incident);
            var restartsEdited = ExtractEditedEvents(totalTime, restartPercentage, restartRaceEvents, InterestState.Restart);
            var battlessEdited = ExtractEditedEvents(totalTime, battlePercentage, battleRaceEvents, InterestState.Battle);

            var editedEvents = firstAndLastLapRaceEvents.Concat(incidentsEdited).Concat(restartsEdited).Concat(battlessEdited).OrderBy(re => re.StartTime);

            TraceInfo.WriteLine("Highlight Edits: Expected duration of highlight video: {0}", editedEvents.Sum(re => re.Duration).Seconds());

            return editedEvents;
        }

        static List<OverlayData.RaceEvent> GetAllFirstAndLastLapEvents(IEnumerable<OverlayData.RaceEvent> raceEvents, out double totalTime)
        {
            var firstAndLastLapRaceEvents = raceEvents
                .Where(re => re.Interest == InterestState.FirstLap || re.Interest == InterestState.LastLap)
                .ToList();
            var firstAndLastLapDuration = firstAndLastLapRaceEvents.Sum(re => re.Duration);
            totalTime = HighlightVideoDuration.TotalSeconds - firstAndLastLapDuration;

            TraceInfo.WriteLine("Highlight Edits: First & last laps.  Duration: {0}", firstAndLastLapDuration.Seconds());
        
            return firstAndLastLapRaceEvents;
        }

        static List<OverlayData.RaceEvent> ExtractEditedEvents(double totalTime, double percentage, List<_Accumulation<OverlayData.RaceEvent, double>> raceEvents, InterestState interest)
        {
            var targetDuration = totalTime * percentage;
            var result = raceEvents.TakeWhile(re => re.Accumulation < targetDuration).Select(re => re.Value).ToList();

            var duration = result.Sum(re => re.Duration);
            TraceInfo.WriteLine("Highlight Edits: {0}.  Target Duration: {1}, Percentage: {2:00}%, Resolved Duration: {3}",
                interest.ToString(), targetDuration.Seconds(), (int)(percentage*100), duration.Seconds());

            return result;
        }

        static List<_Accumulation<OverlayData.RaceEvent, double>> GetAllRaceEvents(IEnumerable<OverlayData.RaceEvent> raceEvents, double middleTime, InterestState interest, double factor, out double ratio)
        {
            var result = raceEvents
                .Where(re => re.Interest == interest)
                .OrderByDescending(re => Math.Abs(re.StartTime - middleTime))
                .Accumulate(0d, (a, re) => a + re.Duration)
                .ToList();
        
            var duration = result.Sum(re => re.Value.Duration);
            ratio = duration * factor;

            TraceInfo.WriteLine("Highlight Edits: {0}.  Duration: {1}, Factor: {2}, Ratio: {3}", interest.ToString(), duration.Seconds(), factor, ratio);

            return result;
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
