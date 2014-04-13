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

        public Main()
        {
            InitializeComponent();
        }

        private void TranscodeVideo_Click(object sender, EventArgs e)
        {
            Transcoder.TranscodeVideo();
        }

        private void Main_Load(object sender, EventArgs e)
        {
			keyboardHook = new KeyboardHook();
			keyboardHook.KeyReleased += GlobalKeyPressed;
			keyboardHook.Start();
        }

		void GlobalKeyPressed(Keys keyCode)
		{
			if( keyCode == Keys.F9)
				captureLight.Visible = !captureLight.Visible;
		}

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
			keyboardHook.Dispose();
        }
    }
}
