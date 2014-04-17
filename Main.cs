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
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace iRacingReplayOverlay.net
{
    public partial class Main : Form
    {
		KeyboardHook keyboardHook;
		IRacingCaptureWorker iRacingCaptureWorker;
        OverlayWorker overlayWorker;
        System.Windows.Forms.Timer aTimer;
        int guessedProgessedAmount;
        const int GuessFinalizationRequiredSeconds = 25;
        System.Windows.Forms.Timer fileWatchTimer;
        
        enum States {Idle, CapturingGameData, Transcoding};
        States _states = States.Idle;

        States State
        {
            set
            {
                _states = value;
                switch(_states)
                {
                    case States.Idle:
                        transcodeVideoButton.Enabled = IsReadyForTranscoding();
                        transcodeCancelButton.Visible = false;
                        transcodeProgressBar.Visible = false;
                        transcodeProgressBar.Value = 0;
                        workingFolderTextBox.Enabled = 
                        workingFolderButton.Enabled = 
                        sourceVideoTextBox.Enabled =
                        sourceVideoButton.Enabled = true;
                        break;

                    case States.CapturingGameData:
                        transcodeVideoButton.Enabled = false;
                        transcodeCancelButton.Visible = false;
                        transcodeProgressBar.Visible = false;
                        workingFolderTextBox.Enabled = 
                        workingFolderButton.Enabled = 
                        sourceVideoTextBox.Enabled =
                        sourceVideoButton.Enabled = false;
                        break;

                    case States.Transcoding:
                        transcodeVideoButton.Enabled = false;
                        transcodeCancelButton.Visible = true;
                        transcodeProgressBar.Visible = true;
                        workingFolderTextBox.Enabled = 
                        workingFolderButton.Enabled = 
                        sourceVideoTextBox.Enabled =
                        sourceVideoButton.Enabled = false;
                        break;
                }
            }
            get
            {
                return _states;
            }
        }

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            keyboardHook = new KeyboardHook();
            keyboardHook.KeyReleased += GlobalKeyPressed;
            keyboardHook.Start();

            iRacingCaptureWorker = new IRacingCaptureWorker();
            iRacingCaptureWorker.NewVideoFileFound += NewVideoFileFound;
            overlayWorker = new OverlayWorker();
            overlayWorker.Progress += OnTranscoderProgress;
            overlayWorker.Completed += OnTranscoderCompleted;
            overlayWorker.ReadFramesCompleted += OnTranscoderReadFramesCompleted;

            fileWatchTimer = new System.Windows.Forms.Timer();
            fileWatchTimer.Interval = 10;
            fileWatchTimer.Tick += (s, a) => OnGameDataFileChanged();
            fileWatchTimer.Start();

            aTimer = new System.Windows.Forms.Timer();
            aTimer.Interval = 500;
            aTimer.Tick += (s, a) => GuessFinializeProgress();

            workingFolderTextBox.Text = Settings.Default.WorkingFolder;
        }

        void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            keyboardHook.Dispose();
            iRacingCaptureWorker.Dispose();
            overlayWorker.Dispose();
        }

//Game Capture

        void GlobalKeyPressed(Keys keyCode)
        {
            if (State == States.Transcoding)
                return;

            if (keyCode != Keys.F9)
                return;

            captureLabel.Visible = captureLight.Visible = iRacingCaptureWorker.Toogle(workingFolderTextBox.Text);
            State = captureLight.Visible ? States.CapturingGameData : States.Idle;
        }

        void workingFolderButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = workingFolderTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.WorkingFolder = workingFolderTextBox.Text = fbd.SelectedPath;
                Settings.Default.Save();
            }
        }

//Transcoding

        void sourceVideoButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "Mpeg 4|*.mp4|All files (*.*)|*.*";
            fbd.FileName = sourceVideoTextBox.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
                sourceVideoTextBox.Text = fbd.FileName;
        }

        void TranscodeVideo_Click(object sender, EventArgs e)
        {
            State = States.Transcoding;
            var destinationFile = Path.ChangeExtension(sourceVideoTextBox.Text, "wmv");
            var sourceGameData = Path.ChangeExtension(sourceVideoTextBox.Text, "csv");
            overlayWorker.TranscodeVideo(sourceVideoTextBox.Text, destinationFile, sourceGameData);
        }

        void OnTranscoderReadFramesCompleted()
        {
            guessedProgessedAmount = (transcodeProgressBar.Maximum - transcodeProgressBar.Value) / ((int)GuessFinalizationRequiredSeconds * 2);
            aTimer.Start();
        }

        void OnTranscoderProgress(long timestamp, long duration)
        {
            duration += GuessFinalizationRequiredSeconds.FromSecondsToNano();
            transcodeProgressBar.Value = (int)(timestamp * transcodeProgressBar.Maximum / duration);
        }

        void OnTranscoderCompleted()
        {
            State = States.Idle;
        }
        
        void GuessFinializeProgress()
        {
            transcodeProgressBar.Value = Math.Min(transcodeProgressBar.Value + guessedProgessedAmount, transcodeProgressBar.Maximum-5);
            if (transcodeProgressBar.Value == transcodeProgressBar.Maximum - 5)
                aTimer.Stop();
        }

        void transcodeCancel_Click(object sender, EventArgs e)
        {
            overlayWorker.Cancel();
            State = States.Idle;
        }

        void sourceVideoTextBox_TextChanged(object sender, EventArgs e)
        {
            OnGameDataFileChanged();
        }

        void NewVideoFileFound(string latestVideoFileName)
        {
            this.sourceVideoTextBox.Text = latestVideoFileName;
        }

        bool IsReadyForTranscoding()
        {
            if (sourceVideoTextBox.Text == null || sourceVideoTextBox.Text.Length == 0)
                return false;

            errorSourceVideoLabel.Visible = !File.Exists(Path.ChangeExtension(sourceVideoTextBox.Text, ".csv"));
            return (!errorSourceVideoLabel.Visible && File.Exists(sourceVideoTextBox.Text));
        }

        void OnGameDataFileChanged()
        {
            if (State == States.Idle)
                transcodeVideoButton.Enabled = IsReadyForTranscoding();
        }
    }
}
