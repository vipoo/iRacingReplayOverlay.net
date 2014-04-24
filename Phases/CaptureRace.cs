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
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            var capture = new Capture();

            iRacing.Replay.MoveToFrame(raceStartFrameNumber);
            iRacing.Replay.SetSpeed(1);
            Thread.Sleep(1000);
            keybd_event(VK_F9, 0, 0, UIntPtr.Zero);
            Thread.Sleep(500);
            keybd_event(VK_F9, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            replayControl.Start();
            capture.Start(workingFolder);

            foreach (var data in iRacing.GetDataFeed()
                .WithCorrectedPercentages()
                .AtSpeed(4)
                .TakeWhile(d => d.Telemetry.RaceLaps < 4))
            {
                replayControl.Process(data);
                capture.Process(data);
            }

            keybd_event(VK_F9, 0, 0, UIntPtr.Zero);
            Thread.Sleep(500);
            keybd_event(VK_F9, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            string errorMessage;
            string fileName;
            capture.Stop(out fileName, out errorMessage);

            onComplete(fileName, errorMessage);
        }
    }
}
