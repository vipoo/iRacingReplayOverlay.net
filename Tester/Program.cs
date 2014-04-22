using iRacingReplayOverlay.net.LapAnalysis;
using iRacingSDK;
using System;
using System.Collections.Generic;
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
            }

            Console.WriteLine("Positional changes per lap:");
            foreach( var p in positionChanges.LapDeltas )
            {
                Console.WriteLine("On lap {0}", p.Lap);

                foreach( var d in p.DeltaDetails)
                {
                    Console.WriteLine("  car {0}, {1}, {2}", DriverNameFor(d.CarIdx), d.Delta, d.NewPosition);
                }
            }

            Console.WriteLine("\n\n\nGap between leader and drivers:");

            foreach( var gap in gapsToLeader.GapsByLaps )
            {
                Console.WriteLine("Lap {0}", gap.Lap);

                foreach( var kv in gap.GapsByCarIndex)
                {
                    Console.WriteLine("   Car {0} - Gap {1}", DriverNameFor(kv.Key), kv.Value);
                }
            }

            var result = new Dictionary<int, int>(); //Lap, CarIdx
            foreach( var p in positionChanges.LapDeltas)
            {
                var change = p.DeltaDetails.FirstOrDefault(d => d.Delta > 0);
                if( change != null )
                {
                    Console.WriteLine("Watching {0} overtake on lap {1}", DriverNameFor(change.CarIdx), p.Lap);
                    result.Add(p.Lap-1, change.CarIdx);
                }
                else
                {
                    var gaps = gapsToLeader.GapsByLaps.First(g => g.Lap == p.Lap);
                    var carIdx = gaps.GapsByCarIndex.OrderBy(g => g.Value).First().Key;
                    result.Add(p.Lap-1, carIdx);
                    Console.WriteLine("Watching {0} on lap {1}", DriverNameFor(carIdx), p.Lap);
                }
            }

            Console.WriteLine("Press any watch race");
            Console.ReadLine();

            var lastRaceLap = -1;

            iRacing.Replay.MoveToParadeLap();

            iRacing.Replay.CameraOnPositon(1, 13, 0);
            foreach (var data in iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(1)
                .TakeWhile(d => d.Telemetry.RaceLaps < 7))
            {
                if( lastRaceLap != data.Telemetry.RaceLaps)
                {
                    lastRaceLap = data.Telemetry.RaceLaps;

                    if(lastRaceLap < 2)
                        continue;

                    if( result.ContainsKey(lastRaceLap))
                    {
                        var carIdx = result[lastRaceLap];
                        var carNumber = sampleData.SessionData.DriverInfo.Drivers[carIdx].CarNumber;
                        Console.WriteLine("Change to driver {0}", DriverNameFor(carIdx));
                        iRacing.Replay.CameraOnDriver((short)carNumber, 11, 0);
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }

        }

        static string DriverNameFor(int index)
        {
            return sampleData.SessionData.DriverInfo.Drivers[index].UserName;
        }
    }
}
