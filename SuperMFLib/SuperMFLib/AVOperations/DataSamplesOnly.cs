using System;

namespace MediaFoundation.Net
{
    public partial class AVOperations
    {
        public static ProcessSample DataSamplesOnly(ProcessSample dataSamples, ProcessSample next)
        {
            return If(s => s.Sample != null, dataSamples, next);
        }

        public static ProcessSample If(Func<SourceReaderSample, bool> selector, ProcessSample trueSamples, ProcessSample falseSamples)
        {
            return sample =>
            {
                if (selector(sample))
                    return trueSamples(sample);

                return falseSamples(sample);
            };
        }
    }
}
