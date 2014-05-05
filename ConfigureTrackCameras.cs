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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class ConfigureTrackCameras : Form
    {
        private TrackCameras trackCameras;

        public ConfigureTrackCameras()
        {
            InitializeComponent();
        }

        private void cameraList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ratioTextBox.Enabled = cameraList.SelectedItems.Count != 0;
            if (ratioTextBox.Enabled)
            {
                ratioTextBox.Text = cameraList.SelectedItems[0].SubItems[1].Text;
                ratioLabel.Text = "{0} has a".F(cameraList.SelectedItems[0].Text);
                var context = SynchronizationContext.Current;
                new Task(() => { Thread.Sleep(200); context.Post(i => ratioTextBox.Focus(), null); }).Start();
            }
            else
            {
                ratioTextBox.Text = "";
                ratioLabel.Text = "-- has a";
            }
        }

        private void trackList_SelectedIndexChanged(object sender, EventArgs e)
        {
            cameraList.Items.Clear();

            InitaliseListOfCameras();

            cameraList_SelectedIndexChanged(sender, e);
            UpdateTotal();
        }

        private void ratioTextBox_TextChanged(object sender, EventArgs e)
        {
            if (cameraList.SelectedItems.Count == 0)
                return;

            var trackCamera = GetCamerasForSelectedTrack().FirstOrDefault(tc => tc.CameraName == cameraList.SelectedItems[0].Text);

            int number = 0;
            int.TryParse(ratioTextBox.Text, out number);
            if (number > 99)
                number = 0;

            trackCamera.Ratio = number;
            cameraList.SelectedItems[0].SubItems[1].Text = number.ToString();

            UpdateTotal();
        }

        private System.Collections.Generic.IEnumerable<TrackCamera> GetCamerasForSelectedTrack()
        {
            return trackCameras.Where(tc => tc.TrackName == (string)trackList.SelectedItem);
        }

        private void TrackCameraPerferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.lastSelectedTrackName = (string)trackList.SelectedItem;
            Settings.Default.Save();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TrackCameraPerferences_Load(object sender, EventArgs e)
        {
            trackCameras = Settings.Default.trackCameras;
            if( trackCameras == null)
            {
                trackCameras = new TrackCameras();
                Settings.Default.trackCameras = trackCameras;
                Settings.Default.Save();
            }
            
            DiscoverAnyNewTrackCameras();

            InitialiseDropDownListOfTracks();
        }

        private void InitialiseDropDownListOfTracks()
        {
            foreach (var track in trackCameras.Select(tc => tc.TrackName).Distinct().OrderBy(t => t))
                trackList.Items.Add(track);

            trackList.SelectedItem = Settings.Default.lastSelectedTrackName;
        }

        /// <summary>
        /// Load session data from iRacing and loads the cameras available for the current loaded
        /// track and saves the track/camera combination into the users setting
        /// </summary>
        private void DiscoverAnyNewTrackCameras()
        {
            var sample = iRacing.GetDataFeed().First();
            if (!sample.IsConnected)
                return;

            if (trackCameras.Any(tc => tc.TrackName == sample.SessionData.WeekendInfo.TrackDisplayName))
                return;

            foreach (var camera in sample.SessionData.CameraInfo.Groups)
                trackCameras.Add(new TrackCamera
                {
                    TrackName = sample.SessionData.WeekendInfo.TrackDisplayName,
                    CameraName = camera.GroupName
                });
        }

        private void InitaliseListOfCameras()
        {
            foreach (var tc in GetCamerasForSelectedTrack())
            {
                var lvi = new ListViewItem { Text = tc.CameraName };
                lvi.SubItems.Add(tc.Ratio.ToString());

                cameraList.Items.Add(lvi);
            }
        }

        private void UpdateTotal()
        {
            int total = 0;
            foreach (var tc in GetCamerasForSelectedTrack())
            {
                total += tc.Ratio;
            }

            totalRatio.Text = total.ToString();
            TotalErrorLabel.Visible = total > 100;
        }
    }
}
