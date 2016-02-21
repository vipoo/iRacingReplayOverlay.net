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
        PluginProxy PluginProxy;
        Action<string, string> onError;

        public void SetOnError(Action<string, string> onError)
        {
            this.onError = onError;
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
    }
}
