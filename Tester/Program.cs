using iRacingReplayOverlay.net;
using iRacingReplayOverlay.Phases.Analysis;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .TakeWhile( d => d.Telemetry.RaceLaps < 7))
            {
                if( sampleData == null )
                    sampleData = data;

                gapsToLeader.Process(data);
                positionChanges.Process(data);
                lapsToFrameNumbers.Process(data);
            }

            var result = new Dictionary<int, int>(); //frameNum, CarIdx

            var replayControl = new ReplayControl(iRacing.GetDataFeed().First().SessionData);

            foreach (var lap in lapsToFrameNumbers.Skip(1))
            {
                var change = positionChanges[lap.LapNumber].DeltaDetails.FirstOrDefault(d => d.Delta > 0);
                if( change != null )
                {
                    Console.WriteLine("Watching {0} overtake on lap {1}", DriverNameFor(change.CarIdx), lap.LapNumber);

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];
                    replayControl.AddCarChange(frameNumber, change.CarIdx, "TV1");
                }
                else
                {
                    var gaps = gapsToLeader[lap.LapNumber];
                    var carIdx = gaps.GapsByCarIndex.OrderBy(g => g.Value).First().Key;

                    var frameNumber = lapsToFrameNumbers[lap.LapNumber - 1];

                    replayControl.AddCarChange(frameNumber, change.CarIdx, "TV1"); 

                    Console.WriteLine("Watching {0} on lap {1}", DriverNameFor(carIdx), lap.LapNumber);
                }
            }

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
