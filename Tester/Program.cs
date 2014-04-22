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
        static void Main(string[] args)
        {
            foreach (var data in iRacing.GetDataFeed().TakeWhile(d => !d.IsConnected))
            {
                Console.WriteLine("Waiting to connect ...");
                continue;
            }

            iRacing.Replay.MoveToStartOfRace(); 

            GetGapsToLeader();
        }

        public static void GetGapsToLeader()
        {
            var gapsToLeader = new GapsToLeader();

            foreach( var data in iRacing.GetDataFeed().WithCorrectedPercentages().AtSpeed(16).RaceOnly())
            {
                gapsToLeader.Process(data);
            }

            foreach( var gap in gapsToLeader.GapsByLaps )
            {
                Console.WriteLine("Lap {0}", gap.Lap);

                foreach( var kv in gap.GapsByCarIndex)
                {
                    Console.WriteLine("Car {0} - Gap {1}", kv.Key, kv.Value);
                }
            }
        }
    }
}
