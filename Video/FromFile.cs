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
        public static Action FromFile(ReadWriteClassFactory factory, Attributes attributes, string filename, ProcessSample next)
        {
            var sourceReader = factory.CreateSourceReaderFromURL(filename, attributes);

            return () => sourceReader.Samples(next);
        }
    }
}
