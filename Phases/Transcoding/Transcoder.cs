
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

using iRacingReplayOverlay.Video;
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

        static Guid TARGET_AUDIO_FORMAT = MFMediaType.WMAudioV9;
        static Guid TARGET_VIDEO_FORMAT = MFMediaType.WMV3;
        List<Capturing.OverlayData.BoringBit>.Enumerator nextCut;

        ProcessSample seperateAudioVideo;
        ProcessSample processSample;

        internal void Frames(Func<SourceReaderSampleWithBitmap, bool> sampleFn)
        {
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
                processSample = seperateAudioVideo;

                using (sinkWriter.BeginWriting())
                {
                    Action<ProcessSample> mainFeed = (next) =>
                    {
                        ProcessSample overlayFn = sample => {

                            if (sample.Stream.CurrentMediaType.IsVideo)
                                using (var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample))
                                    sampleFn(sampleWithBitmap);

                            return next(sample);
                        };

                        sourceReader.Samples(overlayFn);
                    };

                    Process.Concat((sFn) => introSourceReader.Samples(sFn), mainFeed, processSample );
                }
            }
        }

        ProcessSample ApplyOffset(long offset, ProcessSample next)
        {
            return sample =>
                {
                    if (!sample.Flags.EndOfStream)
                        sample.SetSampleTime(sample.Timestamp + offset);

                    return next(sample);
                };
        }

        //    SamplesAfterEditing(EditCuts, -offset))
        
        void ProcessSample(SourceReader sourceReader, Func<SourceReaderSampleWithBitmap, bool> next)
        {
            sourceReader.Samples((sample) => {

                if (sample.Stream.CurrentMediaType.IsVideo)
                    using (var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample))
                        next(sampleWithBitmap);

                return processSample(sample);
            });
        }

        void ConnectSourceToSink(SourceReader introSourceReader, SourceReader sourceReader, SinkWriter sinkWriter)
        {
            var introSourceStreams = introSourceReader.Streams
                .Where(s => s.IsSelected)
                .Select(s => new { Stream = s, NativeMediaType = s.NativeMediaType })
                .ToList();

            ProcessSample saveAudio = null;
            ProcessSample saveVideo = null;
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

                var mediaType = isAudio
                    ? new MediaType() { MajorType = MFMediaType.Audio, SubType = MFMediaType.PCM }
                    : new MediaType() { MajorType = MFMediaType.Video, SubType = MFMediaType.RGB32 };

                sourceStream.CurrentMediaType = mediaType;
                var introSt = introStream.Stream;
                introSt.CurrentMediaType = mediaType;
                sinkStream.InputMediaType = sourceStream.CurrentMediaType;
                if (isAudio)
                    saveAudio = Process.MediaTypeChange(sinkStream, Process.SaveTo(sinkStream));
                else
                    saveVideo = Process.MediaTypeChange(sinkStream, Process.SaveTo(sinkStream));
            }

            seperateAudioVideo = Process.SeperateAudioVideo(saveAudio, saveVideo);
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
