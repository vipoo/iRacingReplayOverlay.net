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
        private System.Windows.Forms.Timer aTimer;
        private int guessedProgessedAmount;

        public Main()
        {
            InitializeComponent();
        }

        private void TranscodeVideo_Click(object sender, EventArgs e)
        {
            transcodeVideoButton.Enabled = false;
            transcodeCancelButton.Visible = true;
            var destinationFile = Path.ChangeExtension(sourceVideoTextBox.Text, "wmv");
            var sourceGameData = Path.ChangeExtension(sourceVideoTextBox.Text, "csv");
            overlayWorker.TranscodeVideo(sourceVideoTextBox.Text, destinationFile, sourceGameData);
        }

        private void OnTranscoderCompleted()
        {
            transcodeCancelButton.Visible = false;
            transcodeVideoButton.Enabled = true;
            transcodeProgressBar.Value = 0;
            transcodeProgressBar.Visible = false;
        }

        const int GuessFinalizationRequiredSeconds = 25;
        private System.Windows.Forms.Timer fileWatchTimer;
        
        private void OnTranscoderProgress(long timestamp, long duration)
        {
            duration += GuessFinalizationRequiredSeconds.FromSecondsToNano();
            transcodeProgressBar.Visible = true;
            transcodeProgressBar.Value = (int)(timestamp * transcodeProgressBar.Maximum / duration);
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
            aTimer.Tick += (s,a) => GuessFinializeProgress();

            workingFolderTextBox.Text = Settings.Default.WorkingFolder;
        }

        void NewVideoFileFound(string latestVideoFileName)
        {
            this.sourceVideoTextBox.Text = latestVideoFileName;
        }

        private void OnTranscoderReadFramesCompleted()
        {
            guessedProgessedAmount = (transcodeProgressBar.Maximum - transcodeProgressBar.Value) / ((int)GuessFinalizationRequiredSeconds * 2);
            aTimer.Start();
        }

        private void GuessFinializeProgress()
        {
            transcodeProgressBar.Value = Math.Min(transcodeProgressBar.Value + guessedProgessedAmount, transcodeProgressBar.Maximum);
            if (transcodeProgressBar.Value == 100)
                aTimer.Stop();
        }

		void GlobalKeyPressed(Keys keyCode)
		{
			if(keyCode != Keys.F9)
				return;

			captureLight.Visible = iRacingCaptureWorker.Toogle(workingFolderTextBox.Text);
		}

        void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
			keyboardHook.Dispose();
			iRacingCaptureWorker.Dispose();
        }

        void transcodeCancel_Click(object sender, EventArgs e)
        {
            transcodeCancelButton.Visible = false;
            overlayWorker.Cancel();
        }

        void sourceVideoButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "Mpeg 4|*.mp4|All files (*.*)|*.*";
            fbd.FileName = sourceVideoTextBox.Text;

            var dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
                sourceVideoTextBox.Text = fbd.FileName;
        }

        private void workingFolderButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = workingFolderTextBox.Text;
            
            var dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                Settings.Default.WorkingFolder = workingFolderTextBox.Text = fbd.SelectedPath;
                Settings.Default.Save();
            }
        }

        void sourceVideoTextBox_TextChanged(object sender, EventArgs e)
        {
            OnGameDataFileChanged();
        }

        void OnGameDataFileChanged()
        {
            if( sourceVideoTextBox.Text == null || sourceVideoTextBox.Text.Length == 0)
            {
                errorSourceVideoLabel.Visible = false;
                transcodeVideoButton.Enabled = false;
                return;
            }

            var gameDataFile = Path.ChangeExtension(sourceVideoTextBox.Text, ".csv");
            errorSourceVideoLabel.Visible = !File.Exists(gameDataFile);
            transcodeVideoButton.Enabled = !errorSourceVideoLabel.Visible && File.Exists(sourceVideoTextBox.Text);
        }
    }
}
