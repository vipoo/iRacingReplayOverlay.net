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
        public static ProcessSample MediaTypeChange(SinkStream sinkStream, ProcessSample next)
        {
            return sample =>
                {
                    if (sample.Flags.CurrentMediaTypeChanged)
                        sinkStream.InputMediaType = sample.Stream.CurrentMediaType;

                    return next(sample);
                };
        }
    }
}
