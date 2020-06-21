using System;

namespace MediaFoundation.Net
{
    public partial class AVOperations
    {
        public static ProcessSample Cut(long from, long to, ProcessSample next)
        {
            return Cut(from, to, next, next);
        }

        public static ProcessSample Cut(long from, long to, ProcessSample beforeEdit, ProcessSample afterEdit)
        {
            var offset = to - from;
            var hasJumped = false;

            return sample =>
            {
                if (sample.Flags.EndOfStream)
                    return afterEdit(sample);

                if (from > sample.Duration)
                    throw new Exception(string.Format("Error.  Edit starting position beyond end of segment. {0}, {1}, {2}", from.FromNanoToSeconds(), sample.Duration.FromNanoToSeconds(), sample.Timestamp.FromNanoToSeconds()));

                if (to > sample.Duration)
                    throw new Exception(string.Format("Error.  Edit to position beyond end of segment. {0}, {1}", to.FromNanoToSeconds(), sample.Duration.FromNanoToSeconds()));

                if (sample.Timestamp < from)
                {
                    sample.SegmentDuration = from;
                    return beforeEdit(sample);
                }

                if (sample.Timestamp < to)
                {
                    if (!hasJumped)
                        sample.Reader.SetCurrentPosition(to);
                    hasJumped = true;
                    return true;
                }

                sample.SampleTime -= offset;
                sample.SegmentTimeStamp = sample.Timestamp - to;
                sample.SegmentDuration = sample.Duration - to;
                return afterEdit(sample);
            };
        }
    }

    public partial class AVOperations
    {
        public static ProcessSample SeperateAudioVideo(ProcessSample audioStreams, ProcessSample videoStreams)
        {
            return sample =>
            {
                if (sample.Stream.NativeMediaType.IsVideo)
                    return videoStreams(sample);

                if (sample.Stream.NativeMediaType.IsAudio)
                    return audioStreams(sample);

                throw new Exception("Unknown stream type");
            };

        }
    }
}
