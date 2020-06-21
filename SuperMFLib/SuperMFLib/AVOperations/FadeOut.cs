using System;
using System.Drawing;

namespace MediaFoundation.Net
{
    public partial class AVOperations
    {
        public static ProcessSample FadeOut(ProcessSample next, long duration = TimingExtensions.OneNanoSecond)
        {
            return SeperateAudioVideo(
                aFadeOut(_AudioFadeOut(duration, next), duration, next),
                aFadeOut(_VideoFadeOut(duration, next), duration, next));
        }
        
        private static ProcessSample aFadeOut(ProcessSample streamFader, long duration, ProcessSample next)
        {
            return DataSamplesOnly(s => {
                
                var fadingOutFrom = s.SegmentDuration - duration;

                if (fadingOutFrom <= s.SegmentTimeStamp)
                    return streamFader(s);

                return next(s);
                
            }, next);
        }

        private static ProcessSample _VideoFadeOut(long duration, ProcessSample next)
        {
            return sample =>
            {
                var fadingOutFrom = sample.SegmentDuration - duration;
                var fadeOut = Math.Min(255, (sample.SegmentTimeStamp - fadingOutFrom).FromNanoToSeconds() * 255.0 / duration.FromNanoToSeconds());
                fadeOut = Math.Max(0, fadeOut);

                using (var videoSample = new SourceReaderSampleWithBitmap(sample))
                    videoSample.Graphic.FillRectangle(new SolidBrush(Color.FromArgb((int)fadeOut, Color.Black)), 0, 0, videoSample.ImageWidth, videoSample.ImageHeight);

                return next(sample);
            };
        }


        private static ProcessSample _AudioFadeOut(long duration, ProcessSample next)
        {
            return sample =>
            {
                var fadingOutFrom = sample.SegmentDuration - duration;

                var fadeOut = 1.0 - (sample.SegmentTimeStamp - fadingOutFrom).FromNanoToSeconds();

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
