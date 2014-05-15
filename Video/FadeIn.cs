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
    public partial class Process
    {
        public static ProcessSample FadeIn(ProcessSample next)
        {
            return Process.SeperateAudioVideo(AudioFadeIn(next), VideoFadeIn(next));
        }

        public static ProcessSample AudioFadeIn(ProcessSample next)
        {
            long fadingInFrom = -1;

            return sample =>
                {
                    if (sample.Sample == null)
                        return next(sample);

                    if (fadingInFrom == -1)
                        fadingInFrom = sample.Timestamp;

                    if (sample.Timestamp - fadingInFrom > 1.FromSecondsToNano())
                        return next(sample);

                    var fadeOut = (sample.Timestamp - fadingInFrom).FromNanoToSeconds();
                    fadeOut = Math.Max(0, fadeOut);
                    fadeOut = Math.Min(1.0, fadeOut);

                    FadeAudio(sample, fadeOut);

                    return next(sample);
                };
        }

        public static ProcessSample VideoFadeIn(ProcessSample next)
        {
            long fadingInFrom = -1;

            return sample =>
                {
                    if (sample.Sample == null)
                        return next(sample);

                    if (fadingInFrom == -1)
                        fadingInFrom = sample.Timestamp;

                    if (sample.Timestamp > fadingInFrom + 1.FromSecondsToNano())
                        return next(sample);

                    var fadeIn = Math.Min(255, 255 - ((sample.Timestamp - fadingInFrom).FromNanoToSeconds() * 255));
                    fadeIn = Math.Max(0, fadeIn);

                    using (var videoSample = new SourceReaderSampleWithBitmap(sample))
                        videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeIn, Color.Black)), 0, 0, 1920, 1080);

                    return next(sample);
                };
        }
    }
}
