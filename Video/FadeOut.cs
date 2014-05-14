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
        public static ProcessSample VideoFadeOut(SourceStream sourceStream, ProcessSample next)
        {
            return VideoFadeOut(sourceStream, 1, next);
        }

        public static ProcessSample VideoFadeOut(SourceStream sourceStream, int duration, ProcessSample next)
        {
            long fadingOutFrom = (long)sourceStream.Duration - duration.FromSecondsToNano();

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
