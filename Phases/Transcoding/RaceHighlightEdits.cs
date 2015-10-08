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
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.

using iRacingReplayOverlay.Phases.Capturing;
using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    public static class RaceEventExtension
    {
        static TimeSpan HighlightVideoDuration
        {
            get { return (Settings.Default.HighlightVideoTargetDuration.TotalMinutes / Settings.Default.TimingFactorForTesting).Minutes(); }
        }

        public static List<VideoEdit> GetRaceEdits(this IEnumerable<OverlayData.RaceEvent> raceEvents)
        {
            var edits = raceEvents._GetRaceEdits().ToList();

            foreach (var e in edits)
                TraceInfo.WriteLine("Editing from {0} to {1}. Duration {2}", e.StartTimeSpan, e.EndTimeSpan, e.Duration);

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
                    yield return new VideoEdit { StartTime = previousEvent.EndTime - 1, EndTime = re.StartTime + 1 };

                previousEvent = re;
            }
        }

        private static IOrderedEnumerable<OverlayData.RaceEvent> GetInterestingRaceEvents(IEnumerable<OverlayData.RaceEvent> raceEvents)
        {
            TraceInfo.WriteLine("Highlight Edits: Total Duration Target: {0}", HighlightVideoDuration);

            double totalTime, incidentsRatio, restartsRatio, battlesRatio;
            var firstAndLastLapRaceEvents = GetAllFirstAndLastLapEvents(raceEvents, out totalTime);
            
            var incidentRaceEvents = GetAllRaceEvents(raceEvents, InterestState.Incident, 1.8, 0, out incidentsRatio);
            var restartRaceEvents = GetAllRaceEvents(raceEvents, InterestState.Restart, 1.0, 0, out restartsRatio);
            var battleRaceEvents = GetAllRaceEvents(raceEvents, InterestState.Battle, 1.4, 15, out battlesRatio);

            battleRaceEvents = NormaliseBattleEvents(battleRaceEvents, Settings.Default.BattleStickyPeriod.TotalSeconds);

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

        private static List<OverlayData.RaceEvent> NormaliseBattleEvents(List<OverlayData.RaceEvent> raceEvents, double maxDuration)
        {
            var result = new List<OverlayData.RaceEvent>();

            foreach( var re in raceEvents)
            {
                if (re.Duration < maxDuration)
                    result.Add(re);
                else
                {
                    var segmentCount = (int)(re.Duration / maxDuration) + 1;
                    var segmentDuration = re.Duration / segmentCount;
                    var startTime = re.StartTime;

                    for (var i = 0; i < segmentCount; i++)
                    {
                        var segment = new OverlayData.RaceEvent { Interest = re.Interest, StartTime = startTime, EndTime = startTime + segmentDuration };
                        result.Add(segment);
                        startTime += segmentDuration;
                    }
                }
            }

            return result;
        }

        static List<OverlayData.RaceEvent> GetAllFirstAndLastLapEvents(IEnumerable<OverlayData.RaceEvent> raceEvents, out double totalTime)
        {
            var firstAndLastLapRaceEvents = raceEvents
                .Where(re => re.Interest == InterestState.FirstLap || re.Interest == InterestState.LastLap)
                .ToList();
            var firstAndLastLapDuration = firstAndLastLapRaceEvents.Sum(re => re.Duration);
            totalTime = HighlightVideoDuration.TotalSeconds - firstAndLastLapDuration;

            TraceInfo.WriteLine("Highlight Edits: First & last laps.  Duration: {0}. Remaining: {1}", firstAndLastLapDuration.Seconds(), totalTime.Seconds());
        
            return firstAndLastLapRaceEvents;
        }

        static void SliceEvent(List<_RaceEvent> result, List<OverlayData.RaceEvent> raceEvents, OverlayData.RaceEvent left, OverlayData.RaceEvent right, int level = 1)
        {
            var middleTime = (left.EndTime + right.StartTime) / 2;

            var middleEvent = raceEvents
                .Where( r => r.StartTime >= left.StartTime && r.StartTime <= right.EndTime)
                .OrderBy(r => Math.Abs(middleTime - r.StartTime)).FirstOrDefault();

            if( middleEvent == null )
                return;

            result.Add(new _RaceEvent { RaceEvent = middleEvent, Level = level });
            raceEvents.Remove(middleEvent);

            SliceEvent(result, raceEvents, left, middleEvent, level + 1);
            SliceEvent(result, raceEvents, middleEvent, right, level + 1);
        }

        static List<OverlayData.RaceEvent> ExtractEditedEvents(double totalTime, double percentage, List<OverlayData.RaceEvent> raceEvents, InterestState interest)
        {
            var duration = 0d;

            var orderRaceEvents = new List<_RaceEvent>();

            var targetDuration = totalTime * percentage;

            raceEvents = raceEvents.ToArray().ToList();

            if( raceEvents.Count <= 2 )
                return raceEvents;

            var firstEvent = raceEvents.OrderBy(r => r.StartTime).First();
            var lastEvent = raceEvents.OrderBy(r => r.StartTime).Last();

            orderRaceEvents.Add(new _RaceEvent { RaceEvent = firstEvent, Level = 0 });
            orderRaceEvents.Add(new _RaceEvent { RaceEvent = lastEvent, Level = 0 });

            var result = new List<OverlayData.RaceEvent>();

            SliceEvent(orderRaceEvents, raceEvents.Where( rc => rc.WithOvertake).ToList(), firstEvent, lastEvent, 1);
            duration = SelectOrderByLevel(orderRaceEvents, targetDuration, result, duration);

            orderRaceEvents.Clear();

            SliceEvent(orderRaceEvents, raceEvents.Where(rc => !rc.WithOvertake).ToList(), firstEvent, lastEvent, 1);
            duration = SelectOrderByLevel(orderRaceEvents, targetDuration, result, duration);

            foreach( var r in result.OrderBy( x => x.StartTime))
                TraceDebug.WriteLine("Highlight edit {0}: {1} - {2}, duration: {3}", r.Interest, r.StartTime.Seconds(), r.EndTime.Seconds(), r.Duration.Seconds());

            TraceInfo.WriteLine("Highlight Edits: {0}.  Target Duration: {1}, Percentage: {2:00}%, Resolved Duration: {3}",
                interest.ToString(), targetDuration.Seconds(), (int)(percentage*100), duration.Seconds());

            return result;
        }

        private static double SelectOrderByLevel(List<_RaceEvent> orderRaceEvents, double targetDuration, List<OverlayData.RaceEvent> result, double duration)
        {
            foreach (var re in orderRaceEvents
                .OrderBy(r => r.Level)
                .ThenBy(r => r.RaceEvent.StartTime)
                .Select(re => re.RaceEvent))
            {
                duration = result.Sum(r => r.Duration);
                if (duration > targetDuration)
                    break;

                if (duration + re.Duration < targetDuration)
                    result.Add(re);
            }
            return duration;
        }

        static List<OverlayData.RaceEvent> GetAllRaceEvents(IEnumerable<OverlayData.RaceEvent> raceEvents, InterestState interest, double factor, double minDuration, out double ratio)
        {
            var result = new List<OverlayData.RaceEvent>(); //Attention risque de problème sur cette ligne, à tester.

            if (Settings.Default.FocusOnPreferedDriver == true) // if focusing on prefered drivers, taking only battles with prefered drivers
            {
                result = raceEvents
                    //.Where(re => re.Interest == interest)
                    .Where(re => re.Interest == interest && re.withPreferedDriver == true) // prefered driver
                    .Where(re => re.Duration > minDuration)
                    .ToList();
            }
            else //Standard behaviour if not focusing.
            {
                result = raceEvents
                    .Where(re => re.Interest == interest)
                    .Where(re => re.Duration > minDuration)
                    .ToList();
            }

            var duration = result.Sum(re => re.Duration);
            ratio = duration * factor;

            TraceInfo.WriteLine("Highlight Edits: {0}.  Duration: {1}, Factor: {2}, Ratio: {3}", interest.ToString(), duration.Seconds(), factor, ratio);

            return result;
        }

        struct _RaceEvent
        {
            public OverlayData.RaceEvent RaceEvent;
            public int Level;
        }
    }
}
