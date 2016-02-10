using iRacingSDK;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace iRacingDirector.Plugin.OverlaySupport
{
    
    public class Drawer : MarshalByRefObject
    {
        public Graphics g;
        public string PluginFileName;

        public void _IntroFlashCard()
        {
            var p = new iRacingDirector.PluginProxy(PluginFileName);

            p.SetGraphics(g);
            p.SetWeekendInfo(new SessionData._WeekendInfo
            {
                TrackDisplayName = "Sample Track Name",
                TrackCity = "Track City",
                TrackCountry = "Track Country"
            });
            p.SetQualifyingResults(new SessionData._SessionInfo._Sessions._ResultsPositions[0]);

            p.DrawIntroFlashCard(0, 0);
        }
    }

    public partial class Main : Form
    {
        private ImageViewer frm;
        private AppDomain domain;

        public Main()
        {
            InitializeComponent();
            frm = new ImageViewer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";

            if (backgroundTestImageFileName.Text != "")
            {
                fbd.FileName = Path.GetFileName(backgroundTestImageFileName.Text);
                fbd.InitialDirectory = Path.GetDirectoryName(backgroundTestImageFileName.Text);
            }

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                backgroundTestImageFileName.Text = fbd.FileName;
                backgroundTestImageFileName_Leave(null, null);
            }

            frm.BackgroundImage = Image.FromFile(backgroundTestImageFileName.Text);
            frm.Show();
            frm.Activate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frm.ClientSize = new Size(1920, 1080);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frm.ClientSize = new Size(1920 / 2 , 1080 / 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frm.ClientSize = new Size(1920 / 3, 1080 / 3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frm.drawAction = g =>
            {
                var d = new Drawer { g = g, PluginFileName = this.pluginAssemblyFileName.Text };
                domain.DoCallBack(d._IntroFlashCard);
            };
            frm.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "Assembly (*.dll)|*.dll";
            if (pluginAssemblyFileName.Text != "")
            {
                fbd.FileName = Path.GetFileName(pluginAssemblyFileName.Text);
                fbd.InitialDirectory = Path.GetDirectoryName(pluginAssemblyFileName.Text);
            }
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                pluginAssemblyFileName.Text = fbd.FileName;
                pluginAssemblyFileName_Leave(null, null);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            pluginAssemblyFileName.Text = Properties.Settings.Default.PluginAssemblyFileName;
            backgroundTestImageFileName.Text = Properties.Settings.Default.BackgroundTestImageFileName;

            if(File.Exists(backgroundTestImageFileName.Text))
                frm.BackgroundImage = Image.FromFile(backgroundTestImageFileName.Text);


            var myLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var info = new AppDomainSetup();
            domain = AppDomain.CreateDomain("TranscodingDomain", null, info);
        }

        private void pluginAssemblyFileName_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.PluginAssemblyFileName = pluginAssemblyFileName.Text;
            Properties.Settings.Default.Save();
        }

        private void backgroundTestImageFileName_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackgroundTestImageFileName = backgroundTestImageFileName.Text;
            Properties.Settings.Default.Save();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            frm.Show();
        }
    }
}
