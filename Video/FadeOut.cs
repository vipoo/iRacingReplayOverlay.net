using iRacingReplayOverlay.Phases.Transcoding;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Video
{
    public partial class Process
    {
        public static ProcessSample FadeOut(long fadingOutFrom, long duration, ProcessSample next)
        {
            return Process.SeperateAudioVideo(
                AudioFadeOut(fadingOutFrom, duration, next), 
                VideoFadeOut(fadingOutFrom, duration, next)
            );
        }

        public static ProcessSample FadeOut(SourceStream sourceStream, ProcessSample next)
        {
            return Process.SeperateAudioVideo(AudioFadeIn(next), VideoFadeOut(sourceStream, next));
        }

        public static ProcessSample VideoFadeOut(SourceStream sourceStream, ProcessSample next)
        {
            return VideoFadeOut(sourceStream, 1.FromSecondsToNano(), next);
        }

        public static ProcessSample VideoFadeOut(SourceStream sourceStream, long duration, ProcessSample next)
        {
            long fadingOutFrom = (long)sourceStream.Duration - duration;

            return VideoFadeOut(fadingOutFrom, duration, next);
        }

        public static ProcessSample AudioFadeOut(long fadingOutFrom, long duration, ProcessSample next)
        {
            return sample =>
                {
                    if (sample.Sample == null)
                        return next(sample);

                    if (fadingOutFrom > sample.Timestamp)
                        return next(sample);

                    var fadeOut = (sample.Timestamp - fadingOutFrom).FromNanoToSeconds();
                    fadeOut = Math.Max(0, fadeOut);
                    fadeOut = Math.Min(1.0, 1.0 - fadeOut);
         
                    FadeAudio(sample, fadeOut);

                    return next(sample);
                };
        }

        static void FadeAudio(SourceReaderSample sample, double fadeout)
        {
            using (var buffer = sample.Sample.ConvertToContiguousBuffer())
                using (var data = buffer.Lock())
                {
                    fadeout = Math.Min(1, fadeout);
                    fadeout = Math.Max(0, fadeout);

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

        public static ProcessSample VideoFadeOut(long fadingOutFrom, long duration, ProcessSample next)
        {
            duration = duration / 1.FromSecondsToNano();

            return sample =>
            {
                if (sample.Sample == null)
                    return next(sample);

                if (fadingOutFrom > sample.Timestamp)
                    return next(sample);

                var fadeOut = Math.Min(255, (sample.Timestamp - fadingOutFrom).FromNanoToSeconds() * 255.0 / duration);
                fadeOut = Math.Max(0, fadeOut);

                using (var videoSample = new SourceReaderSampleWithBitmap(sample))
                    videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeOut, Color.Black)), 0, 0, 1920, 1080);

                return next(sample);
            };
        }
    }
}
