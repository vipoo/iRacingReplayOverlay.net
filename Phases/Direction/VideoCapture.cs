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
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Win32;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class VideoCapture
    {
        string workingFolder;
        private DateTime started;

        public void Activate(string workingFolder)
        {
            this.workingFolder = workingFolder;
            this.started = DateTime.Now;

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

            TraceInfo.WriteLineIf(guessedFileName == null, "Unable to determine video file name in '{0}' - possible wrong working folder", workingFolder);
            return guessedFileName == null ? null : guessedFileName.FileName;
        }

        private static void SendKeyStroke()
        {
            TraceInfo.WriteLine("Sending key event ALT+F9");

            Keyboard.keybd_event(Keyboard.VK_MENU, 0, 0, UIntPtr.Zero);
            Thread.Sleep(500);
            Keyboard.keybd_event(Keyboard.VK_F9, 0, 0, UIntPtr.Zero);
            Thread.Sleep(200);
            Keyboard.keybd_event(Keyboard.VK_F9, 0, Keyboard.KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(200);
            Keyboard.keybd_event(Keyboard.VK_MENU, 0, Keyboard.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
