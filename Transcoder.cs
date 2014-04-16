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

        Dictionary<SourceStream, SinkStream> streamMapping;
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
            streamMapping = new Dictionary<SourceStream, SinkStream>();
            requestCancel = false;

			worker = new Thread(Transcode);
            worker.Start(uiContext);
		}

		void Transcode(object state)
		{
			try
			{
                using( MFSystem.Start() )
                {
                    var readWriteFactory = new ReadWriteClassFactory();
            
				    var attributes = new Attributes {
					    ReadWriterEnableHardwareTransforms = true,
					    SourceReaderEnableVideoProcessing = true
				    };

                    var sourceReader = readWriteFactory.CreateSourceReaderFromURL(@"C:\Users\dean\Documents\iRacingShort.mp4", attributes);
                    var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(@"C:\Users\dean\documents\output.wmv", attributes);

                    ConnectSourceToSink(sourceReader, sinkWriter);

				    ProcessSamples(sourceReader, sinkWriter);

                    if( Completed != null )
                        uiContext.Post(ignored => Completed(), null);
                }
			}
			finally
			{
				worker = null;
			}
        }

		void ConnectSourceToSink(SourceReader sourceReader, SinkWriter sinkWriter)
		{
			foreach (var stream in sourceReader.Streams.Where(s => s.IsSelected))
			{
				var sourceStream = stream;

				var nativeMediaType = sourceStream.NativeMediaType;

				var isVideo = nativeMediaType.IsVideo;
				var isAudio = nativeMediaType.IsAudio;

				if( !isAudio && !isVideo)
					throw new Exception("Unknown stream type");

				var targetType = isAudio ? CreateTargetAudioMediaType(nativeMediaType) : CreateTargetVideoMediaType(nativeMediaType);

				var sinkStream = sinkWriter.AddStream(targetType);
				streamMapping.Add(sourceStream, sinkStream);

				var mediaType = isAudio
					? new MediaType() { MajorType = MFMediaType.Audio, SubType = MFMediaType.Float }
					: new MediaType() { MajorType = MFMediaType.Video, SubType = MFMediaType.RGB32 };

                sourceStream.CurrentMediaType = mediaType;
                sinkStream.InputMediaType = sourceStream.CurrentMediaType;
			}
		}

		void ProcessSamples(SourceReader sourceReader, SinkWriter sinkWriter)
		{
			using(sinkWriter.BeginWriting())
                foreach (var sample in sourceReader.Samples())
                {
                    ProcessSample(sample);
                    if (requestCancel)
                        return;
                }
		}

		void ProcessSample(SourceReaderSample sample)
		{
			var sinkStream = streamMapping [sample.Stream];

			ProcessMediaChange(sinkStream, sample);

			WriteStream(sinkStream, sample);

			SendStreamTick(sinkStream, sample);

			UpdateProgress(sample);
		}

		void ProcessMediaChange(SinkStream sinkStream, SourceReaderSample sample)
		{
			if (sample.Flags.CurrentMediaTypeChanged)
				sinkStream.InputMediaType = sample.Stream.CurrentMediaType;
		}

		void WriteStream(SinkStream sinkStream, SourceReaderSample sample)
		{
			if(sample.Sample == null)
				return;

			if (sample.Count == 0)
				sample.Sample.Discontinuity = true;

			if (sample.Stream.CurrentMediaType.IsVideo)
				ApplyBitmap(sample);

			sinkStream.WriteSample(sample.Sample);
		}

		void SendStreamTick(SinkStream sinkStream, SourceReaderSample sample)
		{
			if(sample.Flags.StreamTick)
				sinkStream.SendStreamTick(sample.Timestamp);
		}

		void UpdateProgress(SourceReaderSample sample)
		{
            if (sample.Timestamp != 0 && Progress != null)
                uiContext.Post(state => Progress(sample.PercentageCompleted), null);
		}

		void ApplyBitmap(SourceReaderSample sample)
        {
			using (var buffer = sample.Sample.ConvertToContiguousBuffer())
            {
                using (var data = buffer.Lock())
                {
					var b = new Bitmap(1920, 1080, 1920 * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, data.Buffer);

					Graphics g = Graphics.FromImage(b);

					Overlayer.Leaderboard(sample.Timestamp, g);

					g.Flush();
                }
            }
        }

		static Guid TARGET_AUDIO_FORMAT = MFMediaType.WMAudioV9;
		static Guid TARGET_VIDEO_FORMAT = MFMediaType.WMV3;

		MediaType CreateTargetAudioMediaType(MediaType nativeMediaType)
		{
			var numberOfChannels = nativeMediaType.AudioNumberOfChannels;
			var sampleRate = nativeMediaType.AudioSamplesPerSecond;

			var availableTypes = MFSystem.TranscodeGetAudioOutputAvailableTypes (TARGET_AUDIO_FORMAT, MFT_EnumFlag.All);

			var type = availableTypes
				.FirstOrDefault (t => t.AudioNumberOfChannels == numberOfChannels && t.AudioSamplesPerSecond == sampleRate);

			if( type != null )
				return new MediaType (type);

			throw new Exception ("Unable to find target audio format");
		}

		MediaType CreateTargetVideoMediaType(MediaType nativeMediaType)
		{
			var size = nativeMediaType.FrameSize;
			var rate = nativeMediaType.FrameRate;
			var aspect = nativeMediaType.AspectRatio;
			var bitRate = nativeMediaType.BitRate;

			var mediaType = new MediaType () {
				MajorType = MFMediaType.Video,
				SubType = TARGET_VIDEO_FORMAT,
				FrameSize = size,
				FrameRate = rate,
				AspectRatio = aspect,
				BitRate = bitRate,
				InterlaceMode = MFVideoInterlaceMode.Progressive
			};

			return mediaType;
		}

        internal void Cancel()
        {
            requestCancel = true;
        }
    }
}
