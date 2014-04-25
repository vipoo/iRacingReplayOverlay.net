using iRacingSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iRacingReplayOverlay.Support;

namespace IRacingReplayOverlay
{
    public partial class TrackCameraPerferences : Form
    {
        private TrackCameras trackCameras;
        public TrackCameraPerferences()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ratioTextBox.Enabled = (cameraList.SelectedItems.Count != 0);
            if (ratioTextBox.Enabled)
            {
                ratioTextBox.Text = cameraList.SelectedItems[0].SubItems[1].Text;
                ratioLabel.Text = "{0} has a".F(cameraList.SelectedItems[0].Text);
            }
            else
            {
                ratioTextBox.Text = "";
                ratioLabel.Text = "-- has a";

            }

        }

        private void TrackCameraPerferences_Load(object sender, EventArgs e)
        {
            trackCameras = Settings.Default.trackCameras;

            var sample = iRacing.GetDataFeed().First();
            if( sample.IsConnected && !trackCameras.Any(tc => tc.TrackName == sample.SessionData.WeekendInfo.TrackDisplayName))
            {
                foreach (var camera in sample.SessionData.CameraInfo.Groups)
                {   
                    trackCameras.Add(new TrackCamera 
                    {
                        TrackName = sample.SessionData.WeekendInfo.TrackDisplayName,
                        CameraName = camera.GroupName
                    });
                }
            }

            foreach( var track in trackCameras.Select( tc => tc.TrackName).Distinct().OrderBy( t => t))
            {
                trackList.Items.Add(track);
            }

            trackList.SelectedItem = Settings.Default.lastSelectedTrackName;
        }

        private void trackList_SelectedIndexChanged(object sender, EventArgs e)
        {
            cameraList.Items.Clear();

            foreach( var tc in trackCameras.Where(tc => tc.TrackName == (string)trackList.SelectedItem))
            {
                var lvi = new ListViewItem { Text = tc.CameraName };
                lvi.SubItems.Add(tc.Ratio.ToString());

                cameraList.Items.Add(lvi);
            }

            listView1_SelectedIndexChanged(sender, e);
            UpdateTotal();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (cameraList.SelectedItems.Count == 0)
                return;

            var trackCamera = trackCameras.FirstOrDefault(tc => tc.TrackName == (string)trackList.SelectedItem
                && tc.CameraName == cameraList.SelectedItems[0].Text);

            int number = 0;
            int.TryParse(ratioTextBox.Text, out number);
            if (number > 99)
                number = 0;

            trackCamera.Ratio = number;
            cameraList.SelectedItems[0].SubItems[1].Text = number.ToString();

            UpdateTotal();
        }

        private void UpdateTotal()
        {
            int total = 0;
            foreach (var tc in trackCameras.Where(tc => tc.TrackName == (string)trackList.SelectedItem))
            {
                total += tc.Ratio;
            }

            totalRatio.Text = total.ToString();
            TotalErrorLabel.Visible = total > 100;
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
    }
}
