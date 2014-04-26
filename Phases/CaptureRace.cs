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
using System.Linq;
using System.Threading;

namespace IRacingReplayOverlay.Phases
{
    public partial class IRacingReplay
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        const int KEYEVENTF_KEYUP = 0x02;
        const byte VK_MENU = 0x12;
        const byte VK_F9 = 0x78;

        void _CaptureRace(string workingFolder, Action<string, string> onComplete)
        {
            var overlayData = new OverlayData();
            
            iRacing.Replay.MoveToFrame(raceStartFrameNumber);
            Thread.Sleep(1000);
            iRacing.Replay.SetSpeed(1);

            var capture = new Capture(overlayData, workingFolder);
            var fastestLaps = new RecordFastestLaps(overlayData);
            var replayControl = new ReplayControl(iRacing.GetDataFeed().First().SessionData);
            
            Thread.Sleep(2000);
            ActivateExternalVideoCapture();
            var startTime = DateTime.Now;

            foreach (var data in iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(2)
                .TakeWhile(d => d.Telemetry.RaceLaps < 5))
            {
                var relativeTime = DateTime.Now - startTime;

                replayControl.Process(data);
                capture.Process(data, relativeTime);
                fastestLaps.Process(data, relativeTime);
            }

            DeactivateVideoCapture();

            string errorMessage;
            string fileName;
            capture.Stop(out fileName, out errorMessage);

            onComplete(fileName, errorMessage);
        }

        private static void DeactivateVideoCapture()
        {
            keybd_event(VK_F9, 0, 0, UIntPtr.Zero);
            Thread.Sleep(500);
            keybd_event(VK_F9, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private static void ActivateExternalVideoCapture()
        {
            keybd_event(VK_F9, 0, 0, UIntPtr.Zero);
            Thread.Sleep(500);
            keybd_event(VK_F9, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
