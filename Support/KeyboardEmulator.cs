using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayDirector.Support
{
    class KeyboardEmulator
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        static public void SendKeyStrokes()
        {

            Byte menuKey = (Byte)Keys.Menu;
            Byte F9Key = (Byte)Keys.F9;

            keybd_event(menuKey, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(F9Key, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(F9Key, 0, 0x02, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(menuKey, 0, 0x02, UIntPtr.Zero);
        }

    }
}
