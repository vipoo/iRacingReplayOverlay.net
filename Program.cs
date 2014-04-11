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

		static void ProcessSamples(SourceReader sourceReader, SinkWriter sinkWriter)
		{
			var progress = 0;
			var progressBarTicks = 52;

			Console.Write( "            0%-------20%-------40%-------60%-------80%-------100%\n" );
			Console.Write("Writing:    ");

			var duration = sourceReader.MediaSource.Duration;

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

					if(sample.Flags.StreamTick)
					{
						sinkStream.SendStreamTick(sample.Timestamp);
						Console.WriteLine("Time tick " +sample.Timestamp);
					}

					if( sample.Timestamp != 0 )
					{
						var percentComplete = Math.Max(sample.Timestamp, 0) * 100L / (long)duration;

						var currentProgress = (Math.Min(percentComplete, 100) * progressBarTicks / 100);

						while(progress <= currentProgress)
						{
							progress++;
							Console.Write( "*" );
						}
					}
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
