using MediaFoundation.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.Video
{
    public partial class AVOperation
    {
        public static ProcessSample SaveTo(SinkStream sinkStream)
        {
            return sample =>
            {
                if (sample.Flags.StreamTick)
                    throw new NotImplementedException();
                //sinkStream.SendStreamTick(sample.Timestamp - offset);

                if (sample.Sample == null)
                    return true;

                if (sample.Count == 0)
                    sample.Sample.Discontinuity = true;

                sinkStream.WriteSample(sample.Sample);

                return true;
            };
        }
    }
}
