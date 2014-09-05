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
        private int nextTabIndex;
        public ConfigureGeneralSettings()
        {
            InitializeComponent();
        }

        void ConfigureGeneralSettings_Load(object sender, EventArgs e)
        {
            cameraStickyPeriod.Text = Settings.Default.CameraStickyPeriod.TotalSeconds.ToString();
            battleStickyPeriod.Text = Settings.Default.BattleStickyPeriod.TotalSeconds.ToString();
            battleGap.Text = Settings.Default.BattleGap.TotalSeconds.ToString();
            battleFactor.Text = Settings.Default.BattleFactor.ToString();
            preferredDriverNameTextBox.Text = Settings.Default.PreferredDriverNames;
            followLeaderAtRaceStartPeriodTextBox.Text = Settings.Default.FollowLeaderAtRaceStartPeriod.TotalSeconds.ToString();

            var cred = Settings.Default.YouTubeCredentials ?? new Credentials();
            this.youTubeUserName.Text = cred.UserName;
            this.youTubePassword.Text = cred.FreePassword;
        }

        void okButton_Click(object sender, EventArgs e)
        {
            var cred = Settings.Default.YouTubeCredentials = Settings.Default.YouTubeCredentials ?? new Credentials();
            cred.UserName = this.youTubeUserName.Text;
            cred.FreePassword = this.youTubePassword.Text;

            Save_cameraStickyPeriod();
            Save_battleGap();
            Save_preferredDriverNameTextBox();
            Save_battleStickyPeriod();
            Save_battleFactor();
            Save_followLeaderAtRaceStartPeriodTextBox();

            Settings.Default.Save();
        }

        void Save_followLeaderAtRaceStartPeriodTextBox()
        {
            var newSeconds = 0.0;
            if (double.TryParse(followLeaderAtRaceStartPeriodTextBox.Text, out newSeconds))
                Settings.Default.FollowLeaderAtRaceStartPeriod= newSeconds.Seconds();
        }

        void Save_cameraStickyPeriod()
        {
            var newSeconds = 0.0;
            if (double.TryParse(cameraStickyPeriod.Text, out newSeconds))
                Settings.Default.CameraStickyPeriod = newSeconds.Seconds();
        }

        void Save_battleGap()
        {
            var newSeconds = 0.0;
            if (double.TryParse(battleGap.Text, out newSeconds))
                Settings.Default.BattleGap = newSeconds.Seconds();
        }

        void Save_preferredDriverNameTextBox()
        {
            Settings.Default.PreferredDriverNames = preferredDriverNameTextBox.Text;
        }

        void Save_battleStickyPeriod()
        {
            var newSeconds = 0.0;
            if (double.TryParse(battleStickyPeriod.Text, out newSeconds))
                Settings.Default.BattleStickyPeriod = newSeconds.Seconds();
        }

        void Save_battleFactor()
        {
            var factor = 0.0d;
            if (double.TryParse(battleFactor.Text, out factor))
                Settings.Default.BattleFactor = factor;
        }

        void OnFocus(object sender, EventArgs e)
        {
            helpText.Text = "";
            if (this.ActiveControl.Tag != null)
                helpText.Text = this.ActiveControl.Tag.ToString();
        }
    }
}