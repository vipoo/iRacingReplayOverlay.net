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

using MediaFoundation;
using MediaFoundation.Net;
using MediaFoundation.Transform;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace iRacingReplayOverlay.net
{
	class SourceReaderSampleWithBitmap : IDisposable
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

				buffer = sample.Sample.ConvertToContiguousBuffer();
				data = buffer.Lock();
				bitmap = new Bitmap(1920, 1080, 1920 * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, data.Buffer);

				graphic = Graphics.FromImage(bitmap);

				return graphic;
            }
        }

		public readonly long Timestamp;
		public readonly int PercentageCompleted;
		readonly SourceReaderSample sample;

        public SourceReaderSampleWithBitmap(SourceReaderSample sample)
        {
			this.Timestamp = sample.Timestamp;
			this.PercentageCompleted = sample.PercentageCompleted;
            this.sample = sample;
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
