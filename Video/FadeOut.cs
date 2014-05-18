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
        public static ProcessSample FadeOut(long fadingOutFrom, long duration, ProcessSample next)
        {
            return AVOperations.SeperateAudioVideo(
                FadeOut(_AudioFadeOut(fadingOutFrom, duration, next), fadingOutFrom, next),
                FadeOut(_VideoFadeOut(fadingOutFrom, duration, next), fadingOutFrom, next));
        }

        public static ProcessSample FadeOut(SourceStream sourceStream, ProcessSample next)
        {
            long duration = 1.FromSecondsToNano();
            long fadingOutFrom = (long)sourceStream.Duration - duration;

            return FadeOut(fadingOutFrom, duration, next);
        }

        static ProcessSample FadeOut(ProcessSample streamFader, long fadingOutFrom, ProcessSample next)
        {
            return DataSamplesOnly(If(s => fadingOutFrom <= s.Timestamp, streamFader, next), next);
        }

        static ProcessSample _VideoFadeOut(long fadingOutFrom, long duration, ProcessSample next)
        {
            duration = duration / 1.FromSecondsToNano();

            return sample =>
            {
                var fadeOut = Math.Min(255, (sample.Timestamp - fadingOutFrom).FromNanoToSeconds() * 255.0 / duration);
                fadeOut = Math.Max(0, fadeOut);

                using (var videoSample = new SourceReaderSampleWithBitmap(sample))
                    videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeOut, Color.Black)), 0, 0, 1920, 1080);

                return next(sample);
            };
        }
        
        static ProcessSample _AudioFadeOut(long fadingOutFrom, long duration, ProcessSample next)
        {
            return sample =>
                {
                    var fadeOut = 1.0 - (sample.Timestamp - fadingOutFrom).FromNanoToSeconds();
         
                    FadeAudio(sample, fadeOut);

                    return next(sample);
                };
        }

        static void FadeAudio(SourceReaderSample sample, double fadeout)
        {
            using (var buffer = sample.Sample.ConvertToContiguousBuffer())
                using (var data = buffer.Lock())
                {
                    fadeout = Math.Min(1.0, fadeout);
                    fadeout = Math.Max(0.0, fadeout);

                    unsafe
                    {
                        var pData = (short*)data.Buffer.ToPointer();

                        int length;
                        buffer.instance.GetMaxLength(out length);

                        for (int i = 0; i < length / 2; i++)
                            pData[i] = (short)((double)pData[i] * fadeout);
                    }
                }
        }
    }
}
