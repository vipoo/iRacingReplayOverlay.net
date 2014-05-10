using NUnit.Framework;
using System;
using iRacingReplayOverlay.Phases;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using iRacingSDK;
using System.Collections.Generic;
using System.Linq;

namespace iRacingReplayOverlay.net.Tests
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void TestCase()
		{
			Settings.Default.trackCameras = new TrackCameras() 
			{
				new TrackCamera { CameraName = "TV1", CameraNumber = 1, Ratio = 34, TrackName = "Mount Panorama Circuit" },
				new TrackCamera { CameraName = "TV2", CameraNumber = 1, Ratio = 33, TrackName = "Mount Panorama Circuit" },
				new TrackCamera { CameraName = "TV3", CameraNumber = 1, Ratio = 33, TrackName = "Mount Panorama Circuit" },
			};

            if (File.Exists("./dummy.xml"))
                File.Delete("./dummy.xml");

			File.WriteAllText("./dummy.avi", "Dummy video file");

			var ir = new IRacingReplay();
			ir.incidents = new iRacingReplayOverlay.Phases.Analysis.Incidents();

			ir._CaptureRaceTest("./", (a, b) =>
			{
			}, GetTestData());

			Assert.IsTrue(File.Exists("./dummy.xml"));
		}


		IEnumerable<DataSample> GetTestData()
		{
			var formatter = new BinaryFormatter();
			using(var stream = new FileStream("Support/SampleRaceStream.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				while(stream.Position < stream.Length)
				{
					var sample = (DataSample)formatter.Deserialize(stream);
                    sample.Telemetry.SessionData = sample.SessionData;
                   
					yield return sample;
				}
			}
		}
	}
}

