using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace iRacingDirector.Plugin.Tester
{
    public partial class Main : Form
    {
        DomainForm domainForm;

        public Main()
        {
            InitializeComponent();
        }

        void browseBackgroundImageButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
            };
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

            domainForm.SetBackgroundImage(backgroundTestImageFileName.Text);
        }

        private void fullSizeButton_Click(object sender, EventArgs e)
        {
            domainForm.SetClientSize(new Size(1920, 1080));
        }

        private void halfSizeButton_Click(object sender, EventArgs e)
        {
            domainForm.SetClientSize(new Size(1920 / 2 , 1080 / 2));
        }

        private void thirdSizeButton_Click(object sender, EventArgs e)
        {
            domainForm.SetClientSize(new Size(1920 / 3, 1080 / 3));
        }

        private void introFlashCardButton_Click(object sender, EventArgs e)
        {
            domainForm.Recreate();
            domainForm.SetPluginFileName(this.pluginAssemblyFileName.Text);
            domainForm.SetBackgroundImage(backgroundTestImageFileName.Text);
            domainForm.SetClientSize(new Size(1920 / 3, 1080 / 3));
            domainForm.SetPosition(this.Left, this.Top + this.Height);
        }

        private void browsePluginButton_Click(object sender, EventArgs e)
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
            this.Left = Screen.PrimaryScreen.WorkingArea.Width / 24;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height / 16;

            pluginAssemblyFileName.Text = Properties.Settings.Default.PluginAssemblyFileName;
            backgroundTestImageFileName.Text = Properties.Settings.Default.BackgroundTestImageFileName;

            if (Properties.Settings.Default.SampleSessionDataFileName == null || Properties.Settings.Default.SampleSessionDataFileName == "") {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                sampleSessionDataFileName.Text = Path.Combine(dir, "session-data.json");
            }
            else
                sampleSessionDataFileName.Text = Properties.Settings.Default.SampleSessionDataFileName;

            domainForm = DomainForm.CreateRemote();
            domainForm.SetSessionDataFileName(sampleSessionDataFileName.Text);
            domainForm.SetPluginFileName(pluginAssemblyFileName.Text);
            domainForm.SetOnError((s, m) => {
                this.errorDetailsTextBox.Text = s + "\r\n" + m;
            });
            if (File.Exists(backgroundTestImageFileName.Text))
            {
                domainForm.SetBackgroundImage(backgroundTestImageFileName.Text);
                domainForm.SetClientSize(new Size(1920 / 3, 1080 / 3));
                domainForm.SetPosition(this.Left, this.Top + this.Height);
            }
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

        private void browserSampleSessionDataButton_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.Filter = "Assembly (*.json)|*.json";
            if (sampleSessionDataFileName.Text != "")
            {
                fbd.FileName = Path.GetFileName(sampleSessionDataFileName.Text);
                fbd.InitialDirectory = Path.GetDirectoryName(sampleSessionDataFileName.Text);
            }
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                sampleSessionDataFileName.Text = fbd.FileName;
                sampleSessionDataFileName_Leave(null, null);
            }
        }

        private void sampleSessionDataFileName_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.SampleSessionDataFileName = sampleSessionDataFileName.Text;
            Properties.Settings.Default.Save();

            domainForm.SetSessionDataFileName(sampleSessionDataFileName.Text);
        }
    }
}
