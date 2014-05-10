using NUnit.Framework;
using System;
using iRacingReplayOverlay.Phases;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using iRacingSDK;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using iRacingReplayOverlay.Phases.Capturing;
using YamlDotNet.Serialization;

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

            var reader = new XmlSerializer(typeof(OverlayData));
            using (var file = new StreamReader("./dummy.xml"))
            {
                var result = (OverlayData)reader.Deserialize(file);

                //result.MessageStates.First(d => d.Messages.First() == "Dean ").Time;
            }
		}


		IEnumerable<DataSample> GetTestData()
		{
			var formatter = new BinaryFormatter();
            formatter.Binder = new MyBinder(formatter.Binder);
			using(var stream = new FileStream("Support/SampleRaceStream.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
                var rawSessionData = (string)formatter.Deserialize(stream);
                var telemetryKeys = (string[])formatter.Deserialize(stream);
                var sessionData = BuildSessionData(rawSessionData);
                DataSample lastDataSample = null;

				while(stream.Position < stream.Length)
				{
					var telemetryValues = (object[])formatter.Deserialize(stream);

                    var telemetry = new Telemetry();
                    for(int i = 0; i < telemetryKeys.Length; i++)
                        telemetry.Add(telemetryKeys[i], telemetryValues[i]);
                    telemetry.SessionData = sessionData;

                    var sample = new DataSample
                    {
                        Telemetry = telemetry,
                        SessionData = sessionData,
                        LastSample = lastDataSample
                    };

                    lastDataSample = sample;
                   
					yield return sample;

                    sample.LastSample = null;
				}
			}
		}

        private SessionData BuildSessionData(string rawSessionData)
        {
            var input = new StringReader(rawSessionData);

            var deserializer = new Deserializer(ignoreUnmatched: true);

            var result = (SessionData)deserializer.Deserialize(input, typeof(SessionData));
            result.Raw = rawSessionData;

            return result;
        }
	}
}

