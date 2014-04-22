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

            foreach( var data in iRacing.GetDataFeed().WithCorrectedPercentages().AtSpeed(16).RaceOnly())
            {
                if( sampleData == null )
                    sampleData = data;

                gapsToLeader.Process(data);
                positionChanges.Process(data);
            }

            foreach( var p in positionChanges.LapDeltas )
            {
                Console.WriteLine("On lap {0}", p.Lap);

                foreach( var d in p.DeltaDetails)
                {
                    Console.WriteLine("  car {0}, {1}, {2}", DriverNameFor(d.CarIdx), d.Delta, d.NewPosition);
                }
            }

            foreach( var gap in gapsToLeader.GapsByLaps )
            {
                Console.WriteLine("Lap {0}", gap.Lap);

                foreach( var kv in gap.GapsByCarIndex)
                {
                    Console.WriteLine("   Car {0} - Gap {1}", DriverNameFor(kv.Key), kv.Value);
                }
            }
        }

        static string DriverNameFor(int index)
        {
            return sampleData.SessionData.DriverInfo.Drivers[index].UserName;
        }
    }
}
