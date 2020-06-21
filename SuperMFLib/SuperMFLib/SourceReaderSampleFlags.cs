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
using System.Collections.Generic;

namespace MediaFoundation.Net
{
	public struct SourceReaderSampleFlags
	{
		readonly int flags;

		public SourceReaderSampleFlags (int flags)
		{
			this.flags = flags;
		}

		public bool CurrentMediaTypeChanged
		{
			get { return (flags & (int)MF_SOURCE_READER_FLAG.CurrentMediaTypeChanged) != 0; }
		}


        public bool StreamTick
        {
            get { return (flags & (int)MF_SOURCE_READER_FLAG.StreamTick) != 0; }
        }

        public bool EndOfStream
        {
            get { return (flags & (int)MF_SOURCE_READER_FLAG.EndOfStream) != 0; }
        }
    }	
}
