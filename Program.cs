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
            using( MediaFoundation.Net.MFSystem.Start() )
            {
                var readWriteFactory = new ReadWriteClassFactory();
            
                var attributes = new Attributes();

                attributes.ReadWriterEnableHardwareTransforms = true;
                attributes.SourceReaderEnableVideoProcessing = true;

                var sourceReader = readWriteFactory.CreateSourceReaderFromURL(@"C:\Users\dean\Documents\iRacingShort.mp4", attributes);
                var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(@"C:\Users\dean\documents\output.wmv", attributes);

                ConnectSourceToSink(sourceReader, sinkWriter);

				ProcessSamples(sourceReader, sinkWriter);
            }
        }

		public struct StreamInfo
		{
			private readonly SinkStream sinkStream;
			private int sampleCount;

			public StreamInfo(SinkStream sinkStream)
			{
				this.sinkStream = sinkStream;
				this.sampleCount = 0;
			}

			public SinkStream SinkStream
			{
				get
				{
					return sinkStream;
				}
			}

			public int SampleCount
			{
				get
				{
					return sampleCount;
				}
				set
				{
					sampleCount = value;
				}
			}
		}

		static Dictionary<SourceStream, SinkStream> streamMapping = new Dictionary<SourceStream, SinkStream> ();

		static void ConnectSourceToSink(SourceReader sourceReader, SinkWriter sinkWriter)
		{
			var duration = sourceReader.MediaSource.Duration;

			foreach (var stream in sourceReader.Streams.Where(s => s.IsSelected))
			{
				var sourceStream = stream;

				var nativeMediaType = sourceStream.NativeMediaType;

				var isVideo = nativeMediaType.IsVideo;
				var isAudio = nativeMediaType.IsAudio;

				MediaType targetType;

				if (isAudio)
					targetType = CreateTargetAudioMediaType (nativeMediaType);
				else if (isVideo)
					targetType = CreateTargetVideoMediaType (nativeMediaType);
				else
					throw new Exception("Unknown stream type");

				var sinkStream = sinkWriter.AddStream (targetType);
				streamMapping.Add (sourceStream, sinkStream);

				if (isAudio)
				{
                    using (var mediaType = new MediaType() { MajorType = MFMediaType.Audio, SubType = MFMediaType.Float })
                    {
                        sourceStream.CurrentMediaType = mediaType;
                        sinkStream.InputMediaType = sourceStream.CurrentMediaType;
                    }
				}
				else if(isVideo)
				{
                    using (var mediaType = new MediaType() { MajorType = MFMediaType.Video, SubType = MFMediaType.RGB32 })
                    {
                        sourceStream.CurrentMediaType = mediaType;
                        sinkStream.InputMediaType = sourceStream.CurrentMediaType;
                    }
				}
			}
		}

		static void ProcessSamples(SourceReader sourceReader, SinkWriter sinkWriter)
		{
			using (sinkWriter.BeginWriting ())
			{
				foreach (var sample in sourceReader.Samples())
				{
					var sinkStream = streamMapping [sample.Stream];

					if (sample.Flags.CurrentMediaTypeChanged)
						sinkStream.InputMediaType = sample.Stream.CurrentMediaType;

                    if (sample.Sample != null)
                    {
                        if (sample.Count == 0)
                            sample.Sample.Discontinuity = true;

                        if (sample.Stream.CurrentMediaType.IsVideo)
                            ApplyBitmap(sample.Sample);

                        sinkStream.WriteSample(sample.Sample);
                    }

                    if (sample.Flags.StreamTick)
                        sinkStream.SendStreamTick(sample.Timestamp);
				}
			}
		}

        private static void ApplyBitmap(Sample sample)
        {
            using (var buffer = sample.ConvertToContiguousBuffer())
            {
                using (var data = buffer.Lock())
                {
                    var b = new Bitmap(1920, 1080, 1920 * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, data.Buffer);

                    Graphics g = Graphics.FromImage(b);

                    Point p = new Point(400, 400);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawString("Does my text appear", new Font("Tahoma", 64), Brushes.Black, p);
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
