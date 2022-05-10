using System;
using System.Windows.Forms;
using WK.Libraries.HotkeyListenerNS;
using iRacingSDK.Support;

namespace iRacingReplayDirector.Support
{
    class KeyboardEmulator
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        //private KeyboardSimulator sim = new KeyboardSimulator();

        static public void SendKeyStrokes(Hotkey hotkey)
        {
            Keys modifierKey = hotkey.Modifiers;
            Byte modifier = (Byte)modifierKey;
            Byte key = (Byte)hotkey.KeyCode;

            TraceDebug.WriteLine("keybd_event sent: modifier = {0} | key = {1}".F(modifier, key));

            //REMARK: After updating to .NET 5.0 values could be easily converted to hex-string using "Convert.toHexString"

            //TraceDebug.WriteLine("Race analysis phase completed. {0} data samples processed with replay speed {1}".F(numberOfDataProcessed, iReplaySpeedForAnalysis));

            keybd_event(modifier, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(key, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(key, 0, 0x02, UIntPtr.Zero);
            System.Threading.Thread.Sleep(700);
            keybd_event(modifier, 0, 0x02, UIntPtr.Zero);
        }

    }
}
