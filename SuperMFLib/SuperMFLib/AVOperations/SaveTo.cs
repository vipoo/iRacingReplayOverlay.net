using System;

namespace MediaFoundation.Net
{
    public partial class AVOperations
    {
        public static ProcessSample SaveTo(SinkStream sinkStream)
        {
            return sample =>
            {
                if (sample.Flags.StreamTick)
                    throw new NotImplementedException();

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
