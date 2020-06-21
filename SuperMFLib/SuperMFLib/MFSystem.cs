// This file is part of SuperMFLib.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/SuperMFLib
//
// SuperMFLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SuperMFLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SuperMFLib.  If not, see <http://www.gnu.org/licenses/>.

using System;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using MediaFoundation.Transform;

namespace MediaFoundation.Net
{
	public class MFSystem : IDisposable
    {
		public static _MFCollection TranscodeGetAudioOutputAvailableTypes (Guid subType, MFT_EnumFlag flags)
		{
			IMFCollection availableTypes;
			MFExtern.MFTranscodeGetAudioOutputAvailableTypes (subType, (int)flags, null, out availableTypes).Hr();

			return new _MFCollection (availableTypes);
		}

        public static MFSystem Start(MFStartup flags = MFStartup.Full)
        {
			MFExtern.MFStartup (0x10070, flags).Hr ();
            return new MFSystem();
        }

        public void Shutdown()
        {
            Dispose();
        }

        public void Dispose()
        {
			MFExtern.MFShutdown ().Hr ();
			GC.SuppressFinalize(this);
        }

		~MFSystem()
		{
			Dispose();
		}
        private MFSystem()
        { }
    }
}
