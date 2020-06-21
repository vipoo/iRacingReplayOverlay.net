// This file is part of SuperMFLib.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/SuperMFLib
//
// SuperMFLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SuperMFLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SuperMFLib.  If not, see <http://www.gnu.org/licenses/>.

using MediaFoundation.Net;
using System;
using System.Windows.Forms;

namespace BasicPlayback
{
    public partial class Main : Form
    {
        private Player player;
        private MFSystem mfSystem;
     
        public Main()
        {
            InitializeComponent();

            mfSystem = MFSystem.Start();

            player = new Player(this.Handle);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if( player != null )
                player.Dispose();

            mfSystem.Dispose();
        }

        void openFileToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "Windows Media|*.wmv;*.wma;*.asf|Wave|*.wav|MP3|*.mp3|All files|*.*";

            var result = dialog.ShowDialog();

            if( result == System.Windows.Forms.DialogResult.OK)
                player.Play(dialog.FileName);
        }

        void OnPaint(IntPtr hwnd)
        {
            player.Repaint();
        }

        const int WM_PAINT = 0x000F;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    OnPaint(m.HWnd);
                    base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
