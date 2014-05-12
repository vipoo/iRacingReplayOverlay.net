
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

namespace iRacingReplayOverlay.Phases.Transcoding
{
    class Transcoder
    {
        public string IntroVideoFile;
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

        internal void Frames(Func<SourceReaderSampleWithBitmap, bool> sampleFn)
        {
            foreach (var f in Frames())
                if (!sampleFn(f))
                    break;
        }

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

                var introSourceReader = readWriteFactory.CreateSourceReaderFromURL(IntroVideoFile, attributes);
                var sourceReader = readWriteFactory.CreateSourceReaderFromURL(SourceFile, attributes);
                var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(DestinationFile, attributes);

                ConnectSourceToSink(introSourceReader, sourceReader, sinkWriter);

                using (sinkWriter.BeginWriting())
                {
                    foreach (var sample in ProcessSamples(introSourceReader, sinkWriter))
                    {
                        sample.IsIntroduction = true;
                        
                        if (!sample.Flags.EndOfStream)
                            yield return sample;

                        if (sample.Timestamp > offset)
                            offset = sample.Timestamp;
                    }

                    foreach (var sample in ProcessSamples(sourceReader, sinkWriter, offset))
                    {
                        sample.IsIntroduction = false;
                        
                        yield return sample;
                    }
                }
            }
        }

        IEnumerable<SourceReaderSampleWithBitmap> ProcessSamples(SourceReader sourceReader, SinkWriter sinkWriter, long offset = 0)
        {
            foreach (var sample in sourceReader.SamplesAfterEditing(EditCuts, -offset))
            {
                var sinkStream = ProcessIncoming(sample);

                if (!sample.Flags.EndOfStream)
                    sample.SetSampleTime(sample.Timestamp + offset);

                if (sample.Stream.CurrentMediaType.IsVideo)
                    using (var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample))
                        yield return sampleWithBitmap;

                WriteSample(sinkStream, sample);
            }
        }

        void ConnectSourceToSink(SourceReader introSourceReader, SourceReader sourceReader, SinkWriter sinkWriter)
        {
            var introSourceStreams = introSourceReader.Streams
                .Where(s => s.IsSelected)
                .Select(s => new { Stream = s, NativeMediaType = s.NativeMediaType })
                .ToList();
            
            foreach (var ss in sourceReader.Streams.Where(s => s.IsSelected))
            {
                var sourceStream = ss;

                var isVideo = sourceStream.NativeMediaType.IsVideo;
                var isAudio = sourceStream.NativeMediaType.IsAudio;

                if (!isAudio && !isVideo)
                    throw new Exception("Unknown stream type");

                var targetType = isAudio ? CreateTargetAudioMediaType(sourceStream.NativeMediaType) : CreateTargetVideoMediaType(sourceStream.NativeMediaType);
                var sinkStream = sinkWriter.AddStream(targetType);

                var introStream = introSourceStreams.First(s => s.NativeMediaType.IsAudio == isAudio && s.NativeMediaType.IsVideo == isVideo);
                streamMapping.Add(sourceStream, sinkStream);
                streamMapping.Add(introStream.Stream, sinkStream);

                var mediaType = isAudio
                    ? new MediaType() { MajorType = MFMediaType.Audio, SubType = MFMediaType.PCM }
                    : new MediaType() { MajorType = MFMediaType.Video, SubType = MFMediaType.RGB32 };

                sourceStream.CurrentMediaType = mediaType;
                var introSt = introStream.Stream;
                introSt.CurrentMediaType = mediaType;
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
