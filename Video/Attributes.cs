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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRacingReplayOverlay.Video
{
    public class VideoAttributes
    {
        public class VideoDetails
        {
            public readonly int[] SupportedAudioBitRates;
         
            public VideoDetails(int[] supportedAudioBitRates)
            {
                this.SupportedAudioBitRates = supportedAudioBitRates;
            }
        }

        public static VideoDetails For(string videoFileName)
        {
            List<int> supportedAudioBitRates = new List<int>();

            using (MFSystem.Start())
            {
                var readWriteFactory = new ReadWriteClassFactory();

                var sourceReader = readWriteFactory.CreateSourceReaderFromURL(videoFileName, null);

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
            }

            return new VideoDetails(supportedAudioBitRates.ToArray());
        }
    }
}
