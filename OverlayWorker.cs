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
using System.IO;
using System.Linq;
using System.Threading;

namespace iRacingReplayOverlay.net
{
	public class OverlayWorker
    {
        public delegate void _Progress(long count, long duration);

        SynchronizationContext uiContext;
        Thread worker = null;
        bool requestCancel = false;
        
        public event _Progress Progress;
        public event Action Completed;
        public event Action ReadFramesCompleted;
        private string sourceFile;
        private string destinationFile;
        private string gameDataFile;

        public void TranscodeVideo(string sourceFile, string destinationFile, string gameDataFile)
		{
			if(worker != null)
				return;

            this.sourceFile = sourceFile;
            this.destinationFile = destinationFile;
            this.gameDataFile = gameDataFile;

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
                    SourceFile = sourceFile,
                    DestinationFile = destinationFile
                };

                var driverNickNames = new Dictionary<string, string>
                {
                    {"Dean Netherton", "Dino"},
                    {"Paul Tarjavaara", "Tarj"}
                };

                var leaderBoard = new LeaderBoard
                {
                    TimingSamples = File
                        .ReadAllLines(gameDataFile)
                        .Skip(1)
                        .Select(line => line.Split(','))
                        .Select(line => new TimingSample { StartTime = long.Parse(line[0]), Drivers = line[1].Split('|'), DriverNickNames = driverNickNames })
                        .ToArray()
                };

                foreach( var frame in transcoder.Frames())
                {
                    if (frame.Flags.EndOfStream && ReadFramesCompleted != null)
                        uiContext.Post(ignored => ReadFramesCompleted(), null);
                    else
                    {
                        leaderBoard.Overlay(frame.Graphic, frame.Timestamp);

                        UpdateProgress(frame);
                    }

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
                uiContext.Post(state => Progress(sample.Timestamp, sample.Duration), null);
		}

        internal void Cancel()
        {
            requestCancel = true;
        }
    }
}
