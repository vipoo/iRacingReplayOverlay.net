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
using iRacingSDK.Support;
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
            cameraStickyPeriod.Text = Settings.Default.CameraStickyPeriod.TotalSeconds.ToString();
            battleStickyPeriod.Text = Settings.Default.BattleStickyPeriod.TotalSeconds.ToString();
            battleGap.Text = Settings.Default.BattleGap.TotalSeconds.ToString();
            battleFactor.Text = Settings.Default.BattleFactor.ToString();
            preferredDriverNameTextBox.Text = Settings.Default.PreferredDriverNames;

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
            if (double.TryParse(cameraStickyPeriod.Text, out newSeconds))
            {
                Settings.Default.CameraStickyPeriod = newSeconds.Seconds();
                Settings.Default.Save();
            }
        }

        private void MaxTimeForInterestingEventTextBox_TextChanged(object sender, EventArgs e)
        {
            var newSeconds = 0.0;
            if (double.TryParse(battleGap.Text, out newSeconds))
            {
                Settings.Default.BattleGap = newSeconds.Seconds();
                Settings.Default.Save();
            }
        }

        private void PreferredDriverNameTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.PreferredDriverNames = preferredDriverNameTextBox.Text;
            Settings.Default.Save();
        }

        private void battleStickyPeriod_TextChanged(object sender, EventArgs e)
        {
            var newSeconds = 0.0;
            if (double.TryParse(battleStickyPeriod.Text, out newSeconds))
            {
                Settings.Default.BattleStickyPeriod = newSeconds.Seconds();
                Settings.Default.Save();
            }
        }

        private void OnFocus(object sender, EventArgs e)
        {
            helpText.Text = "";
            if (this.ActiveControl.Tag != null )
                helpText.Text = this.ActiveControl.Tag.ToString();
        }

        private void battleFactor_TextChanged(object sender, EventArgs e)
        {
            var factor = 0.0d;
            if (double.TryParse(battleFactor.Text, out factor))
            {
                Settings.Default.BattleFactor = factor;
                Settings.Default.Save();
            }
        }
    }
}