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

using MediaFoundation.Net;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace iRacingReplayOverlay.Video
{
    public class VideoAttributes
    {
        public class VideoDetails
        {
            public readonly int[] SupportedAudioBitRates;
            public readonly int BitRate;
            public readonly Size FrameSize;
            public readonly int FrameRate;
         
            public VideoDetails(int[] supportedAudioBitRates, int frameRate, Size frameSize, int bitRate)
            {
                this.SupportedAudioBitRates = supportedAudioBitRates;
                this.FrameRate = frameRate;
                this.FrameSize = frameSize;
                this.BitRate = bitRate;
            }
        }

        public static VideoDetails For(string videoFileName)
        {
            List<int> supportedAudioBitRates = new List<int>();

            using (MFSystem.Start())
            {
                var readWriteFactory = new ReadWriteClassFactory();

                var sourceReader = readWriteFactory.CreateSourceReaderFromURL(videoFileName, null);

                var videoStream = sourceReader.Streams.First(s => s.IsSelected && s.NativeMediaType.IsVideo);

                var audioStream = sourceReader.Streams.First(s => s.IsSelected && s.NativeMediaType.IsAudio);

                var channels = audioStream.NativeMediaType.AudioNumberOfChannels;
                var sampleRate = audioStream.NativeMediaType.AudioSamplesPerSecond;

                var types = MFSystem.TranscodeGetAudioOutputAvailableTypes(MediaFoundation.MFMediaType.WMAudioV9, MediaFoundation.Transform.MFT_EnumFlag.All);

                foreach (var bitRate in types
                    .Where(t => t.AudioNumberOfChannels == channels && t.AudioSamplesPerSecond == sampleRate)
                    .Select(t => t.AudioAverageBytesPerSecond)
                    .Distinct()
                    .OrderBy(t => t))
                {
                    supportedAudioBitRates.Add(bitRate * 8);
                }

                int videoBitRate = 0;
                videoStream.NativeMediaType.TryGetBitRate(out videoBitRate);

                return new VideoDetails(supportedAudioBitRates.ToArray(), videoStream.NativeMediaType.FrameRate.ToInt(), videoStream.NativeMediaType.FrameSize, videoBitRate / 1000000);
            }
        }
    }
}
