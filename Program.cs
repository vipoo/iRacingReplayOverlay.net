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
using System.Drawing.Drawing2D;
using System.Linq;

namespace iRacingReplayOverlay.net
{
	class Program
    {
        static void Main(string[] args)
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
            }
        }

		static Dictionary<SourceStream, SinkStream> streamMapping = new Dictionary<SourceStream, SinkStream> ();

		static void ConnectSourceToSink(SourceReader sourceReader, SinkWriter sinkWriter)
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

		public class Progressor
		{
			private int progress = 0;
			private int progressBarTicks = 52;

			public Progressor()
			{
				Console.Write( "            0%-------20%-------40%-------60%-------80%-------100%\n" );
				Console.Write("Writing:    ");
			}

			public void Update(int percentageCompleted)
			{
				var currentProgress = (Math.Min(percentageCompleted, 100) * progressBarTicks / 100);

				while(progress <= currentProgress)
				{
					progress++;
					Console.Write( "*" );
				}
			}
		}

		static void ProcessSamples(SourceReader sourceReader, SinkWriter sinkWriter)
		{
			var progressor = new Progressor();

			using(sinkWriter.BeginWriting())
				foreach (var sample in sourceReader.Samples())
					ProcessSample(sample, progressor);
		}

		private static void ProcessSample(SourceReaderSample sample, Progressor progressor)
		{
			var sinkStream = streamMapping [sample.Stream];

			ProcessMediaChange(sinkStream, sample);

			WriteStream(sinkStream, sample);

			SendStreamTick(sinkStream, sample);

			UpdateProgress(sample, progressor);
		}

		static void ProcessMediaChange(SinkStream sinkStream, SourceReaderSample sample)
		{
			if (sample.Flags.CurrentMediaTypeChanged)
				sinkStream.InputMediaType = sample.Stream.CurrentMediaType;
		}

		static void WriteStream(SinkStream sinkStream, SourceReaderSample sample)
		{
			if(sample.Sample == null)
				return;

			if (sample.Count == 0)
				sample.Sample.Discontinuity = true;

			if (sample.Stream.CurrentMediaType.IsVideo)
				ApplyBitmap(sample);

			sinkStream.WriteSample(sample.Sample);
		}

		static void SendStreamTick(SinkStream sinkStream, SourceReaderSample sample)
		{
			if(sample.Flags.StreamTick)
				sinkStream.SendStreamTick(sample.Timestamp);
		}

		static void UpdateProgress(SourceReaderSample sample, Progressor progressor)
		{
			if( sample.Timestamp != 0 )
				progressor.Update(sample.PercentageCompleted);
		}

		private static void ApplyBitmap(SourceReaderSample sample)
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

		public static MediaType CreateTargetAudioMediaType(MediaType nativeMediaType)
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

		public static MediaType CreateTargetVideoMediaType(MediaType nativeMediaType)
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
    }
}
