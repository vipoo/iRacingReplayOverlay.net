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
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class ConfigureGeneralSettings : Form
    {
        public ConfigureGeneralSettings()
        {
            InitializeComponent();
            AddPanelComponents();
        }

        void AddPanelComponents()
        {
            AddTimeField("Time between camera switches:", "The time period that must elapsed before a new random camera is selected.", "CameraStickyPeriod");
            AddTimeField("Time between battle switches:", "The time period that must elapsed before a new battle is randomly selected.", "BattleStickyPeriod");
            AddTimeField("Time gap between cars for battle:", "The approximate amount of time between cars to determine if they are battling.  Default 1 second.", "BattleGap");
            AddTimeField("Time to track leader at race start", "The amount of time, to stay focused on the leader, at race start.  After this period, the normal camera and driver tracking rules will apply.", "FollowLeaderAtRaceStartPeriod");
            AddTimeField("Time to track leader on last lap", "The amount of time, to stay focused on the leader, prior to the race finish.", "FollowLeaderBeforeRaceEndPeriod");
            AddNumberField("Factor for battle selection.", "A factor to bias the random selection of battles.  Larger numbers will tend to focus on battles at front.  Smaller numbers will increase the chance of battles futher down the order to be selected.  Recommend values between 1.3 and 2.0.\n\nShould always be more than 1.0\n\n1.0 means all battles have same chance of selection.", "BattleFactor2");

            AddBlankRow();
            AddMinuteField("Duration of Highlight video", "The duration to target for the length of the edited highlight videos.", "HighlightVideoTargetDuration");

            AddBlankRow();
            AddKeyPressField("Hot Key for Video Capture", "The hotkey used by your video capture program.  At this time, this can not be changed in iRacing Replay Director.  Ensure your video capture software uses ALT+F9 or F9");

            AddBlankRow();
            AddCheckboxField("Disable Incidents Capture",
                @"If this option is selected, then the director will not capture or focus on incidents or crashes",
                "DisableIncidentsSearch");

            AddBlankRow();
            AddStringField("Preferred driver names (comma separated):", "A comma seperated list of driver names, to preference in camera selection.", "PreferredDriverNames");
            AddCheckboxField("Only select battles for my perferred drivers",
               @"This option determines what drivers will be selected for battles.  

If selected, then the camera will only focus on battles of your preferred drivers.  All other battles will be ignored and cut from the highlighted video.

If not selected, then all battles can be selected, but your perferred drivers will be prioritised",
               "FocusOnPreferedDriver");

            AddBlankRow();
            AddIntField("Show Results after nth position", "Show the results flash cards, after the driver in the selected position finishes.", "ResultsFlashCardPosition");

            if (AwsKeys.HaveKeys)
            {
                AddBlankRow();
                AddCheckboxField("Allow usage data to be sent to developer",
                @"Send anonymous usage data to the developer, to assist in fixing bugs and improving the application.

What data is sent? 
The data sent is the same data that is written to your local log files.

Why should I say yes?
It will really help me to understand how people use this application, and diagnose common problems people have.

Where does the data go ?
The data is sent encypted to me - the developer - dean.netherton@gmail.com", "SendUsageData");

            }
        }

        void okButton_Click(object sender, EventArgs e)
        {            
            foreach (var s in OnSave)
                s();

            Settings.Default.Save();
        }

        void OnFocus(object sender, EventArgs e)
        {
            helpText.Text = "";
            if (this.ActiveControl.Tag != null)
                helpText.Text = this.ActiveControl.Tag.ToString();
        }
    }
}