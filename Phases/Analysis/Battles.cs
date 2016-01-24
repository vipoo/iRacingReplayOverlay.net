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

using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Analysis
{
    public class Battles : IEnumerable<Battles.Battle>
    {
        public struct BattleKey
        {
            public int Car1Idx;
            public int Car2Idx;

            public BattleKey(int car1Idx, int car2Idx)
            {
                Car1Idx = Math.Min(car1Idx, car2Idx);
                Car2Idx = Math.Max(car1Idx, car2Idx);
            }
            
            public override int GetHashCode()
            {
                return Car1Idx * 100 + Car2Idx;
            }

            public override string ToString()
            {
                return "Car1Idx: {0}, Car2Idx: {1}".F(Car1Idx, Car2Idx);
            }
        }

        public class Battle
        {
            public TimeSpan StartSessionTime;
            public TimeSpan EndSessionTime;
            public BattleKey BattleKey;

            public bool IsInside(TimeSpan time)
            {
                return time >= StartSessionTime && time <= EndSessionTime;
            }

            public override string ToString()
            {
                return "StartSessionTime: {0}, EndSessionTime: {1}, {2}".F(StartSessionTime, EndSessionTime, BattleKey.ToString());
            }
        }

        List<Battle> battles = new List<Battle>();
        HashSet<BattleKey> previousBattleStates = new HashSet<BattleKey>();
        Dictionary<BattleKey, Battle> latestBattles = new Dictionary<BattleKey, Battle>();
        bool raceHasStarted;
        TimeSpan raceStartTime;

        public Battles()
        {
        }

        public void Process(DataSample data)
        {
            if (InFirstLapPeriod(data))
                return;

            foreach(var kv in latestBattles)
                kv.Value.EndSessionTime = data.Telemetry.SessionTimeSpan;

            var battles = Direction.Support.Battle.FindAll(data, Settings.Default.BattleGap, Settings.Default.BattleFactor2, new string[0]);
            
            var currentBattleStates = new HashSet<BattleKey>();
            foreach (var d in battles)
                currentBattleStates.Add(new BattleKey(d.LeaderCarIdx, d.CarIdx));

            var startedBattles = currentBattleStates.Except(previousBattleStates);
            var finishedBattles = previousBattleStates.Except(currentBattleStates);

            foreach(var b in startedBattles)
                if (!((latestBattles.ContainsKey(b) && latestBattles[b].EndSessionTime + 15.Seconds() > data.Telemetry.SessionTimeSpan)))
                    latestBattles[b] = new Battle { BattleKey = b, StartSessionTime = data.Telemetry.SessionTimeSpan, EndSessionTime = data.Telemetry.SessionTimeSpan };

            foreach(var b in finishedBattles)
                this.battles.Add(latestBattles[b]);

            var timeOutBattles = latestBattles.Values.Where(b => b.StartSessionTime + 120.Seconds() < data.Telemetry.SessionTimeSpan).ToList();

            foreach (var b in timeOutBattles)
                latestBattles[b.BattleKey] = new Battle { BattleKey = b.BattleKey, StartSessionTime = data.Telemetry.SessionTimeSpan, EndSessionTime = data.Telemetry.SessionTimeSpan };

            previousBattleStates = currentBattleStates;
        }

        bool InFirstLapPeriod(DataSample data)
        {
            if (!raceHasStarted)
            {
                raceHasStarted = data.Telemetry.SessionState == SessionState.Racing;
                raceStartTime = data.Telemetry.SessionTimeSpan;
                return true;
            }

            return data.Telemetry.SessionTimeSpan < raceStartTime + Settings.Default.FollowLeaderAtRaceStartPeriod;
        }
        public IEnumerator<Battles.Battle> GetEnumerator()
        {
            return battles.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return battles.GetEnumerator();
        }
    }
}

