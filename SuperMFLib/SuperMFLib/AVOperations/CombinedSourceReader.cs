using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MediaFoundation.Net
{
    internal class CombinedSourceReader : ISourceReader
    {
        readonly SourceReader[] readers;
        private bool reposReqeusted = false;
        private long startPosition;
        private readonly double averageLostSecondsBetweenFileSplits;

        public CombinedSourceReader(IEnumerable<SourceReader> readers, double averageLostSecondsBetweenFileSplits)
        {
            this.readers = readers.ToArray();
            this.averageLostSecondsBetweenFileSplits = averageLostSecondsBetweenFileSplits;
        }

        public long Duration
        {
            get
            {
                return readers.Sum(r => r.Duration);
            }
        }

        public IEnumerable<SourceReaderSample> Samples(int streamIndex = -2, int controlFlags = 0)
        {
            long offsetV = 0;

            var duration = Duration;
            SourceReaderSample last = null;

            var readerIndex = 0;

            foreach (var reader in readers)
            {
                readerIndex++;

                if (CheckForSegmentSkip(offsetV, readerIndex, reader))
                {
                    offsetV += (reader.Duration + averageLostSecondsBetweenFileSplits.FromSecondsToNano());
                    continue;
                }

                Trace.WriteLine(string.Format("File index: {0}, Duration: {1}, OffsetV: {1}", readerIndex, reader.Duration.FromNanoToSeconds(), offsetV.FromNanoToSeconds()));

                foreach (var sample in reader.Samples(streamIndex, controlFlags))
                {
                    if (isEndOfStream(ref last, sample))
                        continue;

                    var isContinue = false;
                    var isBreak = false;

                    CheckForReposition(offsetV, readerIndex, sample, ref isContinue, ref isBreak);

                    if (isBreak)
                        break;

                    if (isContinue)
                        continue;

                    ResequenceSample(offsetV, duration, sample);

                    yield return sample;
                }

                
                offsetV += (reader.Duration + averageLostSecondsBetweenFileSplits.FromSecondsToNano());
            }

            if (last != null)
                yield return last;
        }

        private bool CheckForSegmentSkip(long offsetV, int readerIndex, SourceReader reader)
        {
            var isSkipSegment = reader.Duration + offsetV < startPosition;

            if (isSkipSegment)
            {
                Trace.WriteLine(string.Format("File index: {0}, Skipping entire file. Duration: {1}, offset: {2}, startPosition: {3}",
                    readerIndex,
                    reader.Duration.FromNanoToSeconds(),
                    offsetV.FromNanoToSeconds(),
                    startPosition.FromNanoToSeconds()
                ));
                
            }

            return isSkipSegment;
        }

        private void ResequenceSample(long offsetV, long duration, SourceReaderSample sample)
        {
            if (sample.Stream.NativeMediaType.IsVideo)
                sample.Resequence(offsetV, duration, this);

            if (sample.Stream.NativeMediaType.IsAudio)
                sample.Resequence(offsetV, duration, this);
        }

        private void CheckForReposition(long offsetV, int readerIndex, SourceReaderSample sample, ref bool isContinue, ref bool isBreak)
        {
            if (reposReqeusted && sample.SampleTime + offsetV < startPosition)
            {
                var newPos = startPosition - offsetV;

                Trace.WriteLine(string.Format("File index: {0}, Reposition: {1}-{2} = {3}",
                    readerIndex,
                    startPosition.FromNanoToSeconds(),
                    offsetV.FromNanoToSeconds(),
                    (newPos).FromNanoToSeconds()
                    ));

                if (newPos > sample.Duration)
                {
                    //nextOffsetA = nextOffsetV = sample.Duration;
                    Trace.WriteLine(string.Format(
                        "File index: {0}, Skipping remainder of file. sampleDuration: {1}",
                        readerIndex, sample.Duration));
                    isBreak = true;
                }
                else
                {
                    sample.Reader.SetCurrentPosition(newPos);
                    reposReqeusted = false;
                    isContinue = true;
                }
            }
        }

        private bool isEndOfStream(ref SourceReaderSample last, SourceReaderSample sample)
        {
            var endOfStream = sample.Flags.EndOfStream;
            if (endOfStream)
                last = sample;
            return endOfStream;
        }

        public void SetCurrentPosition(long position)
        {
            this.startPosition = position;
            this.reposReqeusted = true;
        }
    }
}