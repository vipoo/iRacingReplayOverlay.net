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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaFoundation.Net
{
    public class MFMediaBuffer : COMDisposable<IMFMediaBuffer>
    {
        public MFMediaBuffer(IMFMediaBuffer instance) : base(instance) { }

        public static MFMediaBuffer CreateMemoryBuffer(int byteCount)
        {
            IMFMediaBuffer instance;
            MFExtern.MFCreateMemoryBuffer(byteCount, out instance).Hr();

            return new MFMediaBuffer(instance);
        }

        public LockedMediaBuffer Lock()
        {
            LockedMediaBuffer result = new LockedMediaBuffer { mediaBuffer = this };

            instance.Lock(out result.buffer, out result.maxLength, out result.currentLength).Hr();

            return result;
        }

		public class LockedMediaBuffer : IDisposable
        {
            internal MFMediaBuffer mediaBuffer;
            internal IntPtr buffer;
            internal int maxLength;
            internal int currentLength;

            public void Dispose()
            {
                mediaBuffer.instance.Unlock().Hr();
            }

            public IntPtr Buffer { get { return buffer; } }
        }

    }
}
