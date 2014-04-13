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

using System;
using System.Threading;
using iRacingSDK;
using System.IO;
using System.Linq;

namespace iRacingReplayOverlay.net
{
	class IRacingCapture : IDisposable
	{
		bool captureOn = false;
		static Thread worker = null;
		static bool workerStopRequest = false;

		public bool Toogle()
		{
			captureOn = !captureOn;

			if(captureOn)
				StartCapture();
			else
				StopCapture();

			return captureOn;
		}

		public void Dispose()
		{
			var w = worker;
			if(w == null)
				return;

			workerStopRequest = true;
			if(!w.Join(500))
			{
				w.Abort();
				throw new Exception("Capture thread did not shutdown cleanly");
			}
		}

		void StartCapture()
		{
			if(worker != null)
				throw new Exception("Capture thread already running");

			workerStopRequest = false;
			worker = new Thread(Loop);
			worker.Start();
		}

		static void Loop()
		{
			try
			{
				var iRacing = new DataFeed();
				if(!iRacing.Connect())
					throw new Exception("Unable to connect to iRacing server"); //TODO: Exception gets lost

				using(var file = File.CreateText(@"C:\users\dean\documents\leaders-table.csv"))
				{
					var startTime = DateTime.Now;

					file.WriteLine("StartTime, Drivers");
					foreach(var data in iRacing.Feed)
					{
                        var timeNow = DateTime.Now - startTime;

                        var numberOfDrivers = data.SessionInfo.DriverInfo.Drivers.Length;

                        var positions = data.Telementary.Cars
                            .Take(numberOfDrivers)
                            .Where(c => c.Index != 0)
                            .OrderByDescending(c => c.Lap + c.DistancePercentage);

                        var drivers = String.Join(",", positions.Select(c => c.Driver.UserName).ToArray());

                        file.WriteLine(timeNow.Seconds.ToString() + ",\"" + drivers + "\"");
                        Console.WriteLine(timeNow.Seconds.ToString() + ",\"" + drivers + "\"");

						for(int i = 0; i < 1000; i++)
						{
							if( workerStopRequest )
								return;
							Thread.Sleep(1);
						}

					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Error in worker " + e.Message);
				throw e;
			}
			finally
			{
				worker = null;
			}
		}

		void StopCapture()
		{
			if(worker == null)
				throw new Exception("Capture thread not running");

			workerStopRequest = true;
			if(!worker.Join(500))
			{
				worker.Abort();
				throw new Exception("Capture thread did not shutdown cleanly");
			}
			worker = null;
		}
	}
}
