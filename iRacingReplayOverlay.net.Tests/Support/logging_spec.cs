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

using iRacingSDK;
using iRacingSDK.Support;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace iRacingReplayOverlay.Support
{
    [TestFixture]
    public class LoggingSpecs
    {
        [Test]
        public void it_should_log_to_file()
        {
            var stubContent = "a normal trace write line";
            var filename = "./logging.test.log";

            File.Delete(filename);

            iRacingReplayOverlay.Support.LogListener.ToFile(filename);

            Trace.WriteLine(stubContent, "INFO");
            AssertThatLogContains(filename, stubContent);
        }

        [Test]
        public void it_changes_log_file_name()
        {
            var stubContent1 = "stub content 11111";
            var stubContent2 = "stub content 22222";
            var filename1 = "./logging.1.test.log";
            var filename2 = "./logging.2.test.log";

            File.Delete(filename1);
            File.Delete(filename2);

            iRacingReplayOverlay.Support.LogListener.ToFile(filename1);
            Trace.WriteLine(stubContent1, "INFO");

            AssertThatLogContains(filename1, stubContent1);

            iRacingReplayOverlay.Support.LogListener.ToFile(filename2);
            Trace.WriteLine(stubContent2, "INFO");

            AssertThatLogContains(filename2, stubContent2);
        }

        [Test]
        public void it_should_rename_file()
        {
            var stubContent1 = "stub content 11111";
            var stubContent2 = "stub content 22222";
            var filename1 = "./logging.1.test.log";
            var filename2 = "./logging.2.test.log";

            File.Delete(filename1);
            File.Delete(filename2);

            iRacingReplayOverlay.Support.LogListener.ToFile(filename1);
            Trace.WriteLine(stubContent1, "INFO");
            AssertThatLogContains(filename1, stubContent1);

            iRacingReplayOverlay.Support.LogListener.MoveToFile(filename2);
            Trace.WriteLine(stubContent2, "INFO");
            AssertThatLogContains(filename2, stubContent2);
            AssertThatLogContains(filename2, stubContent1);
            Assert.That(File.Exists(filename1), Is.False);
        }

        private static void AssertThatLogContains(string filename, string stubContent)
        {
            var count = 10;
            var stillWaiting = true;
            var logContent = "";

            while (stillWaiting)
            {
                try
                {
                    logContent = File.ReadAllText(filename);
                } catch(System.Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }

                stillWaiting = count-- > 0 && !logContent.Contains(stubContent);

                if (stillWaiting)
                    Thread.Sleep(50);
            }

            Assert.That(logContent, Is.StringContaining(stubContent));
        }
    }
}
