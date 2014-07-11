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

using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Linq;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class ConfigureGeneralSettings : Form
    {
        public ConfigureGeneralSettings()
        {
            InitializeComponent();
        }

        private void ConfigureGeneralSettings_Load(object sender, EventArgs e)
        {
            MaxTimeBetweenCameraChangesTextBox.Text = Settings.Default.MaxTimeBetweenCameraChanges.TotalSeconds.ToString();
            MaxTimeForInterestingEventTextBox.Text = Settings.Default.MaxTimeForInterestingEvent.TotalSeconds.ToString();
            PreferredDriverNameTextBox.Text = Settings.Default.PreferredDriverNames;

            var cred = Settings.Default.YouTubeCredentials ?? new Credentials();
            this.youTubeUserName.Text = cred.UserName;
            this.youTubePassword.Text = cred.FreePassword;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var cred = Settings.Default.YouTubeCredentials = Settings.Default.YouTubeCredentials ?? new Credentials();
            cred.UserName = this.youTubeUserName.Text;
            cred.FreePassword = this.youTubePassword.Text;
            Settings.Default.Save();
        }

        private void MaxTimeBetweenCameraSwitchesTextBox_TextChanged(object sender, EventArgs e)
        {
            var newSeconds = 0.0;
            if (double.TryParse(MaxTimeBetweenCameraChangesTextBox.Text, out newSeconds))
            {
                Settings.Default.MaxTimeBetweenCameraChanges = TimeSpan.FromSeconds(newSeconds);
                Settings.Default.Save();
            }
        }

        private void MaxTimeForInterestingEventTextBox_TextChanged(object sender, EventArgs e)
        {
            var newSeconds = 0.0;
            if (double.TryParse(MaxTimeForInterestingEventTextBox.Text, out newSeconds))
            {
                Settings.Default.MaxTimeForInterestingEvent = TimeSpan.FromSeconds(newSeconds);
                Settings.Default.Save();
            }
        }

        private void PreferredDriverNameTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.PreferredDriverNames = PreferredDriverNameTextBox.Text;
            Settings.Default.Save();
        }
    }
}