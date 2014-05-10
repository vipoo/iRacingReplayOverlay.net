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
//

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Direction;
using iRacingSDK;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using iRacingReplayOverlay.Support;
using System.Collections.Generic;

namespace iRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        void _CaptureRace(string workingFolder, Action<string, string> onComplete)
		{
			_CaptureRaceTest(workingFolder, onComplete, iRacing.GetDataFeed());
		}

		internal void _CaptureRaceTest(string workingFolder, Action<string, string> onComplete, IEnumerable<DataSample> samples)
		{
			iRacing.Replay.MoveToFrame(raceStartFrameNumber);

            Thread.Sleep(2000);
            iRacing.Replay.SetSpeed(1);

            var overlayData = new OverlayData();
            var removalEdits = new RemovalEdits(overlayData);
            var commentaryMessages = new CommentaryMessages(overlayData);
            var videoCapture = new VideoCapture();
            var capture = new Capture(overlayData, commentaryMessages, removalEdits, workingFolder);
            var fastestLaps = new RecordFastestLaps(overlayData);
            var replayControl = new ReplayControl(iRacing.GetDataFeed().First().SessionData, incidents, commentaryMessages, removalEdits);

            Thread.Sleep(2000);
            videoCapture.Activate();
            var startTime = DateTime.Now;

			foreach (var data in samples
                .WithCorrectedPercentages()
                .WithCorrectedDistances()
                .WithFinishingStatus()
                //.AtSpeed(3)
                //.AtSpeed(4, d => d.Telemetry.RaceLaps < 3)
                //.AtSpeed(16, d => d.Telemetry.RaceLaps >=0 && d.Telemetry.RaceLaps < 19)
                //.AtSpeed(6, d => d.Telemetry.RaceLaps >=19 )
)            {
                var relativeTime = DateTime.Now - startTime;

                capture.Process(data, relativeTime);
                fastestLaps.Process(data, relativeTime);
                if (replayControl.Process(data))
                    break;
                removalEdits.Process(data, relativeTime);
            }

            removalEdits.Stop();

            videoCapture.Deactivate();

            iRacing.Replay.SetSpeed(0);

            string errorMessage;
            string fileName;
            capture.Stop(out fileName, out errorMessage);

            var hwnd = Win32.Messages.FindWindow(null, "iRacing.com Simulator");
            Win32.Messages.ShowWindow(hwnd, Win32.Messages.SW_FORCEMINIMIZE);
            
            onComplete(fileName, errorMessage);
        }
    }
}
