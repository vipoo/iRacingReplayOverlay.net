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
        Action<string, string> onError;

        public void SetOnError(Action<string, string> onError)
        {
            this.onError = onError;
        }

        public void SetPluginFileName(string value)
        {
            PluginProxy = new PluginProxy(value);

            PluginProxy.SetWeekendInfo(new SessionData._WeekendInfo
            {
                TrackDisplayName = "Sample Track Name",
                TrackCity = "Track City",
                TrackCountry = "Track Country"
            });

            var x = new[]
            {
                new SessionData._SessionInfo._Sessions._ResultsPositions
                {
                     Position = 1,
                     CarIdx = 1
                }
            };

            PluginProxy.SetQualifyingResults(x);

            PluginProxy.SetCompetingDrivers(new[] {
                new SessionData._DriverInfo._Drivers
                {
                    CarIdx = 0
                },
                new SessionData._DriverInfo._Drivers
                {
                    CarIdx = 1,
                    UserName = "John Smith"
                },
            });
        }

        public ImageViewer()
        {
            InitializeComponent();
        }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                base.OnPaintBackground(e);

                e.Graphics.ScaleTransform(this.ClientSize.Width / 1920f, this.ClientSize.Height / 1080f);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                DrawAction(e.Graphics);
            }
            catch(Exception ex)
            {
                if (onError != null)
                    onError(ex.Message, ex.StackTrace);
            }
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

        private void ImageViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
