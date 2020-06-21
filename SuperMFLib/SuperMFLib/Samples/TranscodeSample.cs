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

using MediaFoundation;
using MediaFoundation.Net;
using MediaFoundation.Transform;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Samples
{
    class TranscodeSample
    {
        const string shortSourceFile = @"C:\Users\dean\Documents\Bandicam\intro.avi";
        const string mainSourceFile = @"C:\Users\dean\Documents\Bandicam\video16seconds.avi";
        const string destinationFile = @"C:\Users\dean\Documents\Bandicam\sample3.wmv";
        const int VideoBitRate = 3 * 1000000;
        const int AudioBitRate = 48000;

        static Guid TARGET_AUDIO_FORMAT = MFMediaType.WMAudioV9;
        static Guid TARGET_VIDEO_FORMAT = MFMediaType.WMV3;

        public void Process()
        {
            using (MFSystem.Start())
            {
                var readWriteFactory = new ReadWriteClassFactory();

                var attributes = new Attributes
                {
                    ReadWriterEnableHardwareTransforms = true,
                    SourceReaderEnableVideoProcessing = true,
                };

                var destAttributes = new Attributes
                {
                    ReadWriterEnableHardwareTransforms = true,
                    SourceReaderEnableVideoProcessing = true,
                    MaxKeyFrameSpacing = 3000
                };

                var shortSourceReader = readWriteFactory.CreateSourceReaderFromURL(shortSourceFile, attributes);
                var shortSourceReader2 = readWriteFactory.CreateSourceReaderFromURL(shortSourceFile, attributes);
                var mainSourceReader = readWriteFactory.CreateSourceReaderFromURL(mainSourceFile, attributes);
                var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(destinationFile, destAttributes);

                var writeToSink = ConnectStreams(shortSourceReader, shortSourceReader2, mainSourceReader, sinkWriter);

                ProcessSample progress = sample =>
                {
                    if(!sample.Flags.EndOfStream)
                        Trace.WriteLine(string.Format("sourceTime: {0}, sampleTime: {1}", sample.Timestamp.FromNanoToSeconds(), sample.SampleTime.FromNanoToSeconds()));
                    return writeToSink(sample);
                };

                var fadeSegments = AVOperations.FadeIn(AVOperations.FadeOut(progress));

                var editoutMiddle = AVOperations.Cut(9.FromSecondsToNano(), 19.FromSecondsToNano(), fadeSegments);

                var overlay = AVOperations.Overlay(applyOverlay, editoutMiddle);

                var first4Seconds = AVOperations.Cut(0.FromSecondsToNano(), 4.FromSecondsToNano(), progress, progress);

                var readers = AVOperations.Combine(new[] { shortSourceReader, mainSourceReader }, 1.1);

                using (sinkWriter.BeginWriting())
                {
                    AVOperations.StartConcat(readers, overlay,
                            AVOperations.Concat(shortSourceReader, first4Seconds, () => false), () => false);
                }
            }
        }

        void applyOverlay(SourceReaderSampleWithBitmap sample)
        {
            var f = new Font("Artial", 15, FontStyle.Bold);
            sample.Graphic.DrawString("TEST", f, new SolidBrush(Color.Black), new PointF(500, 500));
        }

        ProcessSample ConnectAudioStreams(SourceReader sourceReader, SinkWriter sinkWriter)
        {
            var sourceAudioStream = SetAudioMediaType(sourceReader);

            var sinkAudioStream = AddStream(sinkWriter, sourceAudioStream.CurrentMediaType, CreateTargetAudioMediaType(sourceAudioStream.NativeMediaType));

            return AVOperations.MediaTypeChange(sinkAudioStream, AVOperations.SaveTo(sinkAudioStream));
        }

        private ProcessSample ConnectVideoStreams(SourceReader sourceReader, SinkWriter sinkWriter)
        {
            var sourceVideoStream = SetVideoMediaType(sourceReader);
            var sinkVideoStream = AddStream(sinkWriter, sourceVideoStream.CurrentMediaType, CreateTargetVideoMediaType(sourceVideoStream.NativeMediaType));

            return AVOperations.MediaTypeChange(sinkVideoStream, AVOperations.SaveTo(sinkVideoStream));
        }

        private ProcessSample ConnectStreams(SourceReader shortSourceReader, SourceReader shortSourceReader2,  SourceReader sourceReader, SinkWriter sinkWriter)
        {
            SetAudioMediaType(shortSourceReader);
            SetVideoMediaType(shortSourceReader);

            SetAudioMediaType(shortSourceReader2);
            SetVideoMediaType(shortSourceReader2);

            return AVOperations.SeperateAudioVideo(
                ConnectAudioStreams(sourceReader, sinkWriter),
                ConnectVideoStreams(sourceReader, sinkWriter));
        }

        SourceStream SetAudioMediaType(SourceReader sourceReader)
        {
            var sourceStream = sourceReader.Streams.First(s => s.IsSelected && s.NativeMediaType.IsAudio);

            sourceStream.CurrentMediaType = new MediaType() { MajorType = MFMediaType.Audio, SubType = MFMediaType.PCM };

            return sourceStream;
        }

        SourceStream SetVideoMediaType(SourceReader sourceReader)
        {
            var sourceStream = sourceReader.Streams.First(s => s.IsSelected && s.NativeMediaType.IsVideo);

            sourceStream.CurrentMediaType = GetMediaTypeRGB();

            return sourceStream;
        }

        private static MediaType GetMediaTypeRGB()
        {
            return new MediaType()
            {
                MajorType = MFMediaType.Video,
                SubType = MFMediaType.RGB32,
                AspectRatio = new AspectRatio(1, 1),
                FrameSize = new System.Drawing.Size(960, 540),
                InterlaceMode = MFVideoInterlaceMode.Progressive
            };
        }

        private static MediaType xGetMediaTypeYUY2()
        {
            return new MediaType()
            {
                MajorType = MFMediaType.Video,
                SubType = MFMediaType.YUY2,
                AspectRatio = new AspectRatio(1, 1),
                FrameSize = new System.Drawing.Size(960, 540),
                InterlaceMode = MFVideoInterlaceMode.Progressive
            };
        }

        SinkStream AddStream(SinkWriter sinkWriter, MediaType input, MediaType encoding)
        {
            var sinkStream = sinkWriter.AddStream(encoding);
            sinkStream.InputMediaType = input;
            return sinkStream;
        }

        MediaType CreateTargetAudioMediaType(MediaType nativeMediaType)
        {
            var numberOfChannels = nativeMediaType.AudioNumberOfChannels;
            var sampleRate = nativeMediaType.AudioSamplesPerSecond;

            var availableTypes = MFSystem.TranscodeGetAudioOutputAvailableTypes(TARGET_AUDIO_FORMAT, MFT_EnumFlag.All);

            var type = availableTypes
                .FirstOrDefault(t => t.AudioNumberOfChannels == numberOfChannels &&
                    t.AudioSamplesPerSecond == sampleRate &&
                    t.AudioAverageBytesPerSecond == AudioBitRate);

            type = availableTypes
                .FirstOrDefault(t => t.AudioNumberOfChannels == numberOfChannels &&
                    t.AudioSamplesPerSecond == sampleRate);

            if (type != null)
                return new MediaType(type);

            throw new Exception("Unable to find target audio format");
        }

        MediaType CreateTargetVideoMediaType(MediaType nativeMediaType)
        {
            var size = nativeMediaType.FrameSize;
            var rate = nativeMediaType.FrameRate;
            var aspect = nativeMediaType.AspectRatio;
            var bitRate = VideoBitRate;

            var mediaType = new MediaType()
            {
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
