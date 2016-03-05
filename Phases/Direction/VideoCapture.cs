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

using iRacingSDK.Support;
using System;
using System.IO;
using System.Linq;
using Win32;
using System.Timers;
using System.Collections.Generic;
using iRacingReplayOverlay.Phases.Capturing;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class VideoCapture
    {
        string workingFolder;
        DateTime started;
        Timer timer;
        List<CapturedVideoFile> captureFileNames = new List<CapturedVideoFile>();

        public void Activate(string workingFolder)
        {
            this.workingFolder = workingFolder;
            this.started = DateTime.Now;

            timer = new Timer(500);
            timer.Elapsed += CaptureNewFileNames; ;
            timer.AutoReset = false;
            timer.Enabled = true;

            SendKeyStroke();
        }

        private void CaptureNewFileNames(object sender, ElapsedEventArgs e)
        {
            try
            {
                var guessedFileName = Directory.GetFiles(workingFolder, "*.avi")
                    .Concat(Directory.GetFiles(workingFolder, "*.mp4"))
                    .Select(fn => new { FileName = fn, CreationTime = File.GetCreationTime(fn) })
                    .Where(f => f.CreationTime >= started)
                    .OrderByDescending(f => f.CreationTime)
                    .FirstOrDefault();

                if (guessedFileName != null && !captureFileNames.Any(c => c.FileName == guessedFileName.FileName))
                {
                    TraceInfo.WriteLine("Found video file {0}", guessedFileName.FileName);
                    captureFileNames.Add(new CapturedVideoFile { FileName = guessedFileName.FileName });
                }
            }
            catch (Exception ee)
            {
                TraceError.WriteLine(ee.Message);
                TraceError.WriteLine(ee.StackTrace);
            }
            finally
            {
                if (timer != null)
                    timer.Start();
            }
        }

        public List<CapturedVideoFile> Deactivate()
        {
            if (timer != null)
            {
                var t = timer;
                timer = null;
                t.Stop();
                t.Dispose();
            }

            SendKeyStroke();

            System.Threading.Thread.Sleep(2000);

            CaptureNewFileNames(null, null);

            TraceInfo.WriteLineIf(captureFileNames.Count == 0, "Unable to find video files in folder '{0}' - check your Video Working folder", workingFolder);

            return captureFileNames;
        }

        private static void SendKeyStroke()
        {
            TraceInfo.WriteLine("Sending key event ALT+F9");

            Keyboard.keybd_event(Keyboard.VK_MENU, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            Keyboard.keybd_event(Keyboard.VK_F9, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            Keyboard.keybd_event(Keyboard.VK_F9, 0, Keyboard.KEYEVENTF_KEYUP, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            Keyboard.keybd_event(Keyboard.VK_MENU, 0, Keyboard.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
