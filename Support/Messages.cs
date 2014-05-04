using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iRacingReplayOverlay.Support
{
    public class MyListener : TraceListener
    {
        public const string INFO = "INFO";

        TextBox textBox;
        SynchronizationContext context;

        public MyListener(TextBox textBox)
        {
            context = SynchronizationContext.Current;
            this.textBox = textBox;
        }

        public override void Write(string message, string category)
        {
            if (category == INFO)
                WriteInfo(message);
        }

        public override void WriteLine(string message, string category)
        {
            if (category == INFO)
                WriteInfoLine(message);
        }

        public void WriteInfo(string message)
        {
            context.Post(ignore =>
            {
                textBox.Text = textBox.Text + message;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }, null);
        }

        public void WriteInfoLine(string message)
        {
            WriteInfo(message);
            WriteInfo("\r\n");
        }

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }
    }
}
