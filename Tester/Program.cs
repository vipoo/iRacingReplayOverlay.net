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

using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Direction;
using IRacingReplayOverlay.Phases.Direction;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tester
{
    class Program
    {
        static DataSample sampleData;

        static void Main(string[] args)
        {
            foreach (var data in iRacing.GetDataFeed().TakeWhile(d => !d.IsConnected))
            {
                Console.WriteLine("Waiting to connect ...");
                continue;
            }

            GetInterestingCarsByLap();
        }

        public static void GetInterestingCarsByLap()
        {
            var gapsToLeader = new GapsToLeader();
            var positionChanges = new PositionChanges();
            var lapsToFrameNumbers = new LapsToFrameNumbers();

            foreach( var data in iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(16)
                .RaceOnly()
                .TakeWhile( d => d.Telemetry.RaceLaps < 4))
            {
                if( sampleData == null )
                    sampleData = data;

                gapsToLeader.Process(data);
                positionChanges.Process(data);
                lapsToFrameNumbers.Process(data);
            }

			var replayControl = ReplayControlFactory.CreateFrom(gapsToLeader, positionChanges, lapsToFrameNumbers);

            
            Console.WriteLine("Press any watch race");
            Console.ReadLine();

            replayControl.DirectReplay();

        }

        static string DriverNameFor(int index)
        {
            return sampleData.SessionData.DriverInfo.Drivers[index].UserName;
        }
    }
}
