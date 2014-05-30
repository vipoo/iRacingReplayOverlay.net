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

using System;
using System.Diagnostics;
using System.IO;

namespace iRacingReplayOverlay.Support
{
    public class LogListener : TraceListener
    {
        static LogListener logFile;
        public string FileName { get; internal set; }
     
        public LogListener(string filename)
        {
            this.FileName = filename;
            Write("\r\n");
            WriteLine("----------------------------");
        }

        public override void Write(string message)
        {
            File.AppendAllText(FileName, message);
        }

        string lastMessage = null;
        DateTime lastTime = DateTime.Now;

        public override void WriteLine(string message)
        {
            var now = DateTime.Now;
            if (message == lastMessage && now - lastTime < TimeSpan.FromSeconds(5))
                return;

            lastMessage = message;
            lastTime = now;

            this.Write(now.ToString("s"));
            this.Write("\t");
            this.Write(message + "\r\n");
        }

        internal static void ToFile(string filename)
        {
            if( logFile != null)
            {
                Trace.WriteLine("Moving logging to file {0}".F(filename), "INFO");
                Trace.Listeners.Remove(logFile);
                logFile.Dispose();
            }

            logFile = new LogListener(filename);
            Trace.Listeners.Add(logFile);
        }

        internal static void MoveToFile(string filename)
        {
            if (logFile == null)
                throw new Exception("Attempt to rename non-existing log file to {0}".F(filename));

            Trace.WriteLine("Renaming logging to file {0}".F(filename), "INFO");
            Trace.Listeners.Remove(logFile);
            logFile.Dispose();

            File.Move(logFile.FileName, filename);

            logFile = new LogListener(filename);
            Trace.Listeners.Add(logFile);
        }
    }
}
