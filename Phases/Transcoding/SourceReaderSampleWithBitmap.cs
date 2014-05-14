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
//

using MediaFoundation.Net;
using System;
using System.Drawing;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    public class SourceReaderSampleWithBitmap : SourceReaderSample, IDisposable
    {
		Graphics graphic;
		MFMediaBuffer buffer;
		MFMediaBuffer.LockedMediaBuffer data;
		Bitmap bitmap;

        public Graphics Graphic
        {
            get
            {
				if(graphic != null)
					return graphic;

                var size = Stream.CurrentMediaType.FrameSize;

				buffer = Sample.ConvertToContiguousBuffer();
				data = buffer.Lock();
				bitmap = new Bitmap(size.Width, size.Height, size.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, data.Buffer);

				graphic = Graphics.FromImage(bitmap);

                graphic.ScaleTransform((float)size.Width / 1920.0f, (float)size.Height / 1080.0f);

				return graphic;
            }
        }

        public SourceReaderSampleWithBitmap(SourceReaderSample sample)
            :base (sample.Stream, sample.Flags, sample.Timestamp, sample.Duration, sample.Sample, sample.Count)
        {
        }

		public void Dispose()
		{
			if(graphic != null)
				graphic.Dispose();

			if(bitmap != null)
				bitmap.Dispose();

			if(data!=null)
				data.Dispose();

			if(buffer != null)
				buffer.Dispose();
		}
    }    
}
