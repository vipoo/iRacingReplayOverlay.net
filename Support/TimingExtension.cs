
namespace iRacingReplayOverlay.Support
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
