using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Video
{
    public partial class Process
    {
        public static void Concat(Action<ProcessSample> first, Action<ProcessSample> second, ProcessSample next)
        {
            long offset = 0;

            first(sample =>
                {
                    if (sample.Timestamp > offset)
                        offset = sample.Timestamp;

                    if (!sample.Flags.EndOfStream)
                        return next(sample);

                    return true;
                });

            second(sample =>
                {
                    if (!sample.Flags.EndOfStream)
                        sample.SetSampleTime(sample.Timestamp + offset);

                    return next(sample);
                });
        }
    }
}
