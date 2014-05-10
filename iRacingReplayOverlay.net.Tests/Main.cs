using System;
using iRacingSDK;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace iRacingReplayOverlay.net.Tests
{
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

			iRacing.Replay.MoveToStartOfRace();//.MoveToFrame(79466);
			iRacing.Replay.SetSpeed(16);

			var formatter = new BinaryFormatter();
			using( var  stream = new FileStream("SampleRaceStream.bin", FileMode.Create, FileAccess.Write, FileShare.None) )

				foreach(var data in iRacing.GetDataFeed().AtSpeed(16).TakeWhile( d => d.Telemetry.ReplayFrameNumEnd > 0 ))
				{
					if(data.LastSample != null && data.Telemetry.Lap != data.LastSample.Telemetry.Lap)
						Console.WriteLine("Capturing Lap {0}", data.Telemetry.Lap);

					formatter.Serialize(stream, data);
				}

			Console.WriteLine("Done!");
		}
	}
}

