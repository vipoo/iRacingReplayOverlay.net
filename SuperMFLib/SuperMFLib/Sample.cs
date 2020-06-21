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
    public class Sample : COMDisposable<IMFSample>
	{
		public Sample(IMFSample instance) :base(instance) { }

        public static Sample Create()
        {
            IMFSample instance;
            MFExtern.MFCreateSample(out instance).Hr();

            return new Sample(instance);
        }

		public bool Discontinuity
		{
			get 
			{
				int result;
				instance.GetUINT32(MFAttributesClsid.MFSampleExtension_Discontinuity, out result).Hr();
				return result != 0;
			}
			set
            {
                instance.SetUINT32 (MFAttributesClsid.MFSampleExtension_Discontinuity, value ? 1 : 0).Hr ();
            }
		}

        public MFMediaBuffer ConvertToContiguousBuffer()
        {
            IMFMediaBuffer mediaBuffer;
            instance.ConvertToContiguousBuffer(out mediaBuffer);

            return new MFMediaBuffer(mediaBuffer);
        }

        public long SampleTime
        {
            get
            {
                return GetSampleTime();
            }
            set
            {
                SetSampleTime(value);
            }
        }

        public void SetSampleTime(long p)
        {
            this.instance.SetSampleTime(p);
        }

        public long GetSampleTime()
        {
            long result;
            this.instance.GetSampleTime(out result);
            return result;
        }
    }
}
