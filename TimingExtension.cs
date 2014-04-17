using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayOverlay.net
{
    public static class TimingExtensions
    {
        const long OneNanoSecond = 10000000;

        public static int FromNanoToSeconds(this long nanoseconds)
        {
            return (int)(nanoseconds / OneNanoSecond);
        }

        public static long FromSecondsToNano(this int seconds)
        {
            return seconds * OneNanoSecond;
        }
    }
}
