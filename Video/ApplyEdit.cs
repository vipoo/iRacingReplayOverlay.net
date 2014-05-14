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
        public static ProcessSample ApplyEdit(long starting, long finishing, ProcessSample beforeEdit, ProcessSample afterEdit)
        {
            bool isBeforeEdit = true;
            long offset = 0;
            long skippingFrom = 0;

            return sample =>
            {
                if (sample.Sample == null)
                    return isBeforeEdit ? beforeEdit(sample) : afterEdit(sample);

                if (sample.Timestamp < starting)
                {
                    if (sample.Timestamp > skippingFrom)
                        skippingFrom = sample.Timestamp;

                    return beforeEdit(sample);
                }

                if (sample.Timestamp > finishing)
                {
                    if (isBeforeEdit)
                    {
                        isBeforeEdit = false;
                        offset = sample.Timestamp - skippingFrom;
                    }

                    if (offset != 0 && !sample.Flags.EndOfStream)
                    {
                        var timestamp = sample.GetSampleTime();
                        sample.SetSampleTime(timestamp - offset);
                    }
                    return afterEdit(sample);
                }

                return true;
            };
        }
    }
}
