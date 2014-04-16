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

using MediaFoundation;
using MediaFoundation.Net;
using MediaFoundation.Transform;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace iRacingReplayOverlay.net
{
	public class OverlayWorker
    {
        public delegate void _Progress(int percentage);

        SynchronizationContext uiContext;
        Thread worker = null;
        bool requestCancel = false;
        
        public event _Progress Progress;
        public event Action Completed;

        public void TranscodeVideo()
		{
			if(worker != null)
				return;

            uiContext = SynchronizationContext.Current;
            requestCancel = false;

			worker = new Thread(Transcode);
            worker.Start(uiContext);
		}

		void Transcode(object state)
		{
			try
			{
                var transcoder = new Transcoder
                {
                    SourceFile = @"C:\Users\dean\Documents\iRacingShort.mp4",
                    DestinationFile = @"C:\Users\dean\documents\output.wmv"
                };

                foreach( var frame in transcoder.Frames())
                {
                    Overlayer.Leaderboard(frame.Timestamp, frame.Graphic);

                    UpdateProgress(frame);
                    if (requestCancel)
                        break;
                }

                if (Completed != null)
                    uiContext.Post(ignored => Completed(), null);
			}
			finally
			{
				worker = null;
			}
        }

		void UpdateProgress(SourceReaderSampleWithBitmap sample)
		{
            if (sample.Timestamp != 0 && Progress != null)
                uiContext.Post(state => Progress(sample.PercentageCompleted), null);
		}

        internal void Cancel()
        {
            requestCancel = true;
        }
    }
}
