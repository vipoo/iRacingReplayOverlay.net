using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iRacingReplayOverlay.net
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void TranscodeVideo_Click(object sender, EventArgs e)
        {
            Transcoder.TranscodeVideo();
        }
    }
}
