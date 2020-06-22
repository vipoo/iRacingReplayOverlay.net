using iRacingSDK;

namespace iRacingDirector.Plugin
{
    public class LeaderBoard
    {
        public readonly Driver[] Drivers;
        public readonly string RacePosition;
        public readonly string LapCounter;
    }

    public class Driver
    {
        public readonly int? Position;
        public readonly string CarNumber;
        public readonly string UserName;
        public readonly int PitStopCount;
        public readonly string ShortName;
        public readonly SessionData._DriverInfo._Drivers DriverDetails;
    }

    public class MessageSet
    {
        public string[] Messages;
        public double Time;
    }

    public class FastLap
    {
        public Driver Driver;
        public double Time;
    }
}