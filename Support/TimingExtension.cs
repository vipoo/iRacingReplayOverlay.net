
namespace iRacingReplayOverlay.Support
{
    public static class TimingExtensions
    {
        const double OneNanoSecond = 10000000;

        public static double FromNanoToSeconds(this long nanoseconds)
        {
            return (double)nanoseconds / OneNanoSecond;
        }

        public static long FromSecondsToNano(this int seconds)
        {
            return seconds * (long)OneNanoSecond;
        }

        public static long FromSecondsToNano(this double seconds)
        {
            return (long)(seconds * OneNanoSecond);
        }
    }
}
