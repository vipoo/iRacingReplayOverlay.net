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

using MediaFoundation.EVR;
using MediaFoundation.Net;

namespace MediaFoundation
{
    public class VideoDisplayControl : COMDisposable<IMFVideoDisplayControl>
    {
        public VideoDisplayControl(IMFVideoDisplayControl instance) : base(instance) { }

        public static VideoDisplayControl Create(MediaSession mediaSession)
        {
            object instance;

            MFExtern.MFGetService(mediaSession.instance, MFServices.MR_VIDEO_RENDER_SERVICE, typeof(IMFVideoDisplayControl).GUID, out instance).Hr();

            return new VideoDisplayControl((IMFVideoDisplayControl)instance);
        }

        public void RepaintVideo()
        {
            instance.RepaintVideo().Hr();
        }
    }
}
