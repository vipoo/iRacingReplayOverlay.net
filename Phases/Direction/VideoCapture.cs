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

using iRacingReplayOverlay.Support;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class VideoCapture
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        const int KEYEVENTF_KEYUP = 0x02;
        const byte VK_MENU = 0x12;
        const byte VK_F9 = 0x78;
        string workingFolder;
        private DateTime started;

        public void Activate(string workingFolder)
        {
            this.workingFolder = workingFolder;
            this.started = DateTime.Now; //.Subtract(TimeSpan.FromSeconds(30));

            SendKeyStroke();
        }

        public string Deactivate()
        {
            SendKeyStroke();

            Thread.Sleep(2000);

            var guessedFileName = Directory.GetFiles(workingFolder, "*.avi")
                .Concat(Directory.GetFiles(workingFolder, "*.mp4"))
                .Select(fn => new { FileName = fn, CreationTime = File.GetCreationTime(fn) })
                .Where( f => f.CreationTime >= started)
                .OrderByDescending(f => f.CreationTime)
                .FirstOrDefault();

            Trace.WriteLineIf(guessedFileName == null, "Unable to determine video file name in '{0}' - possible wrong working folder".F(workingFolder), "INFO");
            return guessedFileName == null ? null : guessedFileName.FileName;
        }

        private static void SendKeyStroke()
        {
            Trace.WriteLine("Sending key event ALT+F9", "INFO");

            keybd_event(VK_MENU, 0, 0, UIntPtr.Zero);
            Thread.Sleep(200);
            keybd_event(VK_F9, 0, 0, UIntPtr.Zero);
            Thread.Sleep(200);
            keybd_event(VK_F9, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(200);
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

    }
}
