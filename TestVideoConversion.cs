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
using iRacingReplayOverlay.Phases.Transcoding;
using iRacingReplayOverlay.Support;
using iRacingReplayOverlay.Video;
using iRacingSDK.Support;
using MediaFoundation.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32;

namespace iRacingReplayOverlay
{
    public partial class TestVideoConversion : Form
    {
        MyListener listener;
        Task task;

        public TestVideoConversion()
        {
            InitializeComponent();
        }

        void TestVideoCapture_Load(object sender, EventArgs e)
        {
            sourceFilenameTextBox.Text = Settings.Default.WorkingFolder;

            listener = new MyListener(this.TraceMessageTextBox);
            Trace.Listeners.Add(listener);
        }

        void TestVideoCapture_FormClosed(object sender, FormClosedEventArgs e)
        {
            Trace.Listeners.Remove(listener);
        }

        void workingFolderButton_Click(object sender, EventArgs e)
        {
             var dlg = new OpenFileDialog();

            dlg.InitialDirectory = sourceFilenameTextBox.Text;
            dlg.Filter = "AVI files (*.avi)|*.avi|MPEG 4 (*.mp4)|*.mp4|All files (*.*)|*.*" ;

            if (dlg.ShowDialog() == DialogResult.OK)
                sourceFilenameTextBox.Text = dlg.FileName;
        }

        void testVideoCaptureButton_Click(object sender, EventArgs e)
        {
            testVideoCaptureButton.Enabled = false;

            if (task != null)
            {
                task.Wait(1.Second());

                task.Dispose();
            }

            var workingFolder = sourceFilenameTextBox.Text;
            var context = SynchronizationContext.Current;

            task = new Task(() => RunTest(workingFolder, context));

            TraceMessageTextBox.Text = "";

            task.Start();
        }

        void RunTest(string filename, SynchronizationContext context)
        {
            try
            {
                TranscodeVideoTest(filename);
            }
            catch(Exception e)
            {
                TraceInfo.WriteLine(e.Message);
                Trace.WriteLine(e.StackTrace, "DEBUG");
            }
            finally
            {
                context.Post(ignored => testVideoCaptureButton.Enabled = true, null);
            }
        }

        void TranscodeVideoTest(string filename)
        {
            using (MFSystem.Start())
            {
                var transcoder = new Transcoder
                {
                    IntroVideoFile = null,
                    SourceFile = filename,
                    DestinationFile = Path.ChangeExtension(filename, "wmv"),
                    VideoBitRate = 5000000,
                    AudioBitRate = 48000/8
                };

                TraceInfo.WriteLine("Begining video re-encoding.");

                transcoder.ProcessVideo((introSourceReader, sourceReader, saveToSink) =>
                {
                    int lastSecond = 0;
                    var fn = AVOperations.FadeIn(saveToSink);

                    sourceReader.Samples(sample => {
                        if (sample.Stream.CurrentMediaType.IsVideo && sample.Sample != null)
                        {
                            var s = (int)sample.Sample.SampleTime.FromNanoToSeconds();
                            if (s != lastSecond)
                                TraceInfo.WriteLine("Converted: {0} seconds", s);
                            lastSecond = s;
                        }

                        return fn(sample);

                    });
                });

                TraceInfo.WriteLine("Video converted.  Review the video file {0} to confirm it looks OK.", transcoder.DestinationFile);
                TraceInfo.WriteLine("Success!");
            }
        }
    }
}
