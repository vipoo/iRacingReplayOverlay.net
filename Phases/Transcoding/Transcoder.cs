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
using System.Linq;
using System.Runtime.InteropServices;
using iRacingReplayOverlay.Support;
using System.Drawing;
using System.Diagnostics;

namespace iRacingReplayOverlay.Phases.Transcoding
{
    class Transcoder
    {
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }
        public int AudioBitRate;
        public int VideoBitRate;
        public List<Capturing.OverlayData.BoringBit> EditCuts;

        Dictionary<SourceStream, SinkStream> streamMapping;

        static Guid TARGET_AUDIO_FORMAT = MFMediaType.WMAudioV9;
        static Guid TARGET_VIDEO_FORMAT = MFMediaType.WMV3;
        List<Capturing.OverlayData.BoringBit>.Enumerator nextCut;
        long offset = 0;

        internal IEnumerable<SourceReaderSampleWithBitmap> Frames()
        {
            streamMapping = new Dictionary<SourceStream, SinkStream>();

            nextCut = EditCuts.GetEnumerator();
            nextCut.MoveNext();

            using (MFSystem.Start())
            {
                var readWriteFactory = new ReadWriteClassFactory();

                var attributes = new Attributes
                {
                    ReadWriterEnableHardwareTransforms = true,
                    SourceReaderEnableVideoProcessing = true
                };

                var sourceReader = readWriteFactory.CreateSourceReaderFromURL(SourceFile, attributes);
                var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(DestinationFile, attributes);

                ConnectSourceToSink(sourceReader, sinkWriter);

                foreach (var sample in ProcessSamples(sourceReader, sinkWriter))
                {
                    yield return sample;

                }
            }
        }

        IEnumerable<SourceReaderSample> ProcessEdits(SourceReader sourceReader, IEnumerable<SourceReaderSample> source)
        {
            long skippingFrom = 0;
            bool justSkipped = false;
            Capturing.OverlayData.BoringBit currentSkip = null;
            bool isFadingOut = false;
            long fadingOutFrom = 0;
            var isFadingIn = false;
            long fadingInFrom = 0;

            foreach( var sample in source)
            {
                if( sample.Flags.EndOfStream)
                {
                    yield return sample;
                    continue;
                }

                if (justSkipped)
                {
                    if (sample.Timestamp + 0.5.FromSecondsToNano() < currentSkip.EndTime.FromSecondsToNano())
                        continue;
                }

                if (sample.Stream.CurrentMediaType.IsAudio)
                {
                    if( isFadingOut)
                    {
                        var fadeOut = (sample.Timestamp - fadingOutFrom).FromNanoToSeconds();
                        FadeAudio(sample, fadeOut);
                    }

                    yield return sample;
                    continue;
                }

                var videoSample = new SourceReaderSampleWithBitmap(sample);

                if( isFadingIn)
                {
                    var fadeIn = Math.Min(255, 255 - ((sample.Timestamp - fadingInFrom).FromNanoToSeconds() * 255));
                    fadeIn = Math.Max(0, fadeIn);

                    videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeIn, Color.Black)), 0, 0, 1920, 1080);

