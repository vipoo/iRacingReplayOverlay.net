using System;
using iRacingSDK;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace iRacingReplayOverlay.net.Tests
{
    [Serializable]
    public struct TestDataSample
    {
        public string YamlSessionData;
        public string[] TelemetryKeys;
        public object[] TelemetryValues;
    }

    public class Support
    {
        public static void Main()
        {
            new Test().TestCase();

            //CaptureAFullReplay();
        }


        static void CaptureAFullReplay()
        {
            var samples = new List<DataSample>();

            iRacing.Replay.MoveToStartOfRace();
            iRacing.Replay.SetSpeed(16);

            var formatter = new BinaryFormatter();
            var firstSampleDone = false;

            using (var stream = new FileStream("./SampleRaceStream.bin", FileMode.Create, FileAccess.Write, FileShare.None))
                foreach (var data in iRacing.GetDataFeed().AtSpeed(16).TakeWhile(d => d.Telemetry.ReplayFrameNumEnd > 40))
                {
                    if (data.LastSample != null && data.Telemetry.Lap != data.LastSample.Telemetry.Lap)
                        Console.WriteLine("Capturing Lap {0}", data.Telemetry.Lap);

                    if( !firstSampleDone)
                    {
                        firstSampleDone = true;
                        formatter.Serialize(stream, data.SessionData.Raw);
                        formatter.Serialize(stream, data.Telemetry.Select(kv => kv.Key).ToArray());
                    }
                    
                    formatter.Serialize(stream, data.Telemetry.Select(kv => kv.Value).ToArray());
                }

            Console.WriteLine("Done!");
        }
    }
}

