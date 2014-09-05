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

using iRacingSDK;
using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Phases.Direction.Support
{
    public static class Battle
    {
        static readonly Random random = new Random();

        public struct GapMetric
        {
            public int CarIdx;
            public double Time;
            public int Position;

            public override string ToString()
            {
                return "CarIdx: {0}, Time: {1}, Position: {2}".F(CarIdx, Time, Position);
            }
        }

        public static SessionData._DriverInfo._Drivers Find(DataSample data, TimeSpan battleGap, double factor)
        {
            var allBattles = All(data, battleGap);

            return SelectABattle(data, allBattles, random.Next(101), factor);
        }

        internal static IEnumerable<GapMetric> All(DataSample data, TimeSpan battleGap)
        {
            var distances = data.Telemetry.CarIdxDistance
                    .Select((d, i) => new { CarIdx = i, Distance = d })
                    .Skip(1)
                    .Where(d => d.Distance > 0)
                    .OrderByDescending(d => d.Distance)
                    .ToList();

            if (distances.Count == 0)
                return new GapMetric[0];

            var gap = Enumerable.Range(1, distances.Count - 1)
                    .Select(i => new
                    {
                        CarIdx = distances[i].CarIdx,
                        Distance = distances[i - 1].Distance - distances[i].Distance,
                        Position = i
                    });

            var timeGap = gap.Select(g => new GapMetric
            {
                CarIdx = g.CarIdx,
                Time = g.Distance * data.Telemetry.Session.ResultsAverageLapTime,
                Position = g.Position
            });

            return timeGap
                .OrderByDescending(d => d.Position)
                .Where(d => d.Time < battleGap.TotalSeconds);
        }

        internal static SessionData._DriverInfo._Drivers SelectABattle(DataSample data, IEnumerable<GapMetric> all, int dice, double factor )
        {
            if (all.Count() == 0)
                return null;

            var numberOfX = Math.Pow(2d, all.Count() * factor) - factor;
            var x = 95.0 / (double)numberOfX;
            var ddice = (double)dice;

            if (ddice < 5.0)
            {
                TraceInfo.WriteLine("{0} Ignoring active battles", data.Telemetry.SessionTimeSpan);
                return null;
            }

            var divide = 5.0;
            var currentFactor = factor;

            foreach (var battle in all)
            {
                var upper = ((Math.Pow(2, currentFactor) - factor) * x) + 5.0;

                if (ddice < upper)
                {
                    var driver = data.SessionData.DriverInfo.Drivers[battle.CarIdx];
                    TraceInfo.WriteLine("{0} Found battle {1} by chance {2}", data.Telemetry.SessionTimeSpan, driver.UserName, ddice);
                    return driver;
                }

                divide = upper;
                currentFactor += factor;
            }
            
            Trace.WriteLine("WARNING!  did not find battle by chance!", "DEBUG");
            return data.SessionData.DriverInfo.Drivers[all.Last().CarIdx];
        }

        internal static bool IsInBattle(DataSample data, TimeSpan battleGap, SessionData._DriverInfo._Drivers follower, SessionData._DriverInfo._Drivers leader)
        {
            var leaderCar = data.Telemetry.Cars[leader.CarIdx];
            var followerCar = data.Telemetry.Cars[follower.CarIdx];

            if (leaderCar.Position == followerCar.Position + 1)
                return false;

            var timeGap = (leaderCar.TotalDistance - followerCar.TotalDistance) * data.Telemetry.Session.ResultsAverageLapTime;
            return timeGap < battleGap.TotalSeconds;
        }
    }
}
