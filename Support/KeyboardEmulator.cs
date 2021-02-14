using System;
using System.Windows.Forms;
using WK.Libraries.HotkeyListenerNS;
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

        static public void SendKeyStrokes(Hotkey hotkey)
        {
            Keys modifierKey = hotkey.Modifiers;
            Byte modifier = (Byte)modifierKey; 
            Byte key = (Byte)(Keys)hotkey.KeyCode; 

            keybd_event(modifierKey, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(key, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(key, 0, 0x02, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(modifierKey, 0, 0x02, UIntPtr.Zero);
        }

    }
}
