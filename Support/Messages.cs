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
                Write(message);
        }

        public override void WriteLine(string message, string category)
        {
            if (category == INFO)
                WriteLine(message);
        }

        public override void Write(string message)
        {
            context.Post(ignore =>
            {
                textBox.Text = textBox.Text + message;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }, null);
        }

        public override void WriteLine(string message)
        {
            Write(message);
            Write("\r\n");
        }
    }
}
