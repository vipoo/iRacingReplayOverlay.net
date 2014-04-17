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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iRacingReplayOverlay.net
{
    public partial class Main : Form
    {
		KeyboardHook keyboardHook;
		IRacingCaptureWorker iRacingCaptureWorker;
        OverlayWorker overlayWorker;
        private System.Timers.Timer aTimer;
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
            overlayWorker.TranscodeVideo(sourceVideoTextBox.Text, destinationFile, sourceGameDataTextBox.Text);
        }

        private void OnTranscoderCompleted()
        {
            transcodeCancelButton.Visible = false;
            transcodeVideoButton.Enabled = true;
            transcodeProgressBar.Value = 0;
            transcodeProgressBar.Visible = false;
        }

        const int GuessFinalizationRequiredSeconds = 25;
        
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

            overlayWorker = new OverlayWorker();
            overlayWorker.Progress += OnTranscoderProgress;
            overlayWorker.Completed += OnTranscoderCompleted;
            overlayWorker.ReadFramesCompleted += OnTranscoderReadFramesCompleted;

            var uiContext = SynchronizationContext.Current;
 
            aTimer = new System.Timers.Timer(500);
            aTimer.Elapsed += (s, a) => uiContext.Post(ignored => GuessFinializeProgress(), null);

            workingFolderTextBox.Text = Settings.Default.WorkingFolder;
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

			captureLight.Visible = iRacingCaptureWorker.Toogle();
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
            var dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
                sourceVideoTextBox.Text = fbd.FileName;
        }

        void sourceGameDataButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            var dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
                sourceGameDataTextBox.Text = fbd.FileName;
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
    }
}
