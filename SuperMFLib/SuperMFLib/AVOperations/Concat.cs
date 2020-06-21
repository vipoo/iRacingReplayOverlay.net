using System;
using System.Collections.Generic;

namespace MediaFoundation.Net
{
    public partial class AVOperations
    {
        public static Action<ProcessSample> FromSource(ISourceReader shortSourceReader, Func<bool> isAborted)
        {
            return next =>
            {
                foreach (var s in shortSourceReader.Samples())
                    if (isAborted())
                        break;
                    else
                        next(s);
            };
        }

        public static ISourceReader Combine(IEnumerable<SourceReader> readers, double averageLostSecondsBetweenFileSplits)
        {
            return new CombinedSourceReader(readers, averageLostSecondsBetweenFileSplits);
        }

        public static void StartConcat(ISourceReader reader, ProcessSample transforms, Action<long, long> next, Func<bool> isAborted)
        {
            Concat(reader, transforms, next, isAborted)(0, 0);
        }

        public static Action<long, long> Concat(ISourceReader reader, ProcessSample transforms, Action<long, long> next, Func<bool> isAborted)
        {
            return (offsetA, offsetV) =>
            {
                var newOffsetA = offsetA;
                var newOffsetV = offsetV;

                reader.SetCurrentPosition(0);
                var stream = FromSource(reader, isAborted);

                stream(s =>
                {
                    if (isAborted())
                        return false;

                    if (s.Flags.EndOfStream)
                        return false;

                    if (s.Stream.CurrentMediaType.IsVideo)
                        s.Resequence(offsetV);

                    if (s.Stream.CurrentMediaType.IsAudio)
                        s.Resequence(offsetA);

                    var r = transforms(s);

                    if (s.Stream.CurrentMediaType.IsVideo)
                        newOffsetV = s.SampleTime;

                    if (s.Stream.CurrentMediaType.IsAudio)
                        newOffsetA = s.SampleTime;

                    return r;
                });

                next(newOffsetA, newOffsetV);
            };
        }

        public static Action<long, long> Concat(ISourceReader reader, ProcessSample transforms, Func<bool> isAborted)
        {
            return (offsetA, offsetV) =>
            {
                reader.SetCurrentPosition(0);

                var stream = FromSource(reader, isAborted);
                bool firstV = false;

                stream(s =>
                {
                    if (isAborted())
                        return false;

                    if (s.Flags.EndOfStream)
                        return transforms(s);

                    if (!firstV && s.Stream.CurrentMediaType.IsVideo)
                    {
                        firstV = true;
                        return true;
                    }

                    if (s.Stream.CurrentMediaType.IsVideo)
                        s.Resequence(offsetV);

                    if (s.Stream.CurrentMediaType.IsAudio)
                        s.Resequence(offsetA);

                    return transforms(s);
                });
            };
        }
    }
}
