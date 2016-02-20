using iRacingSDK;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace iRacingDirector.Plugin.Tester
{
    public partial class ImageViewer : Form
    {
        PluginProxy PluginProxy;

        public void SetPluginFileName(string value)
        {
            PluginProxy = new PluginProxy(value);

            PluginProxy.SetWeekendInfo(new SessionData._WeekendInfo
            {
                TrackDisplayName = "Sample Track Name",
                TrackCity = "Track City",
                TrackCountry = "Track Country"
            });
            PluginProxy.SetQualifyingResults(new SessionData._SessionInfo._Sessions._ResultsPositions[0]);

        }

        public ImageViewer()
        {
            InitializeComponent();
        }

        private void ImageViewer_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.ScaleTransform(this.ClientSize.Width / 1920f, this.ClientSize.Height / 1080f);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            DrawAction(e.Graphics);
        }

        void DrawAction(Graphics g)
        {
            if (PluginProxy == null)
                return;

            PluginProxy.SetGraphics(g);
            PluginProxy.DrawIntroFlashCard(0, 0);
        }

        private void ImageViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
