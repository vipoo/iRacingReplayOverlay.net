using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRacingReplayOverlay
{
    public partial class LogMessages : Form
    {
        public LogMessages()
        {
            InitializeComponent();
        }

        public TextBox TraceMessage
        {
            get
            {
                return this.TraceMessageTextBox;
            }
        }
    }

}
