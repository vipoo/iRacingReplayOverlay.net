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

using iRacingReplayOverlay.Phases.Direction;
using iRacingReplayOverlay.Support;
using iRacingSDK.Support;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class TestVideoCapture : Form
    {
        MyListener listener;
        Task task;
        
        public TestVideoCapture()
        {
            InitializeComponent();
        }

        void TestVideoCapture_Load(object sender, EventArgs e)
        {
            workingFolderTextBox.Text = Settings.Default.WorkingFolder;

            listener = new MyListener(this.TraceMessageTextBox);
            Trace.Listeners.Add(listener);
        }

        void TestVideoCapture_FormClosed(object sender, FormClosedEventArgs e)
        {
            Trace.Listeners.Remove(listener);
        }

        void workingFolderButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = workingFolderTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
                workingFolderTextBox.Text = fbd.SelectedPath;
        }

        void workingFolderTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.WorkingFolder = workingFolderTextBox.Text;
            Settings.Default.Save();
        }

        void testVideoCaptureButton_Click(object sender, EventArgs e)
        {
            testVideoCaptureButton.Enabled = false;

            if (task != null)
            {
                task.Wait(1.Second());

                task.Dispose();
            }

            var workingFolder = workingFolderTextBox.Text;
            var context = SynchronizationContext.Current;

            task = new Task(() => RunTest(workingFolder, context));

            TraceMessageTextBox.Text = "";

            task.Start();
        }

        private void RunTest(string workingFolder, SynchronizationContext context)
        {
            try
            {
                Trace.WriteLine("Begining Test....", "INFO");
                var videoCapture = new VideoCapture();

                Trace.WriteLine("Broadcasting keypress ALT+F9 to activate your video capture software", "INFO");
                videoCapture.Activate(workingFolder);

                Trace.WriteLine("Expecting video file to be written in folder: {0}".F(workingFolder), "INFO");

                Trace.WriteLine("Waiting for 5 seconds", "INFO");

                for (var i = 5; i >= 0; i--)
                {
                    Thread.Sleep(1.Seconds());
                    Trace.WriteLine("{0} Seconds...".F(i), "INFO");
                }

                Trace.WriteLine("Broadcasting keypress ALT+F9 to deactivate your video capture software", "INFO");
                var filename = videoCapture.Deactivate();

                if (filename != null)
                {
                    Trace.WriteLine("", "INFO");
                    Trace.WriteLine("Success!", "INFO");
                    Trace.WriteLine("Found your video file {0}.".F(filename), "INFO");
                }
                else
                {
                    Trace.WriteLine("", "INFO");
                    Trace.WriteLine("Failure!", "INFO");
                }
            }
            finally
            {
                context.Post(ignored => testVideoCaptureButton.Enabled = true, null);
            }
        }
    }
}
