using iRacingReplayOverlay.Phases.Transcoding;
using MediaFoundation.Net;
using System;
using System.Collections.Generic;
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
            return Process.SeperateAudioVideo(next, VideoFadeOut(fadingOutFrom, duration, next));
        }

        public static ProcessSample FadeOut(SourceStream sourceStream, ProcessSample next)
        {
            return Process.SeperateAudioVideo(next, VideoFadeOut(sourceStream, next));
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
