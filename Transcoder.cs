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

namespace iRacingReplayOverlay.net
{
    class Transcoder
    {
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }

        Dictionary<SourceStream, SinkStream> streamMapping;

        internal IEnumerable<SourceReaderSampleWithBitmap> Frames()
        {
            streamMapping = new Dictionary<SourceStream, SinkStream>();

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
                    yield return sample;
            }
        }

        IEnumerable<SourceReaderSampleWithBitmap> ProcessSamples(SourceReader sourceReader, SinkWriter sinkWriter)
        {
            using (sinkWriter.BeginWriting())
                foreach (var sample in sourceReader.Samples())
                {
                    var sinkStream = ProcessIncoming(sample);
                    
                    if (sample.Sample != null && sample.Stream.CurrentMediaType.IsVideo)
						using(var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample) )
							yield return sampleWithBitmap;
                    
                    WriteSample(sinkStream, sample);
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

                if (!isAudio && !isVideo)
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
                sinkStream.SendStreamTick(sample.Timestamp);
        }

        static Guid TARGET_AUDIO_FORMAT = MFMediaType.WMAudioV9;
        static Guid TARGET_VIDEO_FORMAT = MFMediaType.WMV3;

        MediaType CreateTargetAudioMediaType(MediaType nativeMediaType)
        {
            var numberOfChannels = nativeMediaType.AudioNumberOfChannels;
            var sampleRate = nativeMediaType.AudioSamplesPerSecond;

            var availableTypes = MFSystem.TranscodeGetAudioOutputAvailableTypes(TARGET_AUDIO_FORMAT, MFT_EnumFlag.All);

            var type = availableTypes
                .FirstOrDefault(t => t.AudioNumberOfChannels == numberOfChannels && t.AudioSamplesPerSecond == sampleRate);

            if (type != null)
                return new MediaType(type);

            throw new Exception("Unable to find target audio format");
        }

        MediaType CreateTargetVideoMediaType(MediaType nativeMediaType)
        {
            var size = nativeMediaType.FrameSize;
            var rate = nativeMediaType.FrameRate;
            var aspect = nativeMediaType.AspectRatio;
            var bitRate = nativeMediaType.BitRate;

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