                    if (fadingOutFrom + 1.FromSecondsToNano() >= sample.Timestamp)
                        isFadingIn = false;
                }

                if (isFadingOut)
                {
                    var fadeOut = Math.Min(255, (sample.Timestamp - fadingOutFrom).FromNanoToSeconds() * 255);
                    fadeOut = Math.Max(0, fadeOut);

                    videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeOut, Color.Black)), 0, 0, 1920, 1080);

                    if (fadingOutFrom + 1.FromSecondsToNano() < sample.Timestamp)
                    {
                        Trace.WriteLine("Finished fading out at {0}".F(TimeSpan.FromSeconds(sample.Timestamp.FromNanoToSeconds())));
                        isFadingOut = false;
                        currentSkip = nextCut.Current;
                        skippingFrom = sample.Timestamp;
                        justSkipped = true;
                        nextCut.MoveNext();

                        Trace.WriteLine("Skipping to {0}".F(TimeSpan.FromSeconds(currentSkip.EndTime)));

                        sourceReader.SetCurrentPosition(currentSkip.EndTime.FromSecondsToNano());
                        continue;
                    }
                }
                else
                {
                    if (nextCut.Current != null && sample.Timestamp >= nextCut.Current.StartTime.FromSecondsToNano())
                    {
                        Trace.WriteLine("Starting fading out at {0}".F(TimeSpan.FromSeconds(sample.Timestamp.FromNanoToSeconds())));

                        isFadingOut = true;
                        fadingOutFrom = sample.Timestamp;
                    }
                }
                if (justSkipped)
                {
                    justSkipped = false;
                    offset += (sample.Timestamp - skippingFrom);
                    isFadingIn = true;
                    fadingInFrom = sample.Timestamp;

                    Trace.WriteLine("Adjusted timestamp is {0}".F(TimeSpan.FromSeconds(sample.Timestamp.FromNanoToSeconds())));
                    Trace.WriteLine("Offset is now {0}".F(TimeSpan.FromSeconds(offset.FromNanoToSeconds())));
                }

                yield return sample;
            }
        }

        IEnumerable<SourceReaderSampleWithBitmap> ProcessSamples(SourceReader sourceReader, SinkWriter sinkWriter)
        {
            using (sinkWriter.BeginWriting())
                foreach (var sample in ProcessEdits(sourceReader, sourceReader.Samples()))
                {
                    var sinkStream = ProcessIncoming(sample);

                    if (sample.Stream.CurrentMediaType.IsVideo)
                        using (var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample))
                        {
                            yield return sampleWithBitmap;
                        }

                    //Trace.WriteLine("{0} {1}".F(sample.Stream.CurrentMediaType.IsAudio, TimeSpan.FromSeconds(sample.Timestamp.FromNanoToSeconds())));

                    if (offset != 0 && !sample.Flags.EndOfStream)
                        sample.SetSampleTime(sample.Timestamp - offset);

                    WriteSample(sinkStream, sample);
                }
        }

        private void FadeAudio(SourceReaderSample sample, double fadeout)
        {
            var buffer = sample.Sample.ConvertToContiguousBuffer();
            var data = buffer.Lock();

            fadeout = Math.Max(1, fadeout);
            fadeout = 1 - fadeout;
            unsafe
            {
                var pData = (short*)data.Buffer.ToPointer();

                int length;
                buffer.instance.GetMaxLength(out length);

                for( int i = 0; i < length/2; i++)
                    pData[i] = (short)((double)pData[i] * fadeout);
            }

            data.Dispose();
            buffer.Dispose();
        }

        void ConnectSourceToSink(SourceReader sourceReader, SinkWriter sinkWriter)
        {
            foreach (var stream in sourceReader.Streams.Where(s => s.IsSelected))
            {
                var sourceStream = stream;

                var nativeMediaType = sourceStream.NativeMediaType;

                var isVideo = nativeMediaType.IsVideo;
                var isAudio = nativeMediaType.IsAudio;

                if (!isAudio && !isVideo)
                    throw new Exception("Unknown stream type");

                var targetType = isAudio ? CreateTargetAudioMediaType(nativeMediaType) : CreateTargetVideoMediaType(nativeMediaType);

                var sinkStream = sinkWriter.AddStream(targetType);
                streamMapping.Add(sourceStream, sinkStream);

                var mediaType = isAudio
                    ? new MediaType() { MajorType = MFMediaType.Audio, SubType = MFMediaType.PCM }
                    : new MediaType() { MajorType = MFMediaType.Video, SubType = MFMediaType.RGB32 };

                sourceStream.CurrentMediaType = mediaType;
                sinkStream.InputMediaType = sourceStream.CurrentMediaType;
            }
        }

        SinkStream ProcessIncoming(SourceReaderSample sample)
        {
            var sinkStream = streamMapping[sample.Stream];

            if (sample.Flags.CurrentMediaTypeChanged)
                sinkStream.InputMediaType = sample.Stream.CurrentMediaType;

            return sinkStream;
        }

        void WriteSample(SinkStream sinkStream, SourceReaderSample sample)
        {
            WriteStream(sinkStream, sample);
            SendStreamTick(sinkStream, sample);
        }

        void WriteStream(SinkStream sinkStream, SourceReaderSample sample)
        {
            if (sample.Sample == null)
                return;

            if (sample.Count == 0)
                sample.Sample.Discontinuity = true;

            sinkStream.WriteSample(sample.Sample);
        }

        void SendStreamTick(SinkStream sinkStream, SourceReaderSample sample)
        {
            if (sample.Flags.StreamTick)
                sinkStream.SendStreamTick(sample.Timestamp - offset);
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
