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


				foreach(var data in iRacing.Feed)
				{
					if( workerStopRequest )
						return;

					Console.WriteLine("Tick, Session Time: " + data["TickCount"] + ", " + data["SessionTime"]);

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
