// This file is part of iRacingReplayOverlay.
//
// Copyright 2016 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net

using iRacingSDK.Support;
using System;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    partial class UsageDataRequest : Form
    {
        public UsageDataRequest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.SendUsageData = this.sendUsageData.Checked;
            Settings.Default.Save();

           
            TraceInfo.WriteLine(Settings.Default.SendUsageData ? "User has accepted usage data submission" : "user has declined usage data submission");
            this.Close();
        }

        private void UsageDataRequest_Load(object sender, EventArgs e)
        {
            var a = new Random().Next(int.MaxValue);
            var b = new Random().Next(int.MaxValue);
            var trackingId = (long)a * (long)int.MaxValue + (long)b;
            Settings.Default.TrackingID = trackingId.ToString();
            Settings.Default.Save();
            this.trackingId.Text = Settings.Default.TrackingID;
        }
    }
}
