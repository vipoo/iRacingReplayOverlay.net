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
    public class SourceResolver : COMDisposable<IMFSourceResolver>
    {
        public SourceResolver(IMFSourceResolver instance) : base(instance) { }

        public static SourceResolver Create()
        {
            IMFSourceResolver instance;
            MFExtern.MFCreateSourceResolver(out instance);

            return new SourceResolver(instance);
        }

        public MediaSource CreateObjectFromURL( string url, MFResolution flags )
        {
            var objectType = MFObjectType.Invalid;
            object mediaSourceObject;

            instance.CreateObjectFromURL(url, flags, null, out objectType, out mediaSourceObject).Hr();

            return new MediaSource((IMFMediaSource)mediaSourceObject);
        }
    }
}
