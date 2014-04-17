// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Win32;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using Kernel32;

namespace iRacingReplayOverlay.net
{
	public delegate void KeyReleasedEvent(Keys keyCode);

	class KeyboardHook : IDisposable
	{
		public event KeyReleasedEvent KeyReleased;

		IntPtr hookID;
        Hooks.LowLevelKeyboardProc hookDelegate;

		public void Start()
		{
            hookDelegate = new Hooks.LowLevelKeyboardProc(GlobalKeyEvent);
			hookID = SetHook(hookDelegate);
		}

		public void Dispose()
		{
			if(hookID == IntPtr.Zero)
				return;

			if( !Hooks.UnhookWindowsHookEx(hookID) )
				throw new Win32Exception(Marshal.GetLastWin32Error());

            hookDelegate = null;
			hookID = IntPtr.Zero;
		}

		IntPtr GlobalKeyEvent(int nCode, IntPtr wParam, IntPtr lParam)
		{
			try
			{    
				if (wParam == (IntPtr)Hooks.WM_SYSKEYDOWN)
				{
					int vkCode = Marshal.ReadInt32(lParam);
					if( KeyReleased != null)
						KeyReleased((Keys)vkCode);
				}
			}
			catch(Exception e)
			{
				//Probably not wise to through exception through to windows api hook
				Console.WriteLine("Error in windows hook processing " + e.Message);
			}

			return Hooks.CallNextHookEx(hookID, nCode, wParam, lParam);
		}

        static IntPtr SetHook(Hooks.LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
				using (ProcessModule curModule = curProcess.MainModule)
				{
					var hookId = Hooks.SetWindowsHookEx(Hooks.WH_KEYBOARD_LL, proc, Dll.GetModuleHandle(curModule.ModuleName), 0);
					if( hookId == IntPtr.Zero)
						throw new Win32Exception(Marshal.GetLastWin32Error());

					return hookId;
				}
		}
	}
}
