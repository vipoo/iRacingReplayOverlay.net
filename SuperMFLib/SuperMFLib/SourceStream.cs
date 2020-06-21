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

namespace MediaFoundation.Net
{
    public struct SourceStream
    {
        public readonly SourceReader sourceReader;
        public readonly int index;

        public SourceStream(SourceReader sourceReader, int index)
        {
            this.sourceReader = sourceReader;
            this.index = index;
        }

        public bool IsNull
        {
            get { return sourceReader == null; }
        }

        public bool IsSelected
        {
            get
            {
                bool result;
                sourceReader.instance.GetStreamSelection(index, out result).Hr();
                return result;
            }
        }

        public MediaType CurrentMediaType
        {
            get
            {
                IMFMediaType ppMediaType;

                sourceReader.instance.GetCurrentMediaType(index, out ppMediaType).Hr();

                return new MediaType(ppMediaType);
            }
            set
            {
                sourceReader.instance.SetCurrentMediaType(index, IntPtr.Zero, value.instance).Hr();
            }
        }

        public MediaType NativeMediaType
        {
            get
            {
                IMFMediaType ppMediaType;

                sourceReader.instance.GetNativeMediaType(index, 0, out ppMediaType).Hr();

                return new MediaType(ppMediaType);
            }
        }

        public ulong Duration
        {
            get
            {
                PropVariant result = new PropVariant();
                sourceReader.instance.GetPresentationAttribute(index, MFAttributesClsid.MF_PD_DURATION, result).Hr();
                return result.GetULong();
            }
        }
    }
}
