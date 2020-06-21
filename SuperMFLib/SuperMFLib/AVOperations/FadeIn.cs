using System;
using System.Drawing;

namespace MediaFoundation.Net
{
    public partial class AVOperations
    {
        public static ProcessSample FadeInOut(ProcessSample next)
        {
            return FadeIn(FadeOut(next));
        }

        public static ProcessSample FadeIn(ProcessSample next )
        {
            return SeperateAudioVideo(
                FadeIn(_AudioFadeIn(next), next),
                FadeIn(_VideoFadeIn(next), next));
        }

        static ProcessSample FadeIn(ProcessSample streamFader, ProcessSample next)
        {
            return DataSamplesOnly(
                s =>
                {
                    if (s.SegmentTimeStamp < TimingExtensions.OneNanoSecond)
                        return streamFader(s);

                    return next(s);
                }, next);
        }

        static ProcessSample _VideoFadeIn(ProcessSample next)
        {
            return (sample) =>
            {
                var fadeIn = Math.Min(255, 255 - ((sample.SegmentTimeStamp).FromNanoToSeconds() * 255));
                fadeIn = Math.Max(0, fadeIn);

                using (var videoSample = new SourceReaderSampleWithBitmap(sample))
                    videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeIn, Color.Black)), 0, 0, videoSample.ImageWidth, videoSample.ImageHeight);

                return next(sample);
            };
        }

        static ProcessSample _AudioFadeIn(ProcessSample next)
        {
            return (sample) =>
            {
                var fadeOut = (sample.SegmentTimeStamp).FromNanoToSeconds();

                FadeAudio(sample, fadeOut);

                return next(sample);
            };
        }
    }
}
