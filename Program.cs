using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFoundation.Transform;
using System.Drawing;


namespace iRacingReplayOverlay.net
{
    class Program
    {
        static void Main(string[] args)
        {
            using( MediaFoundation.Net.MFSystem.Start() )
            {
                var readWriteFactory = new ReadWriteClassFactory();
            
                var attributes = new Attributes(1);

                attributes.ReadWriterEnableHardwareTransforms = false;
                attributes.SourceReaderEanbleVideoProcessing = true;

                var sourceReader = readWriteFactory.CreateSourceReaderFromURL(@"C:\Users\dean\Documents\iRacingShort.mp4", attributes);
                var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(@"C:\Users\dean\documents\output.wmv", attributes);

				GetDuration (sourceReader, sinkWriter);

				sinkWriter.Dispose();
            }
        }

		static void GetDuration(SourceReader sourceReader, SinkWriter sinkWriter)
		{
			var duration = sourceReader.MediaSource.Duration;

			Console.WriteLine (duration);

			int count = 0;
			foreach (var s in sourceReader.Streams)
				if (s.IsSelected)
					count++;

			foreach (var stream in sourceReader.Streams.Where(s => s.IsSelected))
			{
				var nativeMediaType = stream.NativeMediaType;

				var isVideo = nativeMediaType.IsVideo;
				var isAudio = nativeMediaType.IsAudio;

				MediaType targetType;

				if (isAudio)
					targetType = CreateTargetAudioMediaType (nativeMediaType);
				else if (isVideo)
					targetType = CreateTargetVideoMediaType (nativeMediaType);
				else
					continue;

				var sinkStream = sinkWriter.AddStream (targetType);

				if (isAudio)
				{
					var mediaType = new MediaType () { MajorType = MFMediaType.Audio,  SubType = MFMediaType.Float };

					stream.CurrentMediaType = mediaType;
					var currentMediaType = stream.CurrentMediaType;
					sinkStream.InputMediaType = currentMediaType;

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
