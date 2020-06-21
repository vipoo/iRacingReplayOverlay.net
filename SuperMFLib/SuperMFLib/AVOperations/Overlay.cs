using System;
using System.Drawing;

namespace MediaFoundation.Net
{
    public partial class AVOperations
    {
        public static ProcessSample Overlay(Action<SourceReaderSampleWithBitmap> applyImage, ProcessSample processSample)
        {
            return SeperateAudioVideo(processSample, sample => {

                if( sample.Sample != null)
                    using (var sampleWithBitmap = new SourceReaderSampleWithBitmap(sample))
                        applyImage(sampleWithBitmap);

                return processSample(sample);
            });
        }
    }
}
