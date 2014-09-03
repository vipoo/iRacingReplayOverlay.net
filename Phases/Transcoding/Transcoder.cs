
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

        static Guid TARGET_AUDIO_FORMAT = MFMediaType.WMAudioV9;
        static Guid TARGET_VIDEO_FORMAT = MFMediaType.WMV3;

        internal void ProcessVideo(Action<SourceReader, SourceReader, ProcessSample> process)
        {
                var readWriteFactory = new ReadWriteClassFactory();

                var attributes = new Attributes
                {
                    ReadWriterEnableHardwareTransforms = true,
                    SourceReaderEnableVideoProcessing = true
                };

                var introSourceReader = IntroVideoFile == null ? null : readWriteFactory.CreateSourceReaderFromURL(IntroVideoFile, attributes);
                var sourceReader = readWriteFactory.CreateSourceReaderFromURL(SourceFile, attributes);
                var sinkWriter = readWriteFactory.CreateSinkWriterFromURL(DestinationFile, attributes);
                
                var writeToSink = ConnectStreams(introSourceReader, sourceReader, sinkWriter);

                using(sinkWriter.BeginWriting())
                    process(introSourceReader, sourceReader, writeToSink);
        }

        private ProcessSample ConnectStreams(SourceReader introSourceReader, SourceReader sourceReader, SinkWriter sinkWriter)
        {
            if (introSourceReader != null)
            {
                SetAudioMediaType(introSourceReader);
                SetVideoMediaType(introSourceReader);
            }

            var sourceAudioStream = SetAudioMediaType(sourceReader);
            var sourceVideoStream = SetVideoMediaType(sourceReader);

            var sinkAudioStream = AddStream(sinkWriter, sourceAudioStream.CurrentMediaType, CreateTargetAudioMediaType(sourceAudioStream.NativeMediaType));
            var sinkVideoStream = AddStream(sinkWriter, sourceVideoStream.CurrentMediaType, CreateTargetVideoMediaType(sourceVideoStream.NativeMediaType));

            var saveAudio = AVOperations.MediaTypeChange(sinkAudioStream, AVOperations.SaveTo(sinkAudioStream));
            var saveVideo = AVOperations.MediaTypeChange(sinkVideoStream, AVOperations.SaveTo(sinkVideoStream));

            return AVOperations.SeperateAudioVideo(saveAudio, saveVideo);
        }

        SourceStream SetAudioMediaType(SourceReader sourceReader)
        {
            try
            {
                var sourceStream = sourceReader.Streams.First(s => s.IsSelected && s.NativeMediaType.IsAudio);

                sourceStream.CurrentMediaType = new MediaType() { MajorType = MFMediaType.Audio, SubType = MFMediaType.PCM };

                return sourceStream;
            }
            catch(Exception e)
            {
                throw new Exception(string.Format("Unable to decode audio stream. {0}", e.Message), e);
            }
        }

        SourceStream SetVideoMediaType(SourceReader sourceReader)
        {
            try
            {
                var sourceStream = sourceReader.Streams.First(s => s.IsSelected && s.NativeMediaType.IsVideo);

                sourceStream.CurrentMediaType = new MediaType() { MajorType = MFMediaType.Video, SubType = MFMediaType.RGB32 };

                return sourceStream;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Unable to decode video stream. {0}", e.Message), e);
            }
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
