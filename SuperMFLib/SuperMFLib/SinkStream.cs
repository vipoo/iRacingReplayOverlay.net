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

namespace MediaFoundation.Net
{
	public struct SinkStream 
	{
		readonly SinkWriter sinkWriter;
		readonly int index;

		public SinkStream (SinkWriter sinkWriter, int streamIndex) 
		{
			this.sinkWriter = sinkWriter;
			this.index = streamIndex;
		}

        public void SetInputMediaType(MediaType value, Attributes attributes)
        {
            sinkWriter.instance.SetInputMediaType(index, value.instance, attributes.instance).Hr();
        }

		public MediaType InputMediaType
		{
			set
			{
				sinkWriter.instance.SetInputMediaType (index, value.instance, null).Hr ();
			}
		}

        public void WriteSample(Sample sample)
        {
            sinkWriter.instance.WriteSample(index, sample.instance).Hr();
        }

        public void SendStreamTick(long timestamp)
        {
            sinkWriter.instance.SendStreamTick(index, timestamp).Hr();
        }
    }
}
