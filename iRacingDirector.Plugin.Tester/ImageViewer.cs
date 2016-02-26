using iRacingReplayOverlay;
using iRacingSDK;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using YamlDotNet.Serialization;

namespace iRacingDirector.Plugin.Tester
{
    public partial class ImageViewer : Form
    {
        const long OneNanoSecond = 10000000;

        PluginProxy PluginProxy;
        Action<string, string> onError;
        long timestamp = 0;
        public Timer animationTimer { get; private set; }

        public void SetOnError(Action<string, string> onError)
        {
            this.onError = onError;
        }

        public void SetOnAnimationTick(Action<double> onAnimationTick)
        {
            this.onAnimationTick = onAnimationTick;
        }

        public void InitPlugin(string pluginPath, string sessionDataPath)
        {
            PluginProxy = new PluginProxy(pluginPath);

            var rawSessionData = File.ReadAllText(sessionDataPath);
            var deserializer = new Deserializer(ignoreUnmatched: true);
            var input = new StringReader(rawSessionData);
            var result = (SessionData)deserializer.Deserialize(input, typeof(SessionData));

            PluginProxy.SetEventData(result);
        }

        int framesPerSecond;
        public void SetFramesPerSecond(int framesPerSecond)
        {
            this.framesPerSecond = framesPerSecond;
            if (animationTimer == null)
            {
                animationTimer = new Timer();
                animationTimer.Tick += (s, e) => OnAnimate();
            }
            animationTimer.Interval = 1000 / framesPerSecond;
            animationTimer.Start();
            offset = DateTime.Now;
        }

        internal void SetPlaybackSpeed(int value)
        {
            this.playbackSpeed = value;
        }

        const long Duration = OneNanoSecond * 60;
        private DateTime offset;
        private int playbackSpeed = 1;
        private Action<double> onAnimationTick = x => { };

        public ImageViewer()
        {
            InitializeComponent();
        }

        long lastTimerTick = 0;

        private void OnAnimate()
        {
            var period = (DateTime.Now - offset).TotalSeconds;
            offset = DateTime.Now;

            timestamp += (long)(OneNanoSecond * period) * playbackSpeed;
            if (timestamp > Duration)
            {
                timestamp = timestamp % Duration;
                onAnimationTick(Math.Round((float)timestamp / OneNanoSecond, 4));
                lastTimerTick = timestamp;
            }

            if( timestamp - lastTimerTick > (OneNanoSecond / 4))
            {
                onAnimationTick(Math.Round((float)timestamp / OneNanoSecond, 1));
                lastTimerTick = timestamp;
            }

            Refresh();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                e.Graphics.ScaleTransform(this.ClientSize.Width / 1920f, this.ClientSize.Height / 1080f);
                e.Graphics.DrawImage(BackgroundImage, 0, 0);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                DrawAction(e.Graphics);
            }
            catch (Exception ex)
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
            PluginProxy.DrawIntroFlashCard(Duration, timestamp);
        }

        private void ImageViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
