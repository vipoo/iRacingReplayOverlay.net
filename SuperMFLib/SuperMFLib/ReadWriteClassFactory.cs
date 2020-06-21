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

using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using System;

namespace MediaFoundation.Net
{
	public class ReadWriteClassFactory : COMDisposable<IMFReadWriteClassFactory>
    {
		public ReadWriteClassFactory() : base( (IMFReadWriteClassFactory)new MFReadWriteClassFactory())  {  }

        public SourceReader CreateSourceReaderFromURL(string url, Attributes attributes)
        {
            object tmp;

            var hr = instance.CreateInstanceFromURL(CLSID.CLSID_MFSourceReader, url, attributes == null ? null : attributes.instance, typeof(IMFSourceReader).GUID, out tmp);
            MFError.ThrowExceptionForHR(hr);
            return new SourceReader((IMFSourceReader)tmp);
        }

        public int CreateInstanceFromObject(Guid clsid, object punkObject, IMFAttributes pAttributes, Guid riid, out object ppvObject)
        {
            throw new NotImplementedException();
        }

        public SinkWriter CreateSinkWriterFromURL(string url, Attributes attributes)
        {
            object tmp;

            var hr = instance.CreateInstanceFromURL(CLSID.CLSID_MFSinkWriter, url, attributes.instance, typeof(IMFSinkWriter).GUID, out tmp);
            MFError.ThrowExceptionForHR(hr);
            return new SinkWriter((IMFSinkWriter)tmp);
        }
    }    
}
