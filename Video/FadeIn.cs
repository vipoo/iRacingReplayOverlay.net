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
//

using iRacingReplayOverlay.Phases.Transcoding;
using MediaFoundation.Net;
using System;
using System.Drawing;

namespace iRacingReplayOverlay.Video
{
    public partial class AVOperation
    {
        public static ProcessSample FadeIn(ProcessSample next)
        {
            return AVOperation.SeperateAudioVideo(
                FadeIn(_AudioFadeIn(next), next),
                FadeIn(_VideoFadeIn(next), next));
        }

        static ProcessSample FadeIn(ProcessSample<long> streamFader, ProcessSample next)
        {
            return DataSamplesOnly(Split(1.FromSecondsToNano(), streamFader, next), next);
        }

        static ProcessSample<long> _VideoFadeIn(ProcessSample next)
        {
            return (sample, fadingInFrom) =>
            {
                var fadeIn = Math.Min(255, 255 - ((sample.Timestamp - fadingInFrom).FromNanoToSeconds() * 255));
                fadeIn = Math.Max(0, fadeIn);

                using (var videoSample = new SourceReaderSampleWithBitmap(sample))
                    videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeIn, Color.Black)), 0, 0, 1920, 1080);

                return next(sample);
            };
        }

        static ProcessSample<long> _AudioFadeIn(ProcessSample next)
        {
            return (sample, fadingInFrom) =>
                {
                    var fadeOut = (sample.Timestamp - fadingInFrom).FromNanoToSeconds();

                    FadeAudio(sample, fadeOut);

                    return next(sample);
                };
        }
    }
}
