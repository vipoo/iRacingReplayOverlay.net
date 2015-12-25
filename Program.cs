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
using System.Diagnostics;
using System.Windows.Forms;
using iRacingSDK.Support;
using iRacingReplayOverlay.Support;

namespace iRacingReplayOverlay
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if( AwsKeys.HaveKeys)
                using (var awsLogListener = new AwsLogListener())
                {
                    Trace.Listeners.Add(awsLogListener);
                    Application.Run(new Main());
                }
            else
                Application.Run(new Main());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                TraceInfo.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace, "DEBUG");
            }
            else
            {
                Trace.WriteLine("An unknown error occured. {0}, {1}".F(e.ExceptionObject.GetType().Name, e.ExceptionObject.ToString()));
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("An error occured.  Details have been logged.\n\n{0}".F(e.Exception.Message), "Error");
            TraceInfo.WriteLine(e.Exception.Message);
            Trace.WriteLine(e.Exception.StackTrace, "DEBUG");
        }
    }
}
