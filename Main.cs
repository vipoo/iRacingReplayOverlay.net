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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iRacingReplayOverlay.net
{
    public partial class Main : Form
    {
		KeyboardHook keyboardHook;
		IRacingCaptureWorker iRacingCaptureWorker;
        OverlayWorker overlayWorker;

        public Main()
        {
            InitializeComponent();
        }

        private void TranscodeVideo_Click(object sender, EventArgs e)
        {
            transcodeVideoButton.Enabled = false;
            transcodeCancelButton.Visible = true;
            overlayWorker.TranscodeVideo();
        }

        private void OnTranscoderCompleted()
        {
            transcodeCancelButton.Visible = false;
            transcodeVideoButton.Enabled = true;
            transcodeProgressBar.Value = 0;
            transcodeProgressBar.Visible = false;
        }

        private void OnTranscoderProgress(int percentage)
        {
            transcodeProgressBar.Visible = true;
            transcodeProgressBar.Value = percentage;
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
        }

		void GlobalKeyPressed(Keys keyCode)
		{
			if(keyCode != Keys.F9)
				return;

			captureLight.Visible = iRacingCaptureWorker.Toogle();
		}

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
			keyboardHook.Dispose();
			iRacingCaptureWorker.Dispose();
        }

        private void transcodeCancel_Click(object sender, EventArgs e)
        {
            transcodeCancelButton.Visible = false;
            overlayWorker.Cancel();
        }
    }
}
